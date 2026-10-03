using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides extension methods for <see cref="NativeSocket" />.
    /// </summary>
    public static unsafe class NativeSocketExtensions
    {
        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDualMode(this NativeSocket socket, bool dualMode)
        {
            int optionValue = dualMode ? 0 : 1;
            return socket.SetOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, MemoryMarshalHelpers.AsReadOnlyBytes(ref optionValue));
        }

        /// <summary>
        ///     Sets whether the socket allows its local socket address to be reused.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="reuseAddress">true to allow the local socket address to be reused; false to disallow.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReuseAddress(this NativeSocket socket, bool reuseAddress)
        {
            int optionValue = reuseAddress ? 1 : 0;

            if (!OsName.IsWindows())
            {
                SocketError error = socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, MemoryMarshalHelpers.AsReadOnlyBytes(ref optionValue));
                if (error != SocketError.Success)
                    return error;
            }

            return socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, MemoryMarshalHelpers.AsReadOnlyBytes(ref optionValue));
        }

        /// <summary>
        ///     Sets whether the socket should not fragment packets.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="dontFragment">true to not fragment packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDontFragment(this NativeSocket socket, bool dontFragment)
        {
            int optionValue = dontFragment ? OsName.IsLinux() ? 2 : 1 : 0;
            return socket.SetOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, MemoryMarshalHelpers.AsReadOnlyBytes(ref optionValue));
        }

        /// <summary>
        ///     Sets whether the socket should not route packets.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="dontRoute">true to not route packets; false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDontRoute(this NativeSocket socket, bool dontRoute)
        {
            int optionValue = dontRoute ? 1 : 0;
            return socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, MemoryMarshalHelpers.AsReadOnlyBytes(ref optionValue));
        }

        /// <summary>
        ///     Sets whether the socket can send broadcast packets.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="enableBroadcast">true to enable broadcasting; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetEnableBroadcast(this NativeSocket socket, bool enableBroadcast)
        {
            int optionValue = enableBroadcast ? 1 : 0;
            return socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, MemoryMarshalHelpers.AsReadOnlyBytes(ref optionValue));
        }

        /// <summary>
        ///     Sets the time-to-live value for the socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="ttl">The time-to-live value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetTtl(this NativeSocket socket, int ttl) => socket.IsIpv4 ? socket.SetOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, MemoryMarshalHelpers.AsReadOnlyBytes(ref ttl)) : socket.SetOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit, MemoryMarshalHelpers.AsReadOnlyBytes(ref ttl));

        /// <summary>
        ///     Sets the size of the send buffer for the socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="sendBufferSize">The size of the send buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetSendBufferSize(this NativeSocket socket, int sendBufferSize) => socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, MemoryMarshalHelpers.AsReadOnlyBytes(ref sendBufferSize));

        /// <summary>
        ///     Sets the size of the receive buffer for the socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="receiveBufferSize">The size of the receive buffer, in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReceiveBufferSize(this NativeSocket socket, int receiveBufferSize) => socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, MemoryMarshalHelpers.AsReadOnlyBytes(ref receiveBufferSize));

        /// <summary>
        ///     Sets the send timeout for the socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="milliseconds">The send timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetSendTimeout(this NativeSocket socket, int milliseconds)
        {
            if (!OsName.IsWindows())
            {
                UnixTimeValue timeout = new UnixTimeValue();
                UnixNativeLib.MillisecondsToTimeValue(milliseconds, ref timeout);

                return socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, MemoryMarshalHelpers.AsReadOnlyBytes(ref timeout));
            }

            return socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, MemoryMarshalHelpers.AsReadOnlyBytes(ref milliseconds));
        }

        /// <summary>
        ///     Sets the receive timeout for the socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="milliseconds">The receive timeout in milliseconds.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetReceiveTimeout(this NativeSocket socket, int milliseconds)
        {
            if (!OsName.IsWindows())
            {
                UnixTimeValue timeout = new UnixTimeValue();
                UnixNativeLib.MillisecondsToTimeValue(milliseconds, ref timeout);

                return socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, MemoryMarshalHelpers.AsReadOnlyBytes(ref timeout));
            }

            return socket.SetOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, MemoryMarshalHelpers.AsReadOnlyBytes(ref milliseconds));
        }

        /// <summary>
        ///     Binds a socket to a socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">The socket address to bind to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind(this NativeSocket socket, NativeSocketAddress socketAddress) => socket.IsIpv4 ? SocketPal.BindIpv4(socket, (sockaddr_in4*)&socketAddress) : SocketPal.BindIpv6(socket, (sockaddr_in6*)&socketAddress);

        /// <summary>
        ///     Connects a socket to a socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">The socket address to connect to.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(this NativeSocket socket, NativeSocketAddress socketAddress) => socket.IsIpv4 ? SocketPal.ConnectIpv4(socket, (sockaddr_in4*)&socketAddress) : SocketPal.ConnectIpv6(socket, (sockaddr_in6*)&socketAddress);

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">The option value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///                 platform equivalents by the underlying socket layer.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 The <paramref name="value" /> bytes are passed through unmodified; the platform interprets the
        ///                 buffer according to the mapped option.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
        ///                 <paramref name="level" /> and <paramref name="name" /> is guaranteed.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(this NativeSocket socket, SocketOptionLevel level, SocketOptionName name, ReadOnlySpan<byte> value)
        {
            fixed (void* pValue = &MemoryMarshal.GetReference(value))
            {
                return SocketPal.SetOption(socket, level, name, pValue, value.Length);
            }
        }

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">The buffer to receive the option value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///                 platform equivalents by the underlying socket layer.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 The <paramref name="value" /> buffer is passed through unmodified; the platform populates the
        ///                 buffer according to the mapped option.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
        ///                 <paramref name="level" /> and <paramref name="name" /> is guaranteed.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(this NativeSocket socket, SocketOptionLevel level, SocketOptionName name, ref Span<byte> value)
        {
            int length = value.Length;
            SocketError error;
            fixed (void* pValue = &MemoryMarshal.GetReference(value))
            {
                error = SocketPal.GetOption(socket, level, name, pValue, &length);
            }

            if (error == SocketError.Success)
                value = value.Slice(0, length);

            return error;
        }

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">The option value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetRawOption(this NativeSocket socket, int level, int name, ReadOnlySpan<byte> value)
        {
            fixed (void* pValue = &MemoryMarshal.GetReference(value))
            {
                return SocketPal.SetRawOption(socket, level, name, pValue, value.Length);
            }
        }

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">The buffer to receive the option value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetRawOption(this NativeSocket socket, int level, int name, ref Span<byte> value)
        {
            int length = value.Length;
            SocketError error;
            fixed (void* pValue = &MemoryMarshal.GetReference(value))
            {
                error = SocketPal.GetRawOption(socket, level, name, pValue, &length);
            }

            if (error == SocketError.Success)
                value = value.Slice(0, length);

            return error;
        }

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(this NativeSocket socket, bool blocking) => SocketPal.SetBlocking(socket, blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(this NativeSocket socket, int microseconds, SelectMode mode, out bool status) => SocketPal.Poll(socket, microseconds, mode, out status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(this NativeSocket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags) => SocketPal.PollFlags(socket, microseconds, inFlags, out outFlags);

        /// <summary>
        ///     Gets the local name (socket address) of a socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">The socket address to receive the local name into.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(this NativeSocket socket, out NativeSocketAddress socketAddress)
        {
            SocketError result;
            fixed (void* pAddress = &socketAddress)
            {
                result = socket.IsIpv4 ? SocketPal.GetNameIpv4(socket.Handle, (sockaddr_in4*)pAddress) : SocketPal.GetNameIpv6(socket.Handle, (sockaddr_in6*)pAddress);
            }

            if (socket.IsIpv4 && result == SocketError.Success)
                SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 16), 0, 12);

            return result;
        }

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Send(this NativeSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
        {
            fixed (void* pBuffer = &MemoryMarshal.GetReference(buffer))
            {
                return SocketPal.Send(socket, pBuffer, buffer.Length, socketFlags);
            }
        }

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendTo(this NativeSocket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            fixed (void* pBuffer = &MemoryMarshal.GetReference(buffer))
            {
                fixed (void* pAddress = &socketAddress)
                {
                    return socket.IsIpv4 ? SocketPal.SendToIpv4(socket.Handle, pBuffer, buffer.Length, socketFlags, (sockaddr_in4*)pAddress) : SocketPal.SendToIpv6(socket.Handle, pBuffer, buffer.Length, socketFlags, (sockaddr_in6*)pAddress);
                }
            }
        }

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Receive(this NativeSocket socket, Span<byte> buffer, SocketFlags socketFlags)
        {
            fixed (void* pBuffer = &MemoryMarshal.GetReference(buffer))
            {
                return SocketPal.Receive(socket, pBuffer, buffer.Length, socketFlags);
            }
        }

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFrom(this NativeSocket socket, Span<byte> buffer, SocketFlags socketFlags, out NativeSocketAddress socketAddress)
        {
            IoResult result;
            fixed (void* pBuffer = &MemoryMarshal.GetReference(buffer))
            {
                fixed (void* pAddress = &socketAddress)
                {
                    result = socket.IsIpv4 ? SocketPal.ReceiveFromIpv4(socket.Handle, pBuffer, buffer.Length, socketFlags, (sockaddr_in4*)pAddress) : SocketPal.ReceiveFromIpv6(socket.Handle, pBuffer, buffer.Length, socketFlags, (sockaddr_in6*)pAddress);
                }
            }

            if (socket.IsIpv4 && result.SocketError == SocketError.Success)
                SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 16), 0, 12);

            return result;
        }

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendVectored(this NativeSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags)
        {
            fixed (NativeIoSlice* pBuffer = &MemoryMarshal.GetReference(buffers))
            {
                return SocketPal.SendVectored(socket.Handle, pBuffer, buffers.Length, socketFlags);
            }
        }

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendToVectored(this NativeSocket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            fixed (NativeIoSlice* pBuffer = &MemoryMarshal.GetReference(buffers))
            {
                fixed (void* pAddress = &socketAddress)
                {
                    return socket.IsIpv4 ? SocketPal.SendToVectoredIpv4(socket.Handle, pBuffer, buffers.Length, socketFlags, (sockaddr_in4*)pAddress) : SocketPal.SendToVectoredIpv6(socket.Handle, pBuffer, buffers.Length, socketFlags, (sockaddr_in6*)pAddress);
                }
            }
        }

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveVectored(this NativeSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags)
        {
            fixed (NativeIoSlice* pBuffer = &MemoryMarshal.GetReference(buffers))
            {
                return SocketPal.ReceiveVectored(socket.Handle, pBuffer, buffers.Length, socketFlags);
            }
        }

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFromVectored(this NativeSocket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, out NativeSocketAddress socketAddress)
        {
            IoResult result;
            fixed (NativeIoSlice* pBuffer = &MemoryMarshal.GetReference(buffers))
            {
                fixed (void* pAddress = &socketAddress)
                {
                    result = socket.IsIpv4 ? SocketPal.ReceiveFromVectoredIpv4(socket.Handle, pBuffer, buffers.Length, socketFlags, (sockaddr_in4*)pAddress) : SocketPal.ReceiveFromVectoredIpv6(socket.Handle, pBuffer, buffers.Length, socketFlags, (sockaddr_in6*)pAddress);
                }
            }

            if (socket.IsIpv4 && result.SocketError == SocketError.Success)
                SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 16), 0, 12);

            return result;
        }
    }
}