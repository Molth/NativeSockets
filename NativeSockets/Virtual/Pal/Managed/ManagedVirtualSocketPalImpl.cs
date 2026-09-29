using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides managed virtual socket operations backed by <see cref="Socket" />.
    /// </summary>
    internal static partial class ManagedVirtualSocketPalImpl
    {
        /// <summary>
        ///     The thread-local list of sockets to check for readability.
        /// </summary>
        [ThreadStatic] private static List<Socket>? _checkRead;

        /// <summary>
        ///     The thread-local list of sockets to check for writability.
        /// </summary>
        [ThreadStatic] private static List<Socket>? _checkWrite;

        /// <summary>
        ///     The thread-local list of sockets to check for errors.
        /// </summary>
        [ThreadStatic] private static List<Socket>? _checkError;

        /// <summary>
        ///     Closes a managed socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError Close(Socket socket)
        {
            try
            {
                socket.Dispose();
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetDualMode(Socket socket, bool dualMode)
        {
            try
            {
                socket.DualMode = dualMode;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets whether the socket allows its local socket address to be reused.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="reuseAddress">true to allow the local socket address to be reused; false to disallow.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetReuseAddress(Socket socket, bool reuseAddress)
        {
            try
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets whether the socket should not fragment packets.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="dontFragment">true to not fragment packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetDontFragment(Socket socket, bool dontFragment)
        {
            try
            {
                socket.DontFragment = dontFragment;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch (NotSupportedException)
            {
                return SocketError.AddressFamilyNotSupported;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets whether the socket should not route packets.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="dontRoute">true to not route packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetDontRoute(Socket socket, bool dontRoute)
        {
            try
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, dontRoute);
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets whether the socket can send broadcast packets.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="enableBroadcast">true to enable broadcasting; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetEnableBroadcast(Socket socket, bool enableBroadcast)
        {
            try
            {
                socket.EnableBroadcast = enableBroadcast;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets the time-to-live value for the socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="ttl">The time-to-live value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetTtl(Socket socket, int ttl)
        {
            try
            {
                if (socket.AddressFamily == AddressFamily.InterNetwork)
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, ttl);
                else
                    socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit, ttl);

                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets the size of the send buffer for the socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="sendBufferSize">The size of the send buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetSendBufferSize(Socket socket, int sendBufferSize)
        {
            try
            {
                socket.SendBufferSize = sendBufferSize;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch (ArgumentOutOfRangeException)
            {
                return SocketError.InvalidArgument;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets the size of the receive buffer for the socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="receiveBufferSize">The size of the receive buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetReceiveBufferSize(Socket socket, int receiveBufferSize)
        {
            try
            {
                socket.ReceiveBufferSize = receiveBufferSize;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch (ArgumentOutOfRangeException)
            {
                return SocketError.InvalidArgument;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets the send timeout for the socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="milliseconds">The send timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetSendTimeout(Socket socket, int milliseconds)
        {
            try
            {
                socket.SendTimeout = milliseconds;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch (ArgumentOutOfRangeException)
            {
                return SocketError.InvalidArgument;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets the receive timeout for the socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="milliseconds">The receive timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetReceiveTimeout(Socket socket, int milliseconds)
        {
            try
            {
                socket.ReceiveTimeout = milliseconds;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch (ArgumentOutOfRangeException)
            {
                return SocketError.InvalidArgument;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Binds a socket to a socket address.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="socketAddress">The socket address to bind to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError Bind(Socket socket, NativeSocketAddress socketAddress)
        {
            SocketError error = socketAddress.ToIpEndPoint(out IPEndPoint? ipEndPoint);
            if (error != SocketError.Success)
                return error;

            try
            {
                socket.Bind(ipEndPoint!);
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch (SecurityException)
            {
                return SocketError.AccessDenied;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Connects a socket to a socket address.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="socketAddress">The socket address to connect to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError Connect(Socket socket, NativeSocketAddress socketAddress)
        {
            SocketError error = socketAddress.ToIpEndPoint(out IPEndPoint? ipEndPoint);
            if (error != SocketError.Success)
                return error;

            try
            {
                socket.Connect(ipEndPoint!);
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch (SecurityException)
            {
                return SocketError.AccessDenied;
            }
            catch (InvalidOperationException)
            {
                return SocketError.IsConnected;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError SetBlocking(Socket socket, bool blocking)
        {
            try
            {
                socket.Blocking = blocking;
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError Poll(Socket socket, int microseconds, SelectMode mode, out bool status)
        {
            try
            {
                status = socket.Poll(microseconds, mode);
                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                status = default;
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                status = default;
                return SocketError.NotSocket;
            }
            catch (NotSupportedException)
            {
                status = default;
                return SocketError.InvalidArgument;
            }
            catch
            {
                status = default;
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError PollFlags(Socket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags)
        {
            List<Socket>? checkRead;
            List<Socket>? checkWrite;
            List<Socket>? checkError;

            if ((inFlags & SelectModeFlags.SelectRead) == 0)
            {
                checkRead = null;
            }
            else
            {
                checkRead = _checkRead ??= new List<Socket>();
                checkRead.Add(socket);
            }

            if ((inFlags & SelectModeFlags.SelectWrite) == 0)
            {
                checkWrite = null;
            }
            else
            {
                checkWrite = _checkWrite ??= new List<Socket>();
                checkWrite.Add(socket);
            }

            if ((inFlags & SelectModeFlags.SelectError) == 0)
            {
                checkError = null;
            }
            else
            {
                checkError = _checkError ??= new List<Socket>();
                checkError.Add(socket);
            }

            outFlags = 0;

            if (checkRead == null && checkWrite == null && checkError == null)
                return SocketError.InvalidArgument;

            try
            {
                Socket.Select(checkRead, checkWrite, checkError, microseconds);

                if (checkRead != null && checkRead.Count != 0)
                    outFlags |= SelectModeFlags.SelectRead;

                if (checkWrite != null && checkWrite.Count != 0)
                    outFlags |= SelectModeFlags.SelectWrite;

                if (checkError != null && checkError.Count != 0)
                    outFlags |= SelectModeFlags.SelectError;

                return SocketError.Success;
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            finally
            {
                checkRead?.Clear();
                checkWrite?.Clear();
                checkError?.Clear();
            }
        }

        /// <summary>
        ///     Gets the local name (socket address) of a socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
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
        public static SocketError GetName(Socket socket, ref NativeSocketAddress socketAddress)
        {
            try
            {
                IPEndPoint? ipEndPoint = (IPEndPoint?)socket.LocalEndPoint;
                return ipEndPoint == null ? SocketError.AddressNotAvailable : NativeSocketAddress.FromIpEndPoint(ipEndPoint, out socketAddress);
            }
            catch (SocketException ex)
            {
                return ex.SocketErrorCode;
            }
            catch (ObjectDisposedException)
            {
                return SocketError.NotSocket;
            }
            catch
            {
                return SocketError.SocketError;
            }
        }

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult Send(Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
        {
            try
            {
                int num = socket.Send(buffer, WindowsSocketFlags.ToNativeSocketFlags(socketFlags), out SocketError socketError);

                return new IoResult(num >= 0 ? num : -1, socketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
            }
        }

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult Receive(Socket socket, Span<byte> buffer, SocketFlags socketFlags)
        {
            try
            {
                int num = socket.Receive(buffer, WindowsSocketFlags.ToNativeSocketFlags(socketFlags), out SocketError socketError);

                return new IoResult(num >= 0 ? num : -1, socketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
            }
        }
    }
}