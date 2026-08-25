using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides helper methods and structures for Windows Sockets (Winsock) operations.
    /// </summary>
    internal static class WinSock2
    {
        /// <summary>
        ///     Maximum length of a host name string (including the null terminator)
        ///     for use with <c>getnameinfo</c> and similar APIs.
        /// </summary>
        public const int NI_MAXHOST = 1025;

        /// <summary>
        ///     Gets a pre‑computed Ipv4‑mapped Ipv6 address structure (::ffff:0:0).
        /// </summary>
        private static ReadOnlySpan<byte> ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6 => new byte[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0xFF };

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
        ///     Maps the Ipv4 address to an Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MapToIpv6(ref byte sin6_addr) => SpanHelpers.Copy(ref sin6_addr, ref MemoryMarshal.GetReference(ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6), 12);

        /// <summary>
        ///     Maps the Ipv4 address to an Ipv6 address.
        /// </summary>
        /// <param name="sin6_addr">The 16‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        /// <param name="sin4_addr">The 4‑byte span containing the Ipv4 address data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MapToIpv6(ref byte sin6_addr, uint sin4_addr)
        {
            MapToIpv6(ref sin6_addr);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref sin6_addr, 12), sin4_addr);
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
        public static bool IsIpv4MappedToIpv6(ref byte sin6_addr) => MemoryMarshal.CreateReadOnlySpan(ref sin6_addr, 12).SequenceEqual(ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6);

        /// <summary>
        ///     Converts the specified text to a null-terminated ASCII byte array,
        ///     suitable for use with native APIs that expect null-terminated strings (e.g., <c>inet_pton</c>, <c>getaddrinfo</c>).
        /// </summary>
        /// <param name="buffer">
        ///     A temporary span that can be used for storage;
        ///     if the required size exceeds the span, a larger buffer may be allocated.
        /// </param>
        /// <param name="text">The text to convert to ASCII.</param>
        /// <returns>A array that owns the null-terminated ASCII byte array. The caller should dispose it when done.</returns>
        public static NativeScopedArray<byte> GetBytes(Span<byte> buffer, ReadOnlySpan<char> text)
        {
            int byteCount = Encoding.ASCII.GetByteCount(text);
            NativeScopedArray<byte> array = new NativeScopedArray<byte>(buffer, byteCount + 1);
            Span<byte> bytes = array.AsSpan();
            Encoding.ASCII.GetBytes(text, bytes);
            bytes[byteCount] = (byte)'\0';
            return array;
        }
    }
}