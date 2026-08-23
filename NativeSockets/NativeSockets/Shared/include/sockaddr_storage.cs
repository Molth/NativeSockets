using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a generic socket address storage structure that can hold any address family (<c>sockaddr_storage</c>).
    /// </summary>
    /// <remarks>
    ///     This structure is large enough to contain both Ipv4 and Ipv6 addresses, and is aligned to the most strict alignment
    ///     requirement of the system. It is used for functions that need to accept any address family without knowing the
    ///     exact type.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 128)]
    internal struct sockaddr_storage
    {
        /// <summary>
        ///     The address family of the stored address.
        /// </summary>
        [FieldOffset(0)] public ushort ss_family;

        /// <summary>
        ///     The port number in network byte order.
        /// </summary>
        [FieldOffset(2)] public ushort ss_port;

        /// <summary>
        ///     Alignment padding to ensure the structure is properly aligned in memory.
        /// </summary>
        [FieldOffset(8)] public long __ss_align;
    }
}