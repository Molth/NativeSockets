using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides the virtual socket operations as a single struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VirtualSocketPalImpl
    {
        /// <summary>
        ///     Gets a value that indicates whether a virtual socket has been created.
        /// </summary>
        /// <returns>true if the virtual socket has been created; otherwise false.</returns>
        public delegate* managed<VirtualSocket, bool> IsCreated;

        /// <summary>
        ///     Creates a virtual socket for the specified address family (Ipv4 or Ipv6).
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<bool, out VirtualSocket, SocketError> Create;

        /// <summary>
        ///     Closes a virtual socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, SocketError> Close;

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, bool, SocketError> SetDualMode;

        /// <summary>
        ///     Sets whether the socket allows its local socket address to be reused.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, bool, SocketError> SetReuseAddress;

        /// <summary>
        ///     Sets whether the socket should not fragment packets.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, bool, SocketError> SetDontFragment;

        /// <summary>
        ///     Sets whether the socket should not route packets.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, bool, SocketError> SetDontRoute;

        /// <summary>
        ///     Sets whether the socket can send broadcast packets.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, bool, SocketError> SetEnableBroadcast;

        /// <summary>
        ///     Sets the time-to-live value for the socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, int, SocketError> SetTtl;

        /// <summary>
        ///     Sets the size of the send buffer for the socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, int, SocketError> SetSendBufferSize;

        /// <summary>
        ///     Sets the size of the receive buffer for the socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, int, SocketError> SetReceiveBufferSize;

        /// <summary>
        ///     Sets the send timeout for the socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, int, SocketError> SetSendTimeout;

        /// <summary>
        ///     Sets the receive timeout for the socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, int, SocketError> SetReceiveTimeout;

        /// <summary>
        ///     Binds a socket to a socket address.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, NativeSocketAddress, SocketError> Bind;

        /// <summary>
        ///     Connects a socket to a socket address.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, NativeSocketAddress, SocketError> Connect;

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, bool, SocketError> SetBlocking;

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, int, SelectMode, out bool, SocketError> Poll;

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<VirtualSocket, int, SelectModeFlags, out SelectModeFlags, SocketError> PollFlags;

        /// <summary>
        ///     Gets the local name (socket address) of a socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     This failure only occurs on .NET 8 and later, because the <c>SendTo</c> overload
        ///     <c>SendTo(ReadOnlySpan&lt;byte&gt;, SocketFlags, SocketAddress)</c> added in .NET 8
        ///     does not set the underlying <c>_rightEndPoint</c> field. After a <c>SendTo</c> that
        ///     triggers an implicit bind, the socket is actually bound, but <c>LocalEndPoint</c>
        ///     cannot be queried and throws. In that state this method reports an error even though
        ///     the datagram was delivered successfully.
        /// </remarks>
        public delegate* managed<VirtualSocket, ref NativeSocketAddress, SocketError> GetName;

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<VirtualSocket, ReadOnlySpan<byte>, SocketFlags, IoResult> Send;

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<VirtualSocket, Span<byte>, SocketFlags, IoResult> Receive;

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<VirtualSocket, ReadOnlySpan<byte>, SocketFlags, in NativeSocketAddress, IoResult> SendTo;

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<VirtualSocket, Span<byte>, SocketFlags, ref NativeSocketAddress, IoResult> ReceiveFrom;

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<VirtualSocket, ReadOnlySpan<NativeIoSlice>, SocketFlags, IoResult> SendVectored;

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public delegate* managed<VirtualSocket, ReadOnlySpan<NativeIoSlice>, SocketFlags, in NativeSocketAddress, IoResult> SendToVectored;

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
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
        public delegate* managed<VirtualSocket, Span<NativeIoSlice>, SocketFlags, IoResult> ReceiveVectored;

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
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
        public delegate* managed<VirtualSocket, Span<NativeIoSlice>, SocketFlags, ref NativeSocketAddress, IoResult> ReceiveFromVectored;
    }
}