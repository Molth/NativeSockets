using System.Net.Sockets;
using System.Security;
using static NativeSockets.UnixNativeLib2;
using static NativeSockets.FreeBsdSocketOption;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides Unix-specific socket operations using libc functions.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class FreeBsdNativeLib
    {
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
    }
}