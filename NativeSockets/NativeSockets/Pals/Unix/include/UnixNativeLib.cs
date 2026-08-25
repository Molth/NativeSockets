using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using static NativeSockets.UnixNativeLibName;

#pragma warning disable SYSLIB1054

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides Unix-specific socket operations using libc functions.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class UnixNativeLib
    {
        /// <summary>
        ///     Command for <c>fcntl</c> to get the file status flags.
        /// </summary>
        public const int F_GETFL = 3;

        /// <summary>
        ///     Command for <c>fcntl</c> to set the file status flags.
        /// </summary>
        public const int F_SETFL = 4;

        /// <summary>
        ///     Binds a socket to a local address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle (file descriptor).</param>
        /// <param name="__socketAddress_native">Pointer to the socket address structure.</param>
        /// <param name="__socketAddressSize_native">The size of the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "bind", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _bind(int __socketHandle_native, sockaddr* __socketAddress_native, uint __socketAddressSize_native);

        /// <summary>
        ///     Retrieves the local name (address) of a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__socketAddress_native">Pointer to a buffer that receives the local address.</param>
        /// <param name="__socketAddressSize_native">
        ///     Pointer to the size of the address buffer; on input holds the buffer size, on
        ///     output the actual address size.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "getsockname", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _getsockname(int __socketHandle_native, sockaddr* __socketAddress_native, uint* __socketAddressSize_native);

        /// <summary>
        ///     Creates a new socket.
        /// </summary>
        /// <param name="af">The address family (e.g., AF_INET, AF_INET6).</param>
        /// <param name="type">The socket type (e.g., SOCK_STREAM, SOCK_DGRAM).</param>
        /// <param name="protocol">The protocol (e.g., IPPROTO_TCP, IPPROTO_UDP).</param>
        /// <returns>The socket file descriptor on success; otherwise -1.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "socket", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _socket(int af, int type, int protocol);

        /// <summary>
        ///     Performs file control operations on a socket (e.g., setting non-blocking mode).
        /// </summary>
        /// <param name="fd">The socket file descriptor.</param>
        /// <param name="cmd">The command to perform (e.g., F_GETFL, F_SETFL).</param>
        /// <param name="arg">The argument for the command.</param>
        /// <returns>The result of the operation; -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "fcntl", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _fcntl(int fd, int cmd, int arg);

        /// <summary>
        ///     Connects a socket to a remote address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__socketAddress_native">Pointer to the socket address structure.</param>
        /// <param name="__socketAddressSize_native">The size of the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "connect", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _connect(int __socketHandle_native, sockaddr* __socketAddress_native, uint __socketAddressSize_native);

        /// <summary>
        ///     Closes a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "close", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _close(int __socketHandle_native);

        /// <summary>
        ///     Polls a set of sockets for I/O activity.
        /// </summary>
        /// <param name="fds">Pointer to an array of <see cref="pollfd" /> structures.</param>
        /// <param name="nfds">The number of structures in the array.</param>
        /// <param name="timeout">The timeout in milliseconds; -1 for infinite.</param>
        /// <returns>The number of events occurred, 0 on timeout, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "poll", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _poll(pollfd* fds, nuint nfds, int timeout);

        /// <summary>Converts an Ipv4 or Ipv6 address string to its binary representation (inet_pton).</summary>
        /// <param name="Family">The address family (AF_INET or AF_INET6).</param>
        /// <param name="pszAddrString">Pointer to the null‑terminated string containing the address.</param>
        /// <param name="pAddrBuf">Pointer to the buffer where the binary address will be stored.</param>
        /// <returns>1 on success, 0 if the input string is not a valid address, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "inet_pton", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _inet_pton(int Family, void* pszAddrString, void* pAddrBuf);

        /// <summary>
        ///     Resolves a host name and service name to a list of socket addresses (getaddrinfo).
        /// </summary>
        /// <param name="pNodeName">Pointer to the host name or ip address string.</param>
        /// <param name="pServiceName">Pointer to the service name or port number string.</param>
        /// <param name="pHints">Pointer to an <see cref="addrinfo" /> structure providing hints.</param>
        /// <param name="ppResult">Pointer to a pointer that receives the linked list of <see cref="addrinfo" /> structures.</param>
        /// <returns>0 on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "getaddrinfo", CallingConvention = CALLING_CONVENTION)]
        public static extern int _getaddrinfo(byte* pNodeName, byte* pServiceName, addrinfo* pHints, addrinfo** ppResult);

        /// <summary>
        ///     Frees the memory allocated by getaddrinfo (freeaddrinfo).
        /// </summary>
        /// <param name="pAddrInfo">Pointer to the addrinfo structure to free.</param>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "freeaddrinfo", CallingConvention = CALLING_CONVENTION)]
        public static extern void _freeaddrinfo(addrinfo* pAddrInfo);

        /// <summary>
        ///     Converts a binary Ipv4 or Ipv6 address to a string (inet_ntop).
        /// </summary>
        /// <param name="Family">The address family (AF_INET or AF_INET6).</param>
        /// <param name="pAddr">Pointer to the binary address.</param>
        /// <param name="pStringBuf">Pointer to the buffer that receives the string.</param>
        /// <param name="StringBufSize">The size of the buffer.</param>
        /// <returns>A pointer to the string buffer on success; otherwise <see langword="null" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "inet_ntop", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern byte* _inet_ntop(int Family, void* pAddr, byte* pStringBuf, uint StringBufSize);

        /// <summary>
        ///     Performs reverse name resolution (getnameinfo) on a socket address.
        /// </summary>
        /// <param name="pSockaddr">Pointer to the socket address structure.</param>
        /// <param name="SockaddrLength">The size of the socket address.</param>
        /// <param name="pNodeBuffer">Pointer to a buffer that receives the host name.</param>
        /// <param name="NodeBufferSize">The size of the host name buffer.</param>
        /// <param name="pServiceBuffer">Pointer to a buffer that receives the service name.</param>
        /// <param name="ServiceBufferSize">The size of the service name buffer.</param>
        /// <param name="Flags">Flags controlling the resolution (e.g., NI_NAMEREQD).</param>
        /// <returns>0 on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "getnameinfo", CallingConvention = CALLING_CONVENTION)]
        public static extern int _getnameinfo(sockaddr* pSockaddr, uint SockaddrLength, byte* pNodeBuffer, uint NodeBufferSize, byte* pServiceBuffer, uint ServiceBufferSize, int Flags);

        /// <summary>
        ///     Builds a <see cref="NativeScopedArray{iovec}" /> from an array of <see cref="NativeIoSlice" /> structures.
        /// </summary>
        /// <param name="buffer">A span that can be used for temporary storage (e.g., stackalloc).</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <returns>A <see cref="NativeScopedArray{iovec}" /> that wraps the converted buffers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeScopedArray<iovec> Build(Span<iovec> buffer, NativeIoSlice* buffers, int bufferCount)
        {
            NativeScopedArray<iovec> __buffers_native = new NativeScopedArray<iovec>(buffer, bufferCount);
            Span<iovec> span = __buffers_native.AsSpan();
            for (int i = 0; i < bufferCount; ++i)
                span[i] = new iovec(buffers[i].Buffer, (nuint)buffers[i].Length);
            return __buffers_native;
        }
    }
}