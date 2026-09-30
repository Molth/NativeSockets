using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides socket extension methods for the <see cref="VirtualSocket" /> structure.
    /// </summary>
    public static class VirtualSocketExtensions
    {
        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDualMode(this VirtualSocket socket, bool dualMode) => VirtualSocketPal.SetDualMode(socket, dualMode);

        /// <summary>
        ///     Sets whether the socket allows its local socket address to be reused.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="reuseAddress">true to allow the local socket address to be reused; false to disallow.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReuseAddress(this VirtualSocket socket, bool reuseAddress) => VirtualSocketPal.SetReuseAddress(socket, reuseAddress);

        /// <summary>
        ///     Sets whether the socket should not fragment packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontFragment">true to not fragment packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDontFragment(this VirtualSocket socket, bool dontFragment) => VirtualSocketPal.SetDontFragment(socket, dontFragment);

        /// <summary>
        ///     Sets whether the socket should not route packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontRoute">true to not route packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDontRoute(this VirtualSocket socket, bool dontRoute) => VirtualSocketPal.SetDontRoute(socket, dontRoute);

        /// <summary>
        ///     Sets whether the socket can send broadcast packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="enableBroadcast">true to enable broadcasting; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetEnableBroadcast(this VirtualSocket socket, bool enableBroadcast) => VirtualSocketPal.SetEnableBroadcast(socket, enableBroadcast);

        /// <summary>
        ///     Sets the time-to-live value for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="ttl">The time-to-live value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetTtl(this VirtualSocket socket, int ttl) => VirtualSocketPal.SetTtl(socket, ttl);

        /// <summary>
        ///     Sets the size of the send buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="sendBufferSize">The size of the send buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetSendBufferSize(this VirtualSocket socket, int sendBufferSize) => VirtualSocketPal.SetSendBufferSize(socket, sendBufferSize);

        /// <summary>
        ///     Sets the size of the receive buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="receiveBufferSize">The size of the receive buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReceiveBufferSize(this VirtualSocket socket, int receiveBufferSize) => VirtualSocketPal.SetReceiveBufferSize(socket, receiveBufferSize);

        /// <summary>
        ///     Sets the send timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The send timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetSendTimeout(this VirtualSocket socket, int milliseconds) => VirtualSocketPal.SetSendTimeout(socket, milliseconds);

        /// <summary>
        ///     Sets the receive timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The receive timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReceiveTimeout(this VirtualSocket socket, int milliseconds) => VirtualSocketPal.SetReceiveTimeout(socket, milliseconds);

        /// <summary>
        ///     Binds a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to bind to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind(this VirtualSocket socket, NativeSocketAddress socketAddress) => VirtualSocketPal.Bind(socket, socketAddress);

        /// <summary>
        ///     Connects a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to connect to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(this VirtualSocket socket, NativeSocketAddress socketAddress) => VirtualSocketPal.Connect(socket, socketAddress);

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(this VirtualSocket socket, bool blocking) => VirtualSocketPal.SetBlocking(socket, blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(this VirtualSocket socket, int microseconds, SelectMode mode, out bool status) => VirtualSocketPal.Poll(socket, microseconds, mode, out status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///     caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///     the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(this VirtualSocket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags) => VirtualSocketPal.PollFlags(socket, microseconds, inFlags, out outFlags);

        /// <summary>
        ///     Gets the local name (socket address) of a socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to receive the local name into.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 This failure only occurs on .NET 8 and later, because the <c>SendTo</c> overload
        ///                 <c>SendTo(ReadOnlySpan&lt;byte&gt;, SocketFlags, SocketAddress)</c> added in .NET 8
        ///                 does not set the underlying <c>_rightEndPoint</c> field. After a <c>SendTo</c> that
        ///                 triggers an implicit bind, the socket is actually bound, but <c>LocalEndPoint</c>
        ///                 cannot be queried and throws. In that state this method reports an error even though
        ///                 the datagram was delivered successfully.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(this VirtualSocket socket, ref NativeSocketAddress socketAddress) => VirtualSocketPal.GetName(socket, ref socketAddress);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Send(this VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags) => VirtualSocketPal.Send(socket, buffer, socketFlags);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
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
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Receive(this VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags) => VirtualSocketPal.Receive(socket, buffer, socketFlags);

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendTo(this VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => VirtualSocketPal.SendTo(socket, buffer, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
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
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFrom(this VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => VirtualSocketPal.ReceiveFrom(socket, buffer, socketFlags, ref socketAddress);

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendVectored(this VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags) => VirtualSocketPal.SendVectored(socket, buffers, socketFlags);

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendToVectored(this VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => VirtualSocketPal.SendToVectored(socket, buffers, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
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
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveVectored(this VirtualSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags) => VirtualSocketPal.ReceiveVectored(socket, buffers, socketFlags);

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
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
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />, exceptions are
        ///                 caught and reported as the <see cref="SocketError" /> instead of being thrown; always check
        ///                 the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFromVectored(this VirtualSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => VirtualSocketPal.ReceiveFromVectored(socket, buffers, socketFlags, ref socketAddress);
    }
}