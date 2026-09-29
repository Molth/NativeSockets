using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted virtual socket operations.
    /// </summary>
    internal static unsafe class VirtualSocketPal
    {
        /// <summary>
        ///     The virtual socket pal implementation selected during initialization.
        /// </summary>
        private static readonly VirtualSocketPalImpl Impl;

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        static VirtualSocketPal() => Impl = NativeSocketPal.IsSupported ? NativeVirtualSocketPal.GetImpl() : ManagedVirtualSocketPal.GetImpl();

        /// <summary>
        ///     Gets a value that indicates whether a virtual socket has been created.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <returns>true if the virtual socket has been created; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCreated(VirtualSocket socket) => Impl.IsCreated(socket);

        /// <summary>
        ///     Creates a virtual socket for the specified address family (Ipv4 or Ipv6).
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <param name="socket">When this method returns, contains the created virtual socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Create(bool ipv6, out VirtualSocket socket) => Impl.Create(ipv6, out socket);

        /// <summary>
        ///     Closes a virtual socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(VirtualSocket socket) => Impl.Close(socket);

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDualMode(VirtualSocket socket, bool dualMode) => Impl.SetDualMode(socket, dualMode);

        /// <summary>
        ///     Sets whether the socket allows its local socket address to be reused.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="reuseAddress">true to allow the local socket address to be reused; false to disallow.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReuseAddress(VirtualSocket socket, bool reuseAddress) => Impl.SetReuseAddress(socket, reuseAddress);

        /// <summary>
        ///     Sets whether the socket should not fragment packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontFragment">true to not fragment packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDontFragment(VirtualSocket socket, bool dontFragment) => Impl.SetDontFragment(socket, dontFragment);

        /// <summary>
        ///     Sets whether the socket should not route packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontRoute">true to not route packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDontRoute(VirtualSocket socket, bool dontRoute) => Impl.SetDontRoute(socket, dontRoute);

        /// <summary>
        ///     Sets whether the socket can send broadcast packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="enableBroadcast">true to enable broadcasting; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetEnableBroadcast(VirtualSocket socket, bool enableBroadcast) => Impl.SetEnableBroadcast(socket, enableBroadcast);

        /// <summary>
        ///     Sets the time-to-live value for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="ttl">The time-to-live value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetTtl(VirtualSocket socket, int ttl) => Impl.SetTtl(socket, ttl);

        /// <summary>
        ///     Sets the size of the send buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="sendBufferSize">The size of the send buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetSendBufferSize(VirtualSocket socket, int sendBufferSize) => Impl.SetSendBufferSize(socket, sendBufferSize);

        /// <summary>
        ///     Sets the size of the receive buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="receiveBufferSize">The size of the receive buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReceiveBufferSize(VirtualSocket socket, int receiveBufferSize) => Impl.SetReceiveBufferSize(socket, receiveBufferSize);

        /// <summary>
        ///     Sets the send timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The send timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetSendTimeout(VirtualSocket socket, int milliseconds) => Impl.SetSendTimeout(socket, milliseconds);

        /// <summary>
        ///     Sets the receive timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The receive timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReceiveTimeout(VirtualSocket socket, int milliseconds) => Impl.SetReceiveTimeout(socket, milliseconds);

        /// <summary>
        ///     Binds a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to bind to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind(VirtualSocket socket, NativeSocketAddress socketAddress) => Impl.Bind(socket, socketAddress);

        /// <summary>
        ///     Connects a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to connect to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(VirtualSocket socket, NativeSocketAddress socketAddress) => Impl.Connect(socket, socketAddress);

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(VirtualSocket socket, bool blocking) => Impl.SetBlocking(socket, blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(VirtualSocket socket, int microseconds, SelectMode mode, out bool status) => Impl.Poll(socket, microseconds, mode, out status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(VirtualSocket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags) => Impl.PollFlags(socket, microseconds, inFlags, out outFlags);

        /// <summary>
        ///     Gets the local name (socket address) of a socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to receive the local name into.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     This failure only occurs on .NET 8 and later, because the <c>SendTo</c> overload
        ///     <c>SendTo(ReadOnlySpan&lt;byte&gt;, SocketFlags, SocketAddress)</c> added in .NET 8
        ///     does not set the underlying <c>_rightEndPoint</c> field. After a <c>SendTo</c> that
        ///     triggers an implicit bind, the socket is actually bound, but <c>LocalEndPoint</c>
        ///     cannot be queried and throws. In that state this method reports an error even though
        ///     the datagram was delivered successfully.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(VirtualSocket socket, ref NativeSocketAddress socketAddress) => Impl.GetName(socket, ref socketAddress);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Send(VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags) => Impl.Send(socket, buffer, socketFlags);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Receive(VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags) => Impl.Receive(socket, buffer, socketFlags);

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendTo(VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => Impl.SendTo(socket, buffer, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFrom(VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => Impl.ReceiveFrom(socket, buffer, socketFlags, ref socketAddress);

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendVectored(VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags) => Impl.SendVectored(socket, buffers, socketFlags);

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendToVectored(VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => Impl.SendToVectored(socket, buffers, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveVectored(VirtualSocket socket, Span<NativeIoSlice> buffers, ref SocketFlags inOutFlags) => Impl.ReceiveVectored(socket, buffers, ref inOutFlags);

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFromVectored(VirtualSocket socket, Span<NativeIoSlice> buffers, ref SocketFlags inOutFlags, ref NativeSocketAddress socketAddress) => Impl.ReceiveFromVectored(socket, buffers, ref inOutFlags, ref socketAddress);
    }
}