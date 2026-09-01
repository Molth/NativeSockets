using System.Net.Sockets;
using System.Runtime.InteropServices;
using static NativeSockets.UnixNativeLibName;

#pragma warning disable SYSLIB1054

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides Unix-specific socket operations using libc functions.
    /// </summary>
    internal static unsafe class UnixNativeLib2
    {
        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__optionLevel_native">The option level.</param>
        /// <param name="__optionName_native">The option name.</param>
        /// <param name="__optionValue_native">Pointer to the option value.</param>
        /// <param name="__optionLength_native">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "setsockopt", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int __setsockopt(int __socketHandle_native, int __optionLevel_native, int __optionName_native, byte* __optionValue_native, uint __optionLength_native);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__optionLevel_native">The option level.</param>
        /// <param name="__optionName_native">The option name.</param>
        /// <param name="__optionValue_native">Pointer to a buffer that receives the option value.</param>
        /// <param name="__optionLength_native">Pointer to the size of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "getsockopt", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern int __getsockopt(int __socketHandle_native, int __optionLevel_native, int __optionName_native, byte* __optionValue_native, uint* __optionLength_native);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer containing the data to send.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the send operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "send", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint __send(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="__socketHandle_native">The native socket handle.</param>
        /// <param name="__pinnedBuffer_native">Pointer to the buffer where received data will be stored.</param>
        /// <param name="__len_native">The length of the buffer in bytes.</param>
        /// <param name="__socketFlags_native">The socket flags for the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "recv", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint __recv(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native);

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
        [DllImport(NATIVE_LIBRARY, EntryPoint = "sendto", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint __sendto(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native, sockaddr* __socketAddress_native, uint __socketAddressSize_native);

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
        [DllImport(NATIVE_LIBRARY, EntryPoint = "recvfrom", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        public static extern nint __recvfrom(int __socketHandle_native, byte* __pinnedBuffer_native, nuint __len_native, int __socketFlags_native, sockaddr* __socketAddress_native, uint* __socketAddressSize_native);
    }
}