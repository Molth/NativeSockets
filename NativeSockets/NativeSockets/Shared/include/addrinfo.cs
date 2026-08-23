using System.Runtime.InteropServices;

#pragma warning disable CS8981

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents the address information structure returned by <c>getaddrinfo</c>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct addrinfo
    {
        /// <summary>
        ///     Flags controlling behavior (e.g., AI_PASSIVE, AI_CANONNAME).
        /// </summary>
        public int ai_flags;

        /// <summary>
        ///     The address family (AF_INET, AF_INET6, etc.).
        /// </summary>
        public int ai_family;

        /// <summary>
        ///     The socket type (SOCK_STREAM, SOCK_DGRAM, etc.).
        /// </summary>
        public int ai_socktype;

        /// <summary>
        ///     The protocol (IPPROTO_TCP, IPPROTO_UDP, etc.).
        /// </summary>
        public int ai_protocol;

        /// <summary>
        ///     The length of the socket address pointed to by <see cref="ai_addr" />.
        /// </summary>
        public nuint ai_addrlen;

        /// <summary>
        ///     Pointer to a null-terminated string containing the canonical
        ///     name of the host (if AI_CANONNAME was set).
        /// </summary>
        public byte* ai_canonname;

        /// <summary>
        ///     Pointer to a <see cref="sockaddr" /> structure containing the socket address.
        /// </summary>
        public sockaddr* ai_addr;

        /// <summary>
        ///     Pointer to the next <see cref="addrinfo" /> structure in the linked list.
        /// </summary>
        public addrinfo* ai_next;
    }
}