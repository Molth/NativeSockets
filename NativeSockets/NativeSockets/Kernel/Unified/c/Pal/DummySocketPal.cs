using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations.
    /// </summary>
    internal static unsafe class DummySocketPal
    {
        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V4 = (ushort)AddressFamily.InterNetwork;

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V6 = (ushort)AddressFamily.InterNetworkV6;

        /// <summary>
        ///     Gets a value indicating whether any platform-specific implementation is supported.
        /// </summary>
        public static bool IsSupported => false;

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError()
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Startup()
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup()
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <param name="socket">When this method returns, contains the native socket handle, or -1 on error.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Create(bool ipv6, out nint socket)
        {
            Unsafe.SkipInit(out socket);

            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(nint socket)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Binds a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Binds a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Connects a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Connects a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to the option value.</param>
        /// <param name="length">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, void* value, int length)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <param name="length">Pointer to the length of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, void* value, int* length)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to the option value.</param>
        /// <param name="length">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetRawOption(nint socket, int level, int name, void* value, int length)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <param name="length">Pointer to the length of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetRawOption(nint socket, int level, int name, void* value, int* length)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(nint socket, bool blocking)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(nint socket, int microseconds, SelectMode mode, out bool status)
        {
            Unsafe.SkipInit(out status);

            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(nint socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags)
        {
            Unsafe.SkipInit(out outFlags);

            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer in bytes.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Send(nint socket, void* buffer, int length, SocketFlags socketFlags)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sends data to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendToIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sends data to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendToIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Receive(nint socket, void* buffer, int length, SocketFlags socketFlags)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Receives data from an Ipv4 socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFromIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Receives data from an Ipv6 socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFromIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendToVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult SendToVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFromVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in4* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">Always thrown by this method.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult ReceiveFromVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in6* socketAddress)
        {
            ThrowHelpers.ThrowPlatformNotSupportedException();
            return default;
        }

        /// <summary>
        ///     Creates a <see cref="SocketPalImpl" /> populated with this platform's socket operations.
        /// </summary>
        /// <returns>A <see cref="SocketPalImpl" /> containing this platform's socket operations.</returns>
        public static SocketPalImpl GetImpl()
        {
            SocketPalImpl impl;
            impl.ADDRESS_FAMILY_INTER_NETWORK_V4 = ADDRESS_FAMILY_INTER_NETWORK_V4;
            impl.ADDRESS_FAMILY_INTER_NETWORK_V6 = ADDRESS_FAMILY_INTER_NETWORK_V6;
            impl.GetLastSocketError = &GetLastSocketError;
            impl.Startup = &Startup;
            impl.Cleanup = &Cleanup;
            impl.Create = &Create;
            impl.Close = &Close;
            impl.BindIpv4 = &BindIpv4;
            impl.BindIpv6 = &BindIpv6;
            impl.ConnectIpv4 = &ConnectIpv4;
            impl.ConnectIpv6 = &ConnectIpv6;
            impl.SetOption = &SetOption;
            impl.GetOption = &GetOption;
            impl.SetRawOption = &SetRawOption;
            impl.GetRawOption = &GetRawOption;
            impl.SetBlocking = &SetBlocking;
            impl.Poll = &Poll;
            impl.PollFlags = &PollFlags;
            impl.GetNameIpv4 = &GetNameIpv4;
            impl.GetNameIpv6 = &GetNameIpv6;
            impl.Send = &Send;
            impl.SendToIpv4 = &SendToIpv4;
            impl.SendToIpv6 = &SendToIpv6;
            impl.Receive = &Receive;
            impl.ReceiveFromIpv4 = &ReceiveFromIpv4;
            impl.ReceiveFromIpv6 = &ReceiveFromIpv6;
            impl.SendVectored = &SendVectored;
            impl.SendToVectoredIpv4 = &SendToVectoredIpv4;
            impl.SendToVectoredIpv6 = &SendToVectoredIpv6;
            impl.ReceiveVectored = &ReceiveVectored;
            impl.ReceiveFromVectoredIpv4 = &ReceiveFromVectoredIpv4;
            impl.ReceiveFromVectoredIpv6 = &ReceiveFromVectoredIpv6;
            return impl;
        }
    }
}