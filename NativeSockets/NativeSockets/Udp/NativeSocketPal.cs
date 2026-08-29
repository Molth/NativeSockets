using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations for sending and receiving data.
    /// </summary>
    /// <remarks>
    ///     This class uses function pointers to delegate to the appropriate platform-specific implementation
    ///     (Windows, Linux, Android, macOS) at runtime.
    /// </remarks>
    public static class NativeSocketPal
    {
        /// <summary>
        ///     Gets a value indicating whether any platform-specific implementation is supported.
        /// </summary>
        public static bool IsSupported => SocketPal.IsSupported;

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError() => SocketPal.GetLastSocketError();

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Startup() => SocketPal.Startup();

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup() => SocketPal.Cleanup();

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <param name="result">The native socket handle, or -1 on error.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SocketError Create(bool ipv6, out NativeSocket result)
        {
            result = new NativeSocket(SocketPal.Create(ipv6), ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork);
            return result.Handle != -1 ? SocketError.Success : SocketError.SocketError;
        }

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SocketError Close(NativeSocket socket) => SocketPal.Close(socket);
    }
}