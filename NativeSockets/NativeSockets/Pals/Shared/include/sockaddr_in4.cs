using System.Net.Sockets;
using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a native Ipv4 socket address structure (<c>sockaddr_in</c>).
    /// </summary>
    /// <remarks>
    ///     This structure is used for Ipv4 socket operations and is
    ///     compatible with the native <c>sockaddr_in</c> on both Windows and Unix.
    ///     It contains the address family, port, Ipv4 address, and a zero‑padding field.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal unsafe struct sockaddr_in4
    {
        /// <summary>
        ///     The address family (must be <see cref="AddressFamily.InterNetwork" />).
        /// </summary>
        [FieldOffset(0)] public ushort sin4_family;

        /// <summary>
        ///     The port number in network byte order.
        /// </summary>
        [FieldOffset(2)] public ushort sin4_port;

        /// <summary>
        ///     The Ipv4 address.
        /// </summary>
        [FieldOffset(4)] public uint sin4_addr;

        /// <summary>
        ///     Padding to align the structure to the size of <c>sockaddr</c> (8 bytes of zeros).
        /// </summary>
        [FieldOffset(8)] public fixed byte sin4_zero[8];
    }
}