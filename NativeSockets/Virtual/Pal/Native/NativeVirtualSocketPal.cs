using System;
using System.Net.Sockets;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides native virtual socket operations backed by <see cref="NativeSocket" />.
    /// </summary>
    internal static unsafe class NativeVirtualSocketPal
    {
        /// <summary>
        ///     Gets a value that indicates whether a virtual socket has been created.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <returns>true if the virtual socket has been created; otherwise false.</returns>
        public static bool IsCreated(VirtualSocket socket) => socket.Handle != -1;

        /// <summary>
        ///     Creates a virtual socket for the specified address family (Ipv4 or Ipv6).
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <param name="socket">When this method returns, contains the created virtual socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError Create(bool ipv6, out VirtualSocket socket)
        {
            AddressFamily addressFamily = ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;

            SocketError error = NativeSocketPal.Startup();
            if (error != SocketError.Success)
            {
                socket = new VirtualSocket(-1, addressFamily);
                return error;
            }

            error = NativeSocket.Create(ipv6, out NativeSocket s);
            if (error != SocketError.Success)
            {
                NativeSocketPal.Cleanup();

                socket = new VirtualSocket(-1, addressFamily);
                return error;
            }

            socket = new VirtualSocket(s.Handle, addressFamily);
            return error;
        }

        /// <summary>
        ///     Closes a virtual socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError Close(VirtualSocket socket)
        {
            SocketError error = NativeSocket.Close(socket.AsNativeSocket());
            if (error != SocketError.Success)
            {
                if (error != SocketError.NotSocket)
                    NativeSocketPal.Cleanup();

                return error;
            }

            return NativeSocketPal.Cleanup();
        }

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetDualMode(VirtualSocket socket, bool dualMode) => socket.AsNativeSocket().SetDualMode(dualMode);

        /// <summary>
        ///     Sets whether the socket allows its local socket address to be reused.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="reuseAddress">true to allow the local socket address to be reused; false to disallow.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetReuseAddress(VirtualSocket socket, bool reuseAddress) => socket.AsNativeSocket().SetReuseAddress(reuseAddress);

        /// <summary>
        ///     Sets whether the socket should not fragment packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontFragment">true to not fragment packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetDontFragment(VirtualSocket socket, bool dontFragment) => socket.AsNativeSocket().SetDontFragment(dontFragment);

        /// <summary>
        ///     Sets whether the socket should not route packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontRoute">true to not route packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetDontRoute(VirtualSocket socket, bool dontRoute) => socket.AsNativeSocket().SetDontRoute(dontRoute);

        /// <summary>
        ///     Sets whether the socket can send broadcast packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="enableBroadcast">true to enable broadcasting; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetEnableBroadcast(VirtualSocket socket, bool enableBroadcast) => socket.AsNativeSocket().SetEnableBroadcast(enableBroadcast);

        /// <summary>
        ///     Sets the time-to-live value for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="ttl">The time-to-live value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetTtl(VirtualSocket socket, int ttl) => socket.AsNativeSocket().SetTtl(ttl);

        /// <summary>
        ///     Sets the size of the send buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="sendBufferSize">The size of the send buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetSendBufferSize(VirtualSocket socket, int sendBufferSize) => socket.AsNativeSocket().SetSendBufferSize(sendBufferSize);

        /// <summary>
        ///     Sets the size of the receive buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="receiveBufferSize">The size of the receive buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetReceiveBufferSize(VirtualSocket socket, int receiveBufferSize) => socket.AsNativeSocket().SetReceiveBufferSize(receiveBufferSize);

        /// <summary>
        ///     Sets the send timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The send timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetSendTimeout(VirtualSocket socket, int milliseconds) => socket.AsNativeSocket().SetSendTimeout(milliseconds);

        /// <summary>
        ///     Sets the receive timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The receive timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetReceiveTimeout(VirtualSocket socket, int milliseconds) => socket.AsNativeSocket().SetReceiveTimeout(milliseconds);

        /// <summary>
        ///     Binds a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to bind to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError Bind(VirtualSocket socket, NativeSocketAddress socketAddress) => socket.AsNativeSocket().Bind(socketAddress);

        /// <summary>
        ///     Connects a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to connect to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError Connect(VirtualSocket socket, NativeSocketAddress socketAddress) => socket.AsNativeSocket().Connect(socketAddress);

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError SetBlocking(VirtualSocket socket, bool blocking) => socket.AsNativeSocket().SetBlocking(blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError Poll(VirtualSocket socket, int microseconds, SelectMode mode, out bool status) => socket.AsNativeSocket().Poll(microseconds, mode, out status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError PollFlags(VirtualSocket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags) => socket.AsNativeSocket().PollFlags(microseconds, inFlags, out outFlags);

        /// <summary>
        ///     Gets the local name (socket address) of a socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to receive the local name into.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError GetName(VirtualSocket socket, ref NativeSocketAddress socketAddress) => socket.AsNativeSocket().GetName(ref socketAddress);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult Send(VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags) => socket.AsNativeSocket().Send(buffer, socketFlags);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult Receive(VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags) => socket.AsNativeSocket().Receive(buffer, socketFlags);

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult SendTo(VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => socket.AsNativeSocket().SendTo(buffer, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult ReceiveFrom(VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => socket.AsNativeSocket().ReceiveFrom(buffer, socketFlags, ref socketAddress);

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult SendVectored(VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags) => socket.AsNativeSocket().SendVectored(buffers, socketFlags);

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult SendToVectored(VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => socket.AsNativeSocket().SendToVectored(buffers, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
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
        public static IoResult ReceiveVectored(VirtualSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags) => socket.AsNativeSocket().ReceiveVectored(buffers, socketFlags);

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
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
        public static IoResult ReceiveFromVectored(VirtualSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => socket.AsNativeSocket().ReceiveFromVectored(buffers, socketFlags, ref socketAddress);

        /// <summary>
        ///     Gets the virtual socket pal implementation for this class.
        /// </summary>
        /// <returns>The <see cref="VirtualSocketPalImpl" /> for the native implementation.</returns>
        public static VirtualSocketPalImpl GetImpl()
        {
            VirtualSocketPalImpl impl;
            impl.IsCreated = &IsCreated;
            impl.Create = &Create;
            impl.Close = &Close;
            impl.SetDualMode = &SetDualMode;
            impl.SetReuseAddress = &SetReuseAddress;
            impl.SetDontFragment = &SetDontFragment;
            impl.SetDontRoute = &SetDontRoute;
            impl.SetEnableBroadcast = &SetEnableBroadcast;
            impl.SetTtl = &SetTtl;
            impl.SetSendBufferSize = &SetSendBufferSize;
            impl.SetReceiveBufferSize = &SetReceiveBufferSize;
            impl.SetSendTimeout = &SetSendTimeout;
            impl.SetReceiveTimeout = &SetReceiveTimeout;
            impl.Bind = &Bind;
            impl.Connect = &Connect;
            impl.SetBlocking = &SetBlocking;
            impl.Poll = &Poll;
            impl.PollFlags = &PollFlags;
            impl.GetName = &GetName;
            impl.Send = &Send;
            impl.Receive = &Receive;
            impl.SendTo = &SendTo;
            impl.ReceiveFrom = &ReceiveFrom;
            impl.SendVectored = &SendVectored;
            impl.SendToVectored = &SendToVectored;
            impl.ReceiveVectored = &ReceiveVectored;
            impl.ReceiveFromVectored = &ReceiveFromVectored;
            return impl;
        }
    }
}