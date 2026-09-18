using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NativeSockets.UnixNativeLibName;

#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time.

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides Unix-specific socket operations using libc functions.
    /// </summary>
    internal static unsafe class UnixSocketLib
    {
        /// <summary>
        ///     Binds a socket to a local address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle (file descriptor).</param>
        /// <param name="__socketAddress_native">Pointer to the socket address structure.</param>
        /// <param name="__socketAddressSize_native">The size of the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "bind", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
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
        [DllImport(DLL_NAME_LIBC, EntryPoint = "getsockname", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _getsockname(int __socketHandle_native, sockaddr* __socketAddress_native, uint* __socketAddressSize_native);

        /// <summary>
        ///     Creates a new socket.
        /// </summary>
        /// <param name="af">The address family (e.g., AF_INET, AF_INET6).</param>
        /// <param name="type">The socket type (e.g., SOCK_STREAM, SOCK_DGRAM).</param>
        /// <param name="protocol">The protocol (e.g., IPPROTO_TCP, IPPROTO_UDP).</param>
        /// <returns>The socket file descriptor on success; otherwise -1.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "socket", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _socket(int af, int type, int protocol);

        /// <summary>
        ///     Performs device I/O control operations (ioctl) on a file descriptor.
        /// </summary>
        /// <param name="fd">The socket file descriptor.</param>
        /// <param name="request">The ioctl request code (e.g., FIONBIO).</param>
        /// <param name="arg">Pointer to the request argument; null when the request takes none.</param>
        /// <returns>0 on success; otherwise -1.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "ioctl", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _ioctl(int fd, nuint request, void* arg);

        /// <summary>
        ///     Connects a socket to a remote address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__socketAddress_native">Pointer to the socket address structure.</param>
        /// <param name="__socketAddressSize_native">The size of the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "connect", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _connect(int __socketHandle_native, sockaddr* __socketAddress_native, uint __socketAddressSize_native);

        /// <summary>
        ///     Closes a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "close", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _close(int __socketHandle_native);

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__optionLevel_native">The option level.</param>
        /// <param name="__optionName_native">The option name.</param>
        /// <param name="__optionValue_native">Pointer to the option value.</param>
        /// <param name="__optionLength_native">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "setsockopt", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _setsockopt(int __socketHandle_native, int __optionLevel_native, int __optionName_native, byte* __optionValue_native, uint __optionLength_native);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__optionLevel_native">The option level.</param>
        /// <param name="__optionName_native">The option name.</param>
        /// <param name="__optionValue_native">Pointer to a buffer that receives the option value.</param>
        /// <param name="__optionLength_native">Pointer to the size of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "getsockopt", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int _getsockopt(int __socketHandle_native, int __optionLevel_native, int __optionName_native, byte* __optionValue_native, uint* __optionLength_native);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer containing the data to send.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the send operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "send", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint _send(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer where received data will be stored.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "recv", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint _recv(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native);

        /// <summary>
        ///     Sends data to a specified destination address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer containing the data to send.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the send operation.</param>
        /// <param name="__socketAddress_native">Pointer to the destination socket address.</param>
        /// <param name="__socketAddressSize_native">Size of the destination address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "sendto", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint _sendto(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native, sockaddr* __socketAddress_native, uint __socketAddressSize_native);

        /// <summary>
        ///     Receives data from a socket and captures the source address.
        /// </summary>
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
        [DllImport(DLL_NAME_LIBC, EntryPoint = "recvfrom", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint _recvfrom(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native, sockaddr* __socketAddress_native, uint* __socketAddressSize_native);

        /// <summary>
        ///     Sends data from multiple buffers using a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The socket file descriptor.</param>
        /// <param name="__msg_native">Pointer to a msghdr structure describing the message.</param>
        /// <param name="__socketFlags_native">Flags for the send operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "sendmsg", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint _sendmsg(int __socketHandle_native, void* __msg_native, int __socketFlags_native);

        /// <summary>
        ///     Receives data into multiple buffers from a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The socket file descriptor.</param>
        /// <param name="__msg_native">Pointer to a msghdr structure that will receive the message.</param>
        /// <param name="__socketFlags_native">Flags for the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "recvmsg", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint _recvmsg(int __socketHandle_native, void* __msg_native, int __socketFlags_native);

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