using System.Net.Sockets;
using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a native Ipv6 socket address structure (<c>sockaddr_in6</c>).
    /// </summary>
    /// <remarks>
    ///     This structure is used for Ipv6 socket operations and matches the native layout of <c>sockaddr_in6</c>.
    ///     It includes the address family, port, flow information, the 128‑bit Ipv6 address, and a scope ID.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    internal unsafe struct sockaddr_in6
    {
        /// <summary>
        ///     The address family (must be <see cref="AddressFamily.InterNetworkV6" />).
        /// </summary>
        [FieldOffset(0)] public ushort sin6_family;

        /// <summary>
        ///     The port number in network byte order.
        /// </summary>
        [FieldOffset(2)] public ushort sin6_port;

        /// <summary>
        ///     The flow information (usually 0).
        /// </summary>
        [FieldOffset(4)] public uint sin6_flowinfo;

        /// <summary>
        ///     The 128‑bit Ipv6 address as a 16‑byte array.
        /// </summary>
        [FieldOffset(8)] public fixed byte sin6_addr[16];

        /// <summary>
        ///     The Ipv4 address.
        /// </summary>
        [FieldOffset(20)] public uint sin4_addr;

        /// <summary>
        ///     The scope ID for link‑local or site‑local addresses.
        /// </summary>
        [FieldOffset(24)] public uint sin6_scope_id;
    }
}