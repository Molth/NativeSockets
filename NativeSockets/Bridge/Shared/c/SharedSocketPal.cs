using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations for sending and receiving data.
    /// </summary>
    /// <remarks>
    ///     This class uses function pointers to delegate to the appropriate platform-specific implementation
    ///     (Windows, Linux, Android, macOS) at runtime.
    /// </remarks>
    internal static unsafe class SharedSocketPal
    {
        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        private static readonly delegate* managed<SocketError> _GetLastSocketError;

        /// <summary>
        ///     Starts up the platform-specific socket subsystem (e.g., WSAStartup on Windows).
        /// </summary>
        private static readonly delegate* managed<SocketError> _Startup;

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem (e.g., WSACleanup on Windows).
        /// </summary>
        private static readonly delegate* managed<SocketError> _Cleanup;

        /// <summary>
        ///     Creates a native socket handle for the specified address family (Ipv4 or Ipv6).
        /// </summary>
        private static readonly delegate* managed<bool, nint> _Create;

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        private static readonly delegate* managed<nint, SocketError> _Close;

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        private static readonly delegate* managed<nint, bool, SocketError> _SetDualModeIpv6;

        /// <summary>
        ///     Binds a socket to an Ipv4 address.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in4*, SocketError> _BindIpv4;

        /// <summary>
        ///     Binds a socket to an Ipv6 address.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _BindIpv6;

        /// <summary>
        ///     Connects a socket to an Ipv4 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in4*, SocketError> _ConnectIpv4;

        /// <summary>
        ///     Connects a socket to an Ipv6 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _ConnectIpv6;

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, byte*, int, SocketError> _SetOption;

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, byte*, int*, SocketError> _GetOption;

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        private static readonly delegate* managed<nint, bool, SocketError> _SetBlocking;

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        private static readonly delegate* managed<nint, int, SelectMode, out bool, SocketError> _Poll;

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        private static readonly delegate* managed<nint, int, SelectModeFlags, out SelectModeFlags, SocketError> _PollFlags;

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, int> _Send;

        /// <summary>
        ///     Sends data to an Ipv4 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in4*, int> _SendToIpv4;

        /// <summary>
        ///     Sends data to an Ipv6 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in6*, int> _SendToIpv6;

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, int> _Receive;

        /// <summary>
        ///     Receives data from an Ipv4 endpoint, filling the provided address structure.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in4*, int> _ReceiveFromIpv4;

        /// <summary>
        ///     Receives data from an Ipv6 endpoint, filling the provided address structure.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in6*, int> _ReceiveFromIpv6;

        /// <summary>
        ///     Sends a message on a connected socket.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags, int> _SendMessage;

        /// <summary>
        ///     Sends a message to an Ipv4 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags, sockaddr_in4*, int> _SendMessageToIpv4;

        /// <summary>
        ///     Sends a message to an Ipv6 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags, sockaddr_in6*, int> _SendMessageToIpv6;

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, int> _ReceiveMessage;

        /// <summary>
        ///     Receives a message from an Ipv4 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, sockaddr_in4*, int> _ReceiveMessageFromIpv4;

        /// <summary>
        ///     Receives a message from an Ipv6 endpoint.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, sockaddr_in6*, int> _ReceiveMessageFromIpv6;

        /// <summary>
        ///     Gets the local name (address) of an Ipv4 socket.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in4*, SocketError> _GetNameIpv4;

        /// <summary>
        ///     Gets the local name (address) of an Ipv6 socket.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _GetNameIpv6;

        /// <summary>
        ///     Sets the Ipv4 address in the given address structure.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in4*, ReadOnlySpan<byte>, SocketError> _SetIpIpv4;

        /// <summary>
        ///     Sets the Ipv6 address in the given address structure.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<byte>, SocketError> _SetIpIpv6;

        /// <summary>
        ///     Retrieves the Ipv4 address from a socket address structure.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in4*, Span<byte>, SocketError> _GetIpIpv4;

        /// <summary>
        ///     Retrieves the Ipv6 address from a socket address structure.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetIpIpv6;

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv4 address.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in4*, ReadOnlySpan<byte>, SocketError> _SetHostNameIpv4;

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv6 address.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<byte>, SocketError> _SetHostNameIpv6;

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv4 address.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in4*, Span<byte>, SocketError> _GetHostNameIpv4;

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv6 address.
        /// </summary>
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetHostNameIpv6;

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        static SharedSocketPal()
        {
            if (IsBridge())
            {
                ADDRESS_FAMILY_INTER_NETWORK_V4 = BridgeSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
                ADDRESS_FAMILY_INTER_NETWORK_V6 = BridgeSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;

                _GetLastSocketError = &BridgeSocketPal.GetLastSocketError;
                _Startup = &BridgeSocketPal.Startup;
                _Cleanup = &BridgeSocketPal.Cleanup;
                _Create = &BridgeSocketPal.Create;
                _Close = &BridgeSocketPal.Close;
                _SetDualModeIpv6 = &BridgeSocketPal.SetDualModeIpv6;
                _BindIpv4 = &BridgeSocketPal.BindIpv4;
                _BindIpv6 = &BridgeSocketPal.BindIpv6;
                _ConnectIpv4 = &BridgeSocketPal.ConnectIpv4;
                _ConnectIpv6 = &BridgeSocketPal.ConnectIpv6;
                _SetOption = &BridgeSocketPal.SetOption;
                _GetOption = &BridgeSocketPal.GetOption;
                _SetBlocking = &BridgeSocketPal.SetBlocking;
                _Poll = &BridgeSocketPal.Poll;
                _PollFlags = &BridgeSocketPal.PollFlags;
                _Send = &BridgeSocketPal.Send;
                _SendToIpv4 = &BridgeSocketPal.SendToIpv4;
                _SendToIpv6 = &BridgeSocketPal.SendToIpv6;
                _Receive = &BridgeSocketPal.Receive;
                _ReceiveFromIpv4 = &BridgeSocketPal.ReceiveFromIpv4;
                _ReceiveFromIpv6 = &BridgeSocketPal.ReceiveFromIpv6;
                _SendMessage = &BridgeSocketPal.SendMessage;
                _SendMessageToIpv4 = &BridgeSocketPal.SendMessageToIpv4;
                _SendMessageToIpv6 = &BridgeSocketPal.SendMessageToIpv6;
                _ReceiveMessage = &BridgeSocketPal.ReceiveMessage;
                _ReceiveMessageFromIpv4 = &BridgeSocketPal.ReceiveMessageFromIpv4;
                _ReceiveMessageFromIpv6 = &BridgeSocketPal.ReceiveMessageFromIpv6;
                _GetNameIpv4 = &BridgeSocketPal.GetNameIpv4;
                _GetNameIpv6 = &BridgeSocketPal.GetNameIpv6;
                _SetIpIpv4 = &BridgeSocketPal.SetIpIpv4;
                _SetIpIpv6 = &BridgeSocketPal.SetIpIpv6;
                _GetIpIpv4 = &BridgeSocketPal.GetIpIpv4;
                _GetIpIpv6 = &BridgeSocketPal.GetIpIpv6;
                _SetHostNameIpv4 = &BridgeSocketPal.SetHostNameIpv4;
                _SetHostNameIpv6 = &BridgeSocketPal.SetHostNameIpv6;
                _GetHostNameIpv4 = &BridgeSocketPal.GetHostNameIpv4;
                _GetHostNameIpv6 = &BridgeSocketPal.GetHostNameIpv6;
            }

            else if (IsIos())
            {
                ADDRESS_FAMILY_INTER_NETWORK_V4 = IosSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
                ADDRESS_FAMILY_INTER_NETWORK_V6 = IosSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;

                _GetLastSocketError = &IosSocketPal.GetLastSocketError;
                _Startup = &IosSocketPal.Startup;
                _Cleanup = &IosSocketPal.Cleanup;
                _Create = &IosSocketPal.Create;
                _Close = &IosSocketPal.Close;
                _SetDualModeIpv6 = &IosSocketPal.SetDualModeIpv6;
                _BindIpv4 = &IosSocketPal.BindIpv4;
                _BindIpv6 = &IosSocketPal.BindIpv6;
                _ConnectIpv4 = &IosSocketPal.ConnectIpv4;
                _ConnectIpv6 = &IosSocketPal.ConnectIpv6;
                _SetOption = &IosSocketPal.SetOption;
                _GetOption = &IosSocketPal.GetOption;
                _SetBlocking = &IosSocketPal.SetBlocking;
                _Poll = &IosSocketPal.Poll;
                _PollFlags = &IosSocketPal.PollFlags;
                _Send = &IosSocketPal.Send;
                _SendToIpv4 = &IosSocketPal.SendToIpv4;
                _SendToIpv6 = &IosSocketPal.SendToIpv6;
                _Receive = &IosSocketPal.Receive;
                _ReceiveFromIpv4 = &IosSocketPal.ReceiveFromIpv4;
                _ReceiveFromIpv6 = &IosSocketPal.ReceiveFromIpv6;
                _SendMessage = &IosSocketPal.SendMessage;
                _SendMessageToIpv4 = &IosSocketPal.SendMessageToIpv4;
                _SendMessageToIpv6 = &IosSocketPal.SendMessageToIpv6;
                _ReceiveMessage = &IosSocketPal.ReceiveMessage;
                _ReceiveMessageFromIpv4 = &IosSocketPal.ReceiveMessageFromIpv4;
                _ReceiveMessageFromIpv6 = &IosSocketPal.ReceiveMessageFromIpv6;
                _GetNameIpv4 = &IosSocketPal.GetNameIpv4;
                _GetNameIpv6 = &IosSocketPal.GetNameIpv6;
                _SetIpIpv4 = &IosSocketPal.SetIpIpv4;
                _SetIpIpv6 = &IosSocketPal.SetIpIpv6;
                _GetIpIpv4 = &IosSocketPal.GetIpIpv4;
                _GetIpIpv6 = &IosSocketPal.GetIpIpv6;
                _SetHostNameIpv4 = &IosSocketPal.SetHostNameIpv4;
                _SetHostNameIpv6 = &IosSocketPal.SetHostNameIpv6;
                _GetHostNameIpv4 = &IosSocketPal.GetHostNameIpv4;
                _GetHostNameIpv6 = &IosSocketPal.GetHostNameIpv6;
            }

            IsSupported = IsBridge() || IsIos();

            return;

            static bool IsBridge() => BridgeSocketPal.IsSupported;
            static bool IsIos() => IosSocketPal.IsSupported;
        }

        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        public static ushort ADDRESS_FAMILY_INTER_NETWORK_V4 { get; }

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        public static ushort ADDRESS_FAMILY_INTER_NETWORK_V6 { get; }

        /// <summary>
        ///     Gets a value indicating whether any platform-specific implementation is supported.
        /// </summary>
        public static bool IsSupported { get; }

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError() => _GetLastSocketError();

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Startup() => _Startup();

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup() => _Cleanup();

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <returns>The native socket handle, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint Create(bool ipv6) => _Create(ipv6);

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(nint socket) => _Close(socket);

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDualModeIpv6(nint socket, bool dualMode) => _SetDualModeIpv6(socket, dualMode);

        /// <summary>
        ///     Binds a socket to an Ipv4 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress) => _BindIpv4(socket, socketAddress);

        /// <summary>
        ///     Binds a socket to an Ipv6 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress) => _BindIpv6(socket, socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress) => _ConnectIpv4(socket, socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv6(nint socket, sockaddr_in6* socketAddress) => _ConnectIpv6(socket, socketAddress);

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to the option value.</param>
        /// <param name="length">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int length) => _SetOption(socket, level, name, value, length);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <param name="length">Pointer to the length of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int* length) => _GetOption(socket, level, name, value, length);

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(nint socket, bool blocking) => _SetBlocking(socket, blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(nint socket, int microseconds, SelectMode mode, out bool status) => _Poll(socket, microseconds, mode, out status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(nint socket, int microseconds, SelectModeFlags mode, out SelectModeFlags status) => _PollFlags(socket, microseconds, mode, out status);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer in bytes.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Send(nint socket, void* buffer, int length, SocketFlags socketFlags) => _Send(socket, buffer, length, socketFlags);

        /// <summary>
        ///     Sends data to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress) => _SendToIpv4(socket, buffer, length, socketFlags, socketAddress);

        /// <summary>
        ///     Sends data to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress) => _SendToIpv6(socket, buffer, length, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Receive(nint socket, void* buffer, int length, SocketFlags socketFlags) => _Receive(socket, buffer, length, socketFlags);

        /// <summary>
        ///     Receives data from an Ipv4 endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress) => _ReceiveFromIpv4(socket, buffer, length, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data from an Ipv6 endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress) => _ReceiveFromIpv6(socket, buffer, length, socketFlags, socketAddress);

        /// <summary>
        ///     Sends a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessage(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags) => _SendMessage(socket, buffers, bufferCount, socketFlags);

        /// <summary>
        ///     Sends a message to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in4* socketAddress) => _SendMessageToIpv4(socket, buffers, bufferCount, socketFlags, socketAddress);

        /// <summary>
        ///     Sends a message to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in6* socketAddress) => _SendMessageToIpv6(socket, buffers, bufferCount, socketFlags, socketAddress);

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessage(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags) => _ReceiveMessage(socket, buffers, bufferCount, socketFlags);

        /// <summary>
        ///     Receives a message from an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags, sockaddr_in4* socketAddress) => _ReceiveMessageFromIpv4(socket, buffers, bufferCount, socketFlags, socketAddress);

        /// <summary>
        ///     Receives a message from an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags, sockaddr_in6* socketAddress) => _ReceiveMessageFromIpv6(socket, buffers, bufferCount, socketFlags, socketAddress);

        /// <summary>
        ///     Gets the local name (address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress) => _GetNameIpv4(socket, socketAddress);

        /// <summary>
        ///     Gets the local name (address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress) => _GetNameIpv6(socket, socketAddress);

        /// <summary>
        ///     Sets the Ipv4 address in the given address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">The ip address as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv4(sockaddr_in4* socketAddress, ReadOnlySpan<byte> ip) => _SetIpIpv4(socketAddress, ip);

        /// <summary>
        ///     Sets the Ipv6 address in the given address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="ip">The ip address as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv6(sockaddr_in6* socketAddress, ReadOnlySpan<byte> ip) => _SetIpIpv6(socketAddress, ip);

        /// <summary>
        ///     Retrieves the Ipv4 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIpIpv4(sockaddr_in4* socketAddress, Span<byte> ip) => _GetIpIpv4(socketAddress, ip);

        /// <summary>
        ///     Retrieves the Ipv6 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIpIpv6(sockaddr_in6* socketAddress, Span<byte> ip) => _GetIpIpv6(socketAddress, ip);

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">The host name as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv4(sockaddr_in4* socketAddress, ReadOnlySpan<byte> hostName) => _SetHostNameIpv4(socketAddress, hostName);

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">The host name as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv6(sockaddr_in6* socketAddress, ReadOnlySpan<byte> hostName) => _SetHostNameIpv6(socketAddress, hostName);

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostNameIpv4(sockaddr_in4* socketAddress, Span<byte> hostName) => _GetHostNameIpv4(socketAddress, hostName);

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostNameIpv6(sockaddr_in6* socketAddress, Span<byte> hostName) => _GetHostNameIpv6(socketAddress, hostName);
    }
}