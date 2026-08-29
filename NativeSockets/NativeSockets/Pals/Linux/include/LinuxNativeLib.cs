using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using static NativeSockets.UnixNativeLibName;
using static NativeSockets.UnixNativeLib2;
using static NativeSockets.LinuxSocketOption;
using static NativeSockets.LinuxSocketFlags;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable SYSLIB1054

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides Unix-specific socket operations using libc functions.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class LinuxNativeLib
    {
        /// <summary>
        ///     Flag value for <c>fcntl</c> to set non‑blocking I/O mode on a socket.
        /// </summary>
        public const int O_NONBLOCK = 2048;

        /// <summary>
        ///     Converts an integer value to the <c>msg_iovlen</c> field of a <see cref="msghdr" /> structure.
        /// </summary>
        /// <param name="value">The integer value representing the number of I/O vectors.</param>
        /// <returns>The value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint _get_msg_iovlen(int value) => (nuint)value;

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="optionLevel">The option level.</param>
        /// <param name="optionName">The option name.</param>
        /// <param name="__optionValue_native">Pointer to the option value.</param>
        /// <param name="__optionLength_native">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static int _setsockopt(int __socketHandle_native, SocketOptionLevel optionLevel, SocketOptionName optionName, byte* __optionValue_native, uint __optionLength_native) => __setsockopt(__socketHandle_native, ToNativeSocketOptionLevel(optionLevel), ToNativeSocketOptionName(optionLevel, optionName), __optionValue_native, __optionLength_native);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="optionLevel">The option level.</param>
        /// <param name="optionName">The option name.</param>
        /// <param name="__optionValue_native">Pointer to a buffer that receives the option value.</param>
        /// <param name="__optionLength_native">Pointer to the size of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static int _getsockopt(int __socketHandle_native, SocketOptionLevel optionLevel, SocketOptionName optionName, byte* __optionValue_native, uint* __optionLength_native) => __getsockopt(__socketHandle_native, ToNativeSocketOptionLevel(optionLevel), ToNativeSocketOptionName(optionLevel, optionName), __optionValue_native, __optionLength_native);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer containing the data to send.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="socketFlags">The socket flags for the send operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        public static nint _send(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, SocketFlags socketFlags) => __send(__socketHandle_native, __pinnedBuffer_native, __len_native, ToNativeSocketFlags(socketFlags));

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer where received data will be stored.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="socketFlags">The socket flags for the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        public static nint _recv(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, SocketFlags socketFlags) => __recv(__socketHandle_native, __pinnedBuffer_native, __len_native, ToNativeSocketFlags(socketFlags));

        /// <summary>
        ///     Sends data to a specified destination address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer containing the data to send.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="socketFlags">The socket flags for the send operation.</param>
        /// <param name="__socketAddress_native">Pointer to the destination socket address.</param>
        /// <param name="__socketAddressSize_native">Size of the destination address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        public static nint _sendto(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, SocketFlags socketFlags, sockaddr* __socketAddress_native, uint __socketAddressSize_native) => __sendto(__socketHandle_native, __pinnedBuffer_native, __len_native, ToNativeSocketFlags(socketFlags), __socketAddress_native, __socketAddressSize_native);

        /// <summary>
        ///     Receives data from a socket and captures the source address.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer where received data will be stored.</param>
        /// <param name="__len_native">The maximum length of the buffer in bytes.</param>
        /// <param name="socketFlags">The socket flags for the receive operation.</param>
        /// <param name="__socketAddress_native">Pointer to a buffer that receives the source address.</param>
        /// <param name="__socketAddressSize_native">
        ///     Pointer to the size of the address buffer; on input holds the buffer size, on
        ///     output the actual address size.
        /// </param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        public static nint _recvfrom(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, SocketFlags socketFlags, sockaddr* __socketAddress_native, uint* __socketAddressSize_native) => __recvfrom(__socketHandle_native, __pinnedBuffer_native, __len_native, ToNativeSocketFlags(socketFlags), __socketAddress_native, __socketAddressSize_native);

        /// <summary>
        ///     Sends a message using a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The socket file descriptor.</param>
        /// <param name="__msg_native">Pointer to a <see cref="msghdr" /> structure describing the message.</param>
        /// <param name="socketFlags">Flags for the send operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        public static nint _sendmsg(int __socketHandle_native, msghdr* __msg_native, SocketFlags socketFlags) => __sendmsg(__socketHandle_native, __msg_native, ToNativeSocketFlags(socketFlags));

        /// <summary>
        ///     Receives a message from a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The socket file descriptor.</param>
        /// <param name="__msg_native">Pointer to a <see cref="msghdr" /> structure that will receive the message.</param>
        /// <param name="socketFlags">Flags for the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        public static nint _recvmsg(int __socketHandle_native, msghdr* __msg_native, SocketFlags socketFlags)
        {
            nint result = __recvmsg(__socketHandle_native, __msg_native, ToNativeSocketFlags(socketFlags));

            if (__msg_native != null)
                __msg_native->msg_flags = (int)FromNativeSocketFlags(__msg_native->msg_flags);

            return result;
        }

        /// <summary>
        ///     Sends a message using a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The socket file descriptor.</param>
        /// <param name="__msg_native">Pointer to a <see cref="msghdr" /> structure describing the message.</param>
        /// <param name="__socketFlags_native">Flags for the send operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "sendmsg", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        private static extern nint __sendmsg(int __socketHandle_native, msghdr* __msg_native, int __socketFlags_native);

        /// <summary>
        ///     Receives a message from a socket.
        /// </summary>
        /// <param name="__socketHandle_native">The socket file descriptor.</param>
        /// <param name="__msg_native">Pointer to a <see cref="msghdr" /> structure that will receive the message.</param>
        /// <param name="__socketFlags_native">Flags for the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "recvmsg", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        private static extern nint __recvmsg(int __socketHandle_native, msghdr* __msg_native, int __socketFlags_native);

        /// <summary>
        ///     Represents a message header used with <c>sendmsg</c>
        ///     and <c>recvmsg</c> on Unix-like systems.
        /// </summary>
        public struct msghdr
        {
            /// <summary>
            ///     Optional address of the destination or source socket.
            /// </summary>
            public void* msg_name;

            /// <summary>
            ///     Length of the address pointed to by <see cref="msg_name" />.
            /// </summary>
            public uint msg_namelen;

            /// <summary>
            ///     Pointer to an array of <see cref="iovec" /> structures describing the scatter/gather buffers.
            /// </summary>
            public iovec* msg_iov;

            /// <summary>
            ///     Number of elements in the <see cref="msg_iov" /> array.
            /// </summary>
            public nuint msg_iovlen;

            /// <summary>
            ///     Pointer to ancillary data (control messages).
            /// </summary>
            public void* msg_control;

            /// <summary>
            ///     Length of the ancillary data buffer.
            /// </summary>
            public nuint msg_controllen;

            /// <summary>
            ///     Flags received or set for the message.
            /// </summary>
            public int msg_flags;
        }
    }
}