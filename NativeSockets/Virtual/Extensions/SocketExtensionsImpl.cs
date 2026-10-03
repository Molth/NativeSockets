using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Contains the function pointers of the selected socket extensions implementation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SocketExtensionsImpl
    {
        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<Socket, int, SelectMode, out bool, SocketError> Poll;

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<Socket, int, SelectModeFlags, out SelectModeFlags, SocketError> PollFlags;

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<Socket, ReadOnlySpan<byte>, SocketFlags, in NativeSocketAddress, out SocketError, int> SendTo;

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<Socket, Span<byte>, SocketFlags, out NativeSocketAddress, out SocketError, int> ReceiveFrom;

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<Socket, ReadOnlySpan<NativeIoSlice>, SocketFlags, out SocketError, int> SendVectored;

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<Socket, ReadOnlySpan<NativeIoSlice>, SocketFlags, in NativeSocketAddress, out SocketError, int> SendToVectored;

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 If the data was truncated, returns <see cref="IoResult.Err" /> with
        ///                 <see cref="SocketError.MessageSize" />,
        ///                 even if the underlying operation succeeded.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public delegate* managed<Socket, Span<NativeIoSlice>, SocketFlags, out SocketError, int> ReceiveVectored;

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 If the data was truncated, returns <see cref="IoResult.Err" /> with
        ///                 <see cref="SocketError.MessageSize" />,
        ///                 even if the underlying operation succeeded.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public delegate* managed<Socket, Span<NativeIoSlice>, SocketFlags, out NativeSocketAddress, out SocketError, int> ReceiveFromVectored;
    }
}