using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides helper methods and structures for Windows Sockets (Winsock) operations.
    /// </summary>
    internal static class WinSock2
    {
        /// <summary>
        ///     Gets a pre‑computed Ipv4‑mapped Ipv6 address structure (::ffff:0:0).
        /// </summary>
        private static sockaddr_in4_map_in6 ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6 { get; } = sockaddr_in4_map_in6.Create(stackalloc byte[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF });

        /// <summary>
        ///     Converts a 16‑bit unsigned integer from host byte order to network byte order (big‑endian).
        /// </summary>
        /// <param name="host">The value in host byte order.</param>
        /// <returns>The value in network byte order.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HOST_TO_NET_16(ushort host) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(host) : host;

        /// <summary>
        ///     Converts a 32‑bit unsigned integer from host byte order to network byte order (big‑endian).
        /// </summary>
        /// <param name="host">The value in host byte order.</param>
        /// <returns>The value in network byte order.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint HOST_TO_NET_32(uint host) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(host) : host;

        /// <summary>
        ///     Converts a 16‑bit unsigned integer from network byte order (big‑endian) to host byte order.
        /// </summary>
        /// <param name="network">The value in network byte order.</param>
        /// <returns>The value in host byte order.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort NET_TO_HOST_16(ushort network) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(network) : network;

        /// <summary>
        ///     Converts a 32‑bit unsigned integer from network byte order (big‑endian) to host byte order.
        /// </summary>
        /// <param name="network">The value in network byte order.</param>
        /// <returns>The value in host byte order.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NET_TO_HOST_32(uint network) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(network) : network;

        /// <summary>
        ///     Maps the Ipv4 address to an Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MapToIpv6(ref byte sin6_addr) => Unsafe.WriteUnaligned(ref sin6_addr, ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6);

        /// <summary>
        ///     Maps the Ipv4 address to an Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 16‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        /// <param name="sin_addr">The 4‑byte span containing the Ipv4 address data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MapToIpv6(ref byte sin6_addr, uint sin_addr)
        {
            MapToIpv6(ref sin6_addr);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref sin6_addr, 12), sin_addr);
        }

        /// <summary>
        ///     Gets whether the ip address is an Ipv4-mapped Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        /// <returns>
        ///     Returns true if the ip address is an Ipv4-mapped Ipv6 address;
        ///     otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIpv4MappedToIpv6(ref byte sin6_addr) => Unsafe.ReadUnaligned<sockaddr_in4_map_in6>(ref sin6_addr).Equals(ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6);
    }
}