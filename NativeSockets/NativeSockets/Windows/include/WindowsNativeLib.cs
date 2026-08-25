using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

#pragma warning disable SYSLIB1054

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations for sending and receiving data.
    ///     This class contains Windows-specific implementations using Winsock.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class WindowsNativeLib
    {
        /// <summary>
        ///     The name of the native library containing the socket functions (Winsock 2.2).
        /// </summary>
        private const string NATIVE_LIBRARY = "ws2_32.dll";

        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        private const CallingConvention CALLING_CONVENTION = CallingConvention.StdCall;

        /// <summary>
        ///     Starts up the Winsock library (WSAStartup).
        /// </summary>
        /// <param name="wVersionRequested">The highest version of Winsock that the caller can support (e.g., 0x0202 for 2.2).</param>
        /// <param name="lpWSAData">Pointer to a <see cref="WSAData" /> structure that receives the Winsock implementation details.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSAStartup", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _WSAStartup(short wVersionRequested, WSAData* lpWSAData);

        /// <summary>
        ///     Cleans up the Winsock library (WSACleanup).
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSACleanup", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _WSACleanup();

        /// <summary>
        ///     Binds a socket to a local address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__socketAddress_native">Pointer to the socket address structure.</param>
        /// <param name="__socketAddressSize_native">The size of the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "bind", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _bind(nint __socketHandle_native, sockaddr* __socketAddress_native, int __socketAddressSize_native);

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
        [DllImport(NATIVE_LIBRARY, EntryPoint = "getsockname", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _getsockname(nint __socketHandle_native, sockaddr* __socketAddress_native, int* __socketAddressSize_native);

        /// <summary>
        ///     Creates a socket (WSASocketW).
        /// </summary>
        /// <param name="__addressFamily_native">The address family.</param>
        /// <param name="__socketType_native">The socket type.</param>
        /// <param name="__protocolType_native">The protocol type.</param>
        /// <param name="__protocolInfo_native">Pointer to a WSAPROTOCOL_INFO structure (can be <see langword="null" />).</param>
        /// <param name="__group_native">Reserved; must be 0.</param>
        /// <param name="__flags_native">Socket flags (e.g., WSA_FLAG_OVERLAPPED).</param>
        /// <returns>The native socket handle on success; otherwise -1.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSASocketW", CallingConvention = CALLING_CONVENTION)]
        public static extern nint _WSASocketW(AddressFamily __addressFamily_native, SocketType __socketType_native, ProtocolType __protocolType_native, nint __protocolInfo_native, uint __group_native, int __flags_native);

        /// <summary>
        ///     Controls the I/O mode of a socket (ioctlsocket).
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__cmd_native">The command to perform (e.g., FIONBIO for non-blocking).</param>
        /// <param name="__argp_native">Pointer to the argument for the command.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "ioctlsocket", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _ioctlsocket(nint __socketHandle_native, int __cmd_native, int* __argp_native);

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__optionLevel_native">The option level.</param>
        /// <param name="__optionName_native">The option name.</param>
        /// <param name="__optionValue_native">Pointer to the option value.</param>
        /// <param name="__optionLength_native">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "setsockopt", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _setsockopt(nint __socketHandle_native, SocketOptionLevel __optionLevel_native, SocketOptionName __optionName_native, int* __optionValue_native, int __optionLength_native);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__optionLevel_native">The option level.</param>
        /// <param name="__optionName_native">The option name.</param>
        /// <param name="__optionValue_native">Pointer to a buffer that receives the option value.</param>
        /// <param name="__optionLength_native">Pointer to the size of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "getsockopt", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _getsockopt(nint __socketHandle_native, SocketOptionLevel __optionLevel_native, SocketOptionName __optionName_native, byte* __optionValue_native, int* __optionLength_native);

        /// <summary>
        ///     Connects a socket to a remote address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="name">Pointer to the socket address structure.</param>
        /// <param name="namelen">The size of the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "connect", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _connect(nint __socketHandle_native, sockaddr* name, int namelen);

        /// <summary>
        ///     Closes a socket (closesocket).
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "closesocket", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _closesocket(nint __socketHandle_native);

        /// <summary>Sends data on a connected socket.</summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer containing the data to send.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the send operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "send", CallingConvention = CALLING_CONVENTION)]
        public static extern int _send(nint __socketHandle_native, byte* __pinnedBuffer_native, int __len_native, SocketFlags __socketFlags_native);

        /// <summary>Receives data on a connected socket.</summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer where received data will be stored.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "recv", CallingConvention = CALLING_CONVENTION)]
        public static extern int _recv(nint __socketHandle_native, byte* __pinnedBuffer_native, int __len_native, SocketFlags __socketFlags_native);

        /// <summary>Sends data to a specified destination address.</summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer containing the data to send.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the send operation.</param>
        /// <param name="__socketAddress_native">Pointer to the destination socket address.</param>
        /// <param name="__socketAddressSize_native">Size of the destination address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "sendto", CallingConvention = CALLING_CONVENTION)]
        public static extern int _sendto(nint __socketHandle_native, byte* __pinnedBuffer_native, int __len_native, SocketFlags __socketFlags_native, byte* __socketAddress_native, int __socketAddressSize_native);

        /// <summary>Receives data from a socket and captures the source address.</summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer where received data will be stored.</param>
        /// <param name="__len_native">The maximum length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the receive operation.</param>
        /// <param name="__socketAddress_native">Pointer to a buffer that receives the source address.</param>
        /// <param name="__socketAddressSize_native">
        ///     Pointer to the size of the address buffer; on input holds the buffer size, on
        ///     output the actual address size.
        /// </param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "recvfrom", CallingConvention = CALLING_CONVENTION)]
        public static extern int _recvfrom(nint __socketHandle_native, byte* __pinnedBuffer_native, int __len_native, SocketFlags __socketFlags_native, byte* __socketAddress_native, int* __socketAddressSize_native);

        /// <summary>
        ///     Polls a set of sockets for I/O activity (select).
        /// </summary>
        /// <param name="__ignoredParameter_native">Ignored; pass 0.</param>
        /// <param name="__readfds_native">Pointer to the read file descriptor set.</param>
        /// <param name="__writefds_native">Pointer to the write file descriptor set.</param>
        /// <param name="__exceptfds_native">Pointer to the except file descriptor set.</param>
        /// <param name="__timeout_native">Pointer to a <see cref="TimeValue" /> structure specifying the timeout.</param>
        /// <returns>The number of sockets ready, 0 on timeout, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "select", CallingConvention = CALLING_CONVENTION)]
        public static extern int _select(int __ignoredParameter_native, nint* __readfds_native, nint* __writefds_native, nint* __exceptfds_native, TimeValue* __timeout_native);

        /// <summary>
        ///     Retrieves the last socket error code (WSAGetLastError).
        /// </summary>
        /// <returns>The last Winsock error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSAGetLastError", CallingConvention = CALLING_CONVENTION)]
        public static extern int _WSAGetLastError();

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

        /// <summary>Converts an Ipv4 or Ipv6 address string to its binary representation (inet_pton).</summary>
        /// <param name="Family">The address family (AF_INET or AF_INET6).</param>
        /// <param name="pszAddrString">Pointer to the null‑terminated string containing the address.</param>
        /// <param name="pAddrBuf">Pointer to the buffer where the binary address will be stored.</param>
        /// <returns>1 on success, 0 if the input string is not a valid address, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "inet_pton", CallingConvention = CALLING_CONVENTION)]
        public static extern int _inet_pton(int Family, void* pszAddrString, void* pAddrBuf);

        /// <summary>
        ///     Converts a binary Ipv4 or Ipv6 address to a string (inet_ntop).
        /// </summary>
        /// <param name="Family">The address family (AF_INET or AF_INET6).</param>
        /// <param name="pAddr">Pointer to the binary address.</param>
        /// <param name="pStringBuf">Pointer to the buffer that receives the string.</param>
        /// <param name="StringBufSize">The size of the buffer.</param>
        /// <returns>A pointer to the string buffer on success; otherwise <see langword="null" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "inet_ntop", CallingConvention = CALLING_CONVENTION)]
        public static extern byte* _inet_ntop(int Family, void* pAddr, byte* pStringBuf, nuint StringBufSize);

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
        public static extern int _getnameinfo(sockaddr* pSockaddr, int SockaddrLength, byte* pNodeBuffer, ulong NodeBufferSize, byte* pServiceBuffer, ulong ServiceBufferSize, int Flags);

        /// <summary>
        ///     Performs a Winsock I/O control operation (WSAIoctl).
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__ioControlCode_native">The control code.</param>
        /// <param name="__inBuffer_native">Pointer to the input buffer.</param>
        /// <param name="__inBufferSize_native">The size of the input buffer.</param>
        /// <param name="__outBuffer_native">Pointer to the output buffer.</param>
        /// <param name="__outBufferSize_native">The size of the output buffer.</param>
        /// <param name="__bytesTransferred_native">Pointer to a variable that receives the number of bytes transferred.</param>
        /// <param name="__overlapped_native">Pointer to an overlapped structure (can be <see langword="null" />).</param>
        /// <param name="__completionRoutine_native">A completion routine (can be <see langword="null" />).</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSAIoctl", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _WSAIoctl(nint __socketHandle_native, int __ioControlCode_native, byte* __inBuffer_native, int __inBufferSize_native, byte* __outBuffer_native, int __outBufferSize_native, int* __bytesTransferred_native, nint __overlapped_native, nint __completionRoutine_native);

        /// <summary>
        ///     Sends data using Winsock scatter/gather (WSASend).
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__buffers_native">Pointer to an array of <see cref="WSABuffer" /> structures.</param>
        /// <param name="__bufferCount_native">The number of buffers.</param>
        /// <param name="__bytesTransferred_native">Pointer to a variable that receives the number of bytes sent.</param>
        /// <param name="__socketFlags_native">The socket flags.</param>
        /// <param name="__overlapped_native">Pointer to an overlapped structure (can be <see langword="null" />).</param>
        /// <param name="__completionRoutine_native">A completion routine (can be <see langword="null" />).</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSASend", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _WSASend(nint __socketHandle_native, WSABuffer* __buffers_native, uint __bufferCount_native, uint* __bytesTransferred_native, SocketFlags __socketFlags_native, NativeOverlapped* __overlapped_native, nint __completionRoutine_native);

        /// <summary>
        ///     Sends data to a specified destination using Winsock scatter/gather (WSASendTo).
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__buffers_native">Pointer to an array of <see cref="WSABuffer" /> structures.</param>
        /// <param name="__bufferCount_native">The number of buffers.</param>
        /// <param name="__bytesTransferred_native">Pointer to a variable that receives the number of bytes sent.</param>
        /// <param name="__socketFlags_native">The socket flags.</param>
        /// <param name="__socketAddress_native">Pointer to the destination socket address.</param>
        /// <param name="__socketAddressSize_native">The size of the destination address.</param>
        /// <param name="__overlapped_native">Pointer to an overlapped structure (can be <see langword="null" />).</param>
        /// <param name="__completionRoutine_native">A completion routine (can be <see langword="null" />).</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSASendTo", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _WSASendTo(nint __socketHandle_native, WSABuffer* __buffers_native, uint __bufferCount_native, uint* __bytesTransferred_native, SocketFlags __socketFlags_native, byte* __socketAddress_native, int __socketAddressSize_native, NativeOverlapped* __overlapped_native, nint __completionRoutine_native);

        /// <summary>
        ///     Receives data using Winsock scatter/gather (WSARecv).
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__buffer_native">Pointer to an array of <see cref="WSABuffer" /> structures.</param>
        /// <param name="__bufferCount_native">The number of buffers.</param>
        /// <param name="__bytesTransferred_native">Pointer to a variable that receives the number of bytes received.</param>
        /// <param name="__socketFlags_native">Pointer to the socket flags; can be <see langword="null" />.</param>
        /// <param name="__overlapped_native">Pointer to an overlapped structure (can be <see langword="null" />).</param>
        /// <param name="__completionRoutine_native">A completion routine (can be <see langword="null" />).</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSARecv", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _WSARecv(nint __socketHandle_native, WSABuffer* __buffer_native, uint __bufferCount_native, uint* __bytesTransferred_native, SocketFlags* __socketFlags_native, NativeOverlapped* __overlapped_native, nint __completionRoutine_native);

        /// <summary>
        ///     Receives data from a source address using Winsock scatter/gather (WSARecvFrom).
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__buffers_native">Pointer to an array of <see cref="WSABuffer" /> structures.</param>
        /// <param name="__bufferCount_native">The number of buffers.</param>
        /// <param name="__bytesTransferred_native">Pointer to a variable that receives the number of bytes received.</param>
        /// <param name="__socketFlags_native">Pointer to the socket flags; can be <see langword="null" />.</param>
        /// <param name="__socketAddressPointer_native">Pointer to a buffer that receives the source address.</param>
        /// <param name="__socketAddressSizePointer_native">
        ///     Pointer to the size of the address buffer; on input holds the buffer
        ///     size, on output the actual address size.
        /// </param>
        /// <param name="__overlapped_native">Pointer to an overlapped structure (can be <see langword="null" />).</param>
        /// <param name="__completionRoutine_native">A completion routine (can be <see langword="null" />).</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "WSARecvFrom", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError _WSARecvFrom(nint __socketHandle_native, WSABuffer* __buffers_native, uint __bufferCount_native, uint* __bytesTransferred_native, SocketFlags* __socketFlags_native, void* __socketAddressPointer_native, void* __socketAddressSizePointer_native, NativeOverlapped* __overlapped_native, nint __completionRoutine_native);

        /// <summary>
        ///     Converts a time duration in microseconds to a <see cref="TimeValue" /> structure.
        /// </summary>
        /// <param name="microseconds">The duration in microseconds.</param>
        /// <param name="socketTime">The <see cref="TimeValue" /> structure to fill.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MicrosecondsToTimeValue(long microseconds, ref TimeValue socketTime)
        {
            const long microcnv = 1000000;
            long quotient = microseconds / microcnv;
            long remainder = microseconds - quotient * microcnv;
            socketTime.Seconds = (int)quotient;
            socketTime.Microseconds = (int)remainder;
        }

        /// <summary>
        ///     Builds a <see cref="NativeScopedArray{WSABuffer}" /> from an array of <see cref="NativeIoSlice" /> structures.
        /// </summary>
        /// <param name="buffer">A span that can be used for temporary storage (e.g., stackalloc).</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <returns>A <see cref="NativeScopedArray{WSABuffer}" /> that wraps the converted buffers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeScopedArray<WSABuffer> Build(Span<WSABuffer> buffer, NativeIoSlice* buffers, int bufferCount)
        {
            NativeScopedArray<WSABuffer> __buffers_native = new NativeScopedArray<WSABuffer>(buffer, bufferCount);
            Span<WSABuffer> span = __buffers_native.AsSpan();
            for (int i = 0; i < bufferCount; ++i)
                span[i] = new WSABuffer((nuint)buffers[i].Length, buffers[i].Buffer);
            return __buffers_native;
        }

        /// <summary>
        ///     A dummy structure used for WSAStartup data.
        ///     The actual content is not required for this implementation.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 408)]
        public struct WSAData
        {
            /// <summary>
            ///     Alignment padding to ensure the structure is properly aligned in memory.
            /// </summary>
            private nint __ss_align;
        }

        /// <summary>
        ///     Represents a time value used with select and other functions.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TimeValue
        {
            /// <summary>
            ///     The number of seconds.
            /// </summary>
            public int Seconds;

            /// <summary>
            ///     The number of microseconds.
            /// </summary>
            public int Microseconds;
        }
    }
}