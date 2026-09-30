using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides managed virtual socket operations backed by <see cref="Socket" />.
    /// </summary>
    internal static unsafe class ManagedVirtualSocketPal
    {
        /// <summary>
        ///     Reusable buffer that holds the Boolean value written by <c>WSAIoctl</c> when applying
        ///     <see cref="WindowsNativeLibName.SIO_UDP_CONNRESET" />. A zero value disables the
        ///     connection‑reset notification behavior on Windows UDP sockets.
        /// </summary>
        private static readonly byte[] SIO_UDP_CONNRESET_NEW_BEHAVIOR_BUFFER = new byte[4];

        /// <summary>
        ///     Gets a value that indicates whether a virtual socket has been created.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <returns>true if the virtual socket has been created; otherwise false.</returns>
        public static bool IsCreated(VirtualSocket socket) => socket.TryGetSocket(out _) == SocketError.Success;

        /// <summary>
        ///     Creates a virtual socket for the specified address family (Ipv4 or Ipv6).
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <param name="socket">When this method returns, contains the created virtual socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError Create(bool ipv6, out VirtualSocket socket)
        {
            AddressFamily addressFamily = ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;

            try
            {
                Socket s = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);

                if (OsName.IsWindows())
                {
                    try
                    {
                        s.IOControl(unchecked((int)WindowsNativeLibName.SIO_UDP_CONNRESET), SIO_UDP_CONNRESET_NEW_BEHAVIOR_BUFFER, null);
                    }
                    catch
                    {
                    }
                }

                GCHandle gcHandle = GCHandle.Alloc(s);
                socket = new VirtualSocket(GCHandle.ToIntPtr(gcHandle), addressFamily);
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                socket = new VirtualSocket(0, addressFamily);
                return ex.SocketErrorCode;
            }
            catch
            {
                socket = new VirtualSocket(0, addressFamily);
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Closes a virtual socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError Close(VirtualSocket socket)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            error = ManagedVirtualSocketPalImpl.Close(s!);

            try
            {
                GCHandle.FromIntPtr(socket.Handle).Free();
            }
            catch
            {
                if (error == SocketError.Success)
                    error = SocketError.NotSocket;
            }

            return error;
        }

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetDualMode(VirtualSocket socket, bool dualMode)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetDualMode(s!, dualMode);
        }

        /// <summary>
        ///     Sets whether the socket allows its local socket address to be reused.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="reuseAddress">true to allow the local socket address to be reused; false to disallow.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetReuseAddress(VirtualSocket socket, bool reuseAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetReuseAddress(s!, reuseAddress);
        }

        /// <summary>
        ///     Sets whether the socket should not fragment packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontFragment">true to not fragment packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetDontFragment(VirtualSocket socket, bool dontFragment)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetDontFragment(s!, dontFragment);
        }

        /// <summary>
        ///     Sets whether the socket should not route packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="dontRoute">true to not route packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetDontRoute(VirtualSocket socket, bool dontRoute)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetDontRoute(s!, dontRoute);
        }

        /// <summary>
        ///     Sets whether the socket can send broadcast packets.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="enableBroadcast">true to enable broadcasting; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetEnableBroadcast(VirtualSocket socket, bool enableBroadcast)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetEnableBroadcast(s!, enableBroadcast);
        }

        /// <summary>
        ///     Sets the time-to-live value for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="ttl">The time-to-live value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetTtl(VirtualSocket socket, int ttl)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetTtl(s!, ttl);
        }

        /// <summary>
        ///     Sets the size of the send buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="sendBufferSize">The size of the send buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetSendBufferSize(VirtualSocket socket, int sendBufferSize)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetSendBufferSize(s!, sendBufferSize);
        }

        /// <summary>
        ///     Sets the size of the receive buffer for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="receiveBufferSize">The size of the receive buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetReceiveBufferSize(VirtualSocket socket, int receiveBufferSize)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetReceiveBufferSize(s!, receiveBufferSize);
        }

        /// <summary>
        ///     Sets the send timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The send timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetSendTimeout(VirtualSocket socket, int milliseconds)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetSendTimeout(s!, milliseconds);
        }

        /// <summary>
        ///     Sets the receive timeout for the socket.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="milliseconds">The receive timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetReceiveTimeout(VirtualSocket socket, int milliseconds)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetReceiveTimeout(s!, milliseconds);
        }

        /// <summary>
        ///     Binds a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to bind to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError Bind(VirtualSocket socket, NativeSocketAddress socketAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.Bind(s!, socketAddress);
        }

        /// <summary>
        ///     Connects a socket to a socket address.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="socketAddress">The socket address to connect to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError Connect(VirtualSocket socket, NativeSocketAddress socketAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.Connect(s!, socketAddress);
        }

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError SetBlocking(VirtualSocket socket, bool blocking)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.SetBlocking(s!, blocking);
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError Poll(VirtualSocket socket, int microseconds, SelectMode mode, out bool status)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
            {
                status = default;
                return error;
            }

            return ManagedVirtualSocketPalImpl.Poll(s!, microseconds, mode, out status);
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The virtual socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///     thrown; always check the returned socket error instead of ignoring or discarding it.
        /// </remarks>
        public static SocketError PollFlags(VirtualSocket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
            {
                outFlags = default;
                return error;
            }

            return ManagedVirtualSocketPalImpl.PollFlags(s!, microseconds, inFlags, out outFlags);
        }

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
        ///                 There is a known bug in the underlying managed socket: after a <c>SendTo</c> that
        ///                 triggers an implicit bind, the socket is actually bound, but <c>LocalEndPoint</c>
        ///                 cannot be queried and throws. In that state this method reports an error even though
        ///                 the datagram was delivered successfully.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static SocketError GetName(VirtualSocket socket, ref NativeSocketAddress socketAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return error;

            return ManagedVirtualSocketPalImpl.GetName(s!, ref socketAddress);
        }

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
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult Send(VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.Send(s!, buffer, socketFlags);
        }

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
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult Receive(VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.Receive(s!, buffer, socketFlags);
        }

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
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult SendTo(VirtualSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.SendTo(s!, buffer, socketFlags, socketAddress);
        }

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
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult ReceiveFrom(VirtualSocket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.ReceiveFrom(s!, buffer, socketFlags, ref socketAddress);
        }

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
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult SendVectored(VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.SendVectored(s!, buffers, socketFlags);
        }

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
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult SendToVectored(VirtualSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.SendToVectored(s!, buffers, socketFlags, socketAddress);
        }

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
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult ReceiveVectored(VirtualSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.ReceiveVectored(s!, buffers, socketFlags);
        }

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
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Exceptions are caught and reported as the <see cref="SocketError" /> instead of being
        ///                 thrown; always check the returned socket error instead of ignoring or discarding it.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult ReceiveFromVectored(VirtualSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, ref NativeSocketAddress socketAddress)
        {
            SocketError error = socket.TryGetSocket(out Socket? s);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            return ManagedVirtualSocketPalImpl.ReceiveFromVectored(s!, buffers, socketFlags, ref socketAddress);
        }

        /// <summary>
        ///     Gets the virtual socket pal implementation for this class.
        /// </summary>
        /// <returns>The <see cref="VirtualSocketPalImpl" /> for the managed implementation.</returns>
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