using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides helper methods and structures for Windows Sockets (Winsock) operations.
    /// </summary>
    internal static unsafe class WinSock2
    {
        /// <summary>
        ///     Maximum length of a host name string (including the null terminator)
        ///     for use with <c>getnameinfo</c> and similar APIs.
        /// </summary>
        public const int NI_MAXHOST = 1025;

        /// <summary>
        ///     Gets a pre‑computed Ipv4‑mapped Ipv6 address structure (::ffff:0:0).
        /// </summary>
        private static ReadOnlySpan<byte> AF_INET_4_MAPPED_AF_INET_6_PREFIX => new byte[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF };

        /// <summary>
        ///     Converts a 16‑bit unsigned integer from host byte order to network byte order (big‑endian).
        /// </summary>
        /// <param name="host">The value in host byte order.</param>
        /// <returns>The value in network byte order.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HOST_TO_NET_16(ushort host) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(host) : host;

        /// <summary>
        ///     Converts a 16‑bit unsigned integer from network byte order (big‑endian) to host byte order.
        /// </summary>
        /// <param name="network">The value in network byte order.</param>
        /// <returns>The value in host byte order.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort NET_TO_HOST_16(ushort network) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(network) : network;

        /// <summary>
        ///     Gets whether the ip address is an Ipv4-mapped Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        /// <returns>
        ///     Returns true if the ip address is an Ipv4-mapped Ipv6 address;
        ///     otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIpv4MappedToIpv6(ref byte sin6_addr) => MemoryMarshal.CreateReadOnlySpan(ref sin6_addr, 12).SequenceEqual(AF_INET_4_MAPPED_AF_INET_6_PREFIX);

        /// <summary>
        ///     Writes the 12‑byte prefix to an Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteIpv6Prefix(ref byte sin6_addr) => SpanHelpers.Copy(ref sin6_addr, ref MemoryMarshal.GetReference(AF_INET_4_MAPPED_AF_INET_6_PREFIX), 12);

        /// <summary>
        ///     Maps the Ipv4 address to an Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 16‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        /// <param name="sin4_addr">The 4‑byte span containing the Ipv4 address data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MapIpv4ToIpv6(ref byte sin6_addr, uint sin4_addr)
        {
            WriteIpv6Prefix(ref sin6_addr);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref sin6_addr, 12), sin4_addr);
        }

        /// <summary>
        ///     Normalizes the address to an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the target Ipv6 socket address structure to fill.</param>
        /// <param name="addressStorage">Reference to the source address storage, which may contain an Ipv4 or Ipv6 address.</param>
        /// <param name="ADDRESS_FAMILY_INTER_NETWORK_V4">The address family value for Ipv4 used by the current platform.</param>
        /// <param name="ADDRESS_FAMILY_INTER_NETWORK_V6">The address family value for Ipv6 used by the current platform.</param>
        [MustBePinned(nameof(addressStorage))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeToIpv6(sockaddr_in6* socketAddress, [MustBePinned] in sockaddr_storage addressStorage, [Constant] ushort ADDRESS_FAMILY_INTER_NETWORK_V4, [Constant] ushort ADDRESS_FAMILY_INTER_NETWORK_V6)
        {
            if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V4)
            {
                sockaddr_in4* __socketAddress_native = (sockaddr_in4*)Unsafe.AsPointer(ref Unsafe.AsRef(in addressStorage));
                socketAddress->sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;
                socketAddress->sin6_port = __socketAddress_native->sin4_port;
                socketAddress->sin6_flowinfo = 0;
                MapIpv4ToIpv6(ref Unsafe.AsRef<byte>(socketAddress->sin6_addr), __socketAddress_native->sin4_addr);
                socketAddress->sin6_scope_id = 0;
            }
            else if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V6)
            {
                sockaddr_in6* __socketAddress_native = (sockaddr_in6*)Unsafe.AsPointer(ref Unsafe.AsRef(in addressStorage));
                *socketAddress = *__socketAddress_native;
            }
        }
    }
}