using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations.
    /// </summary>
    internal static unsafe class UnifiedSocketPal
    {
        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        private static readonly delegate* managed<SocketError> _GetLastSocketError;

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        private static readonly delegate* managed<SocketError> _Startup;

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        private static readonly delegate* managed<SocketError> _Cleanup;

        /// <summary>
        ///     Creates a native socket handle for the specified address family (Ipv4 or Ipv6).
        /// </summary>
        private static readonly delegate* managed<bool, out nint, SocketError> _Create;

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        private static readonly delegate* managed<nint, SocketError> _Close;

        /// <summary>
        ///     Binds a socket to an Ipv4 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in4*, SocketError> _BindIpv4;

        /// <summary>
        ///     Binds a socket to an Ipv6 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _BindIpv6;

        /// <summary>
        ///     Connects a socket to an Ipv4 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in4*, SocketError> _ConnectIpv4;

        /// <summary>
        ///     Connects a socket to an Ipv6 socket address.
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
        ///     Sets a socket option.
        /// </summary>
        private static readonly delegate* managed<nint, int, int, byte*, int, SocketError> _SetRawOption;

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        private static readonly delegate* managed<nint, int, int, byte*, int*, SocketError> _GetRawOption;

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
        ///     Sends data to an Ipv4 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in4*, int> _SendToIpv4;

        /// <summary>
        ///     Sends data to an Ipv6 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in6*, int> _SendToIpv6;

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, int> _Receive;

        /// <summary>
        ///     Receives data from an Ipv4 socket address, filling the provided socket address.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in4*, int> _ReceiveFromIpv4;

        /// <summary>
        ///     Receives data from an Ipv6 socket address, filling the provided socket address.
        /// </summary>
        private static readonly delegate* managed<nint, void*, int, SocketFlags, sockaddr_in6*, int> _ReceiveFromIpv6;

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags, int> _SendVectored;

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv4 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags, sockaddr_in4*, int> _SendToVectoredIpv4;

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv6 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags, sockaddr_in6*, int> _SendToVectoredIpv6;

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, int> _ReceiveVectored;

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv4 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, sockaddr_in4*, int> _ReceiveFromVectoredIpv4;

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv6 socket address.
        /// </summary>
        private static readonly delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, sockaddr_in6*, int> _ReceiveFromVectoredIpv6;

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv4 socket.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in4*, SocketError> _GetNameIpv4;

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv6 socket.
        /// </summary>
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _GetNameIpv6;

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        static UnifiedSocketPal()
        {
            if (IsShared())
            {
                ADDRESS_FAMILY_INTER_NETWORK_V4 = SharedSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
                ADDRESS_FAMILY_INTER_NETWORK_V6 = SharedSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;

                _GetLastSocketError = &SharedSocketPal.GetLastSocketError;
                _Startup = &SharedSocketPal.Startup;
                _Cleanup = &SharedSocketPal.Cleanup;
                _Create = &SharedSocketPal.Create;
                _Close = &SharedSocketPal.Close;
                _BindIpv4 = &SharedSocketPal.BindIpv4;
                _BindIpv6 = &SharedSocketPal.BindIpv6;
                _ConnectIpv4 = &SharedSocketPal.ConnectIpv4;
                _ConnectIpv6 = &SharedSocketPal.ConnectIpv6;
                _SetOption = &SharedSocketPal.SetOption;
                _GetOption = &SharedSocketPal.GetOption;
                _SetRawOption = &SharedSocketPal.SetRawOption;
                _GetRawOption = &SharedSocketPal.GetRawOption;
                _SetBlocking = &SharedSocketPal.SetBlocking;
                _Poll = &SharedSocketPal.Poll;
                _PollFlags = &SharedSocketPal.PollFlags;
                _Send = &SharedSocketPal.Send;
                _SendToIpv4 = &SharedSocketPal.SendToIpv4;
                _SendToIpv6 = &SharedSocketPal.SendToIpv6;
                _Receive = &SharedSocketPal.Receive;
                _ReceiveFromIpv4 = &SharedSocketPal.ReceiveFromIpv4;
                _ReceiveFromIpv6 = &SharedSocketPal.ReceiveFromIpv6;
                _SendVectored = &SharedSocketPal.SendVectored;
                _SendToVectoredIpv4 = &SharedSocketPal.SendToVectoredIpv4;
                _SendToVectoredIpv6 = &SharedSocketPal.SendToVectoredIpv6;
                _ReceiveVectored = &SharedSocketPal.ReceiveVectored;
                _ReceiveFromVectoredIpv4 = &SharedSocketPal.ReceiveFromVectoredIpv4;
                _ReceiveFromVectoredIpv6 = &SharedSocketPal.ReceiveFromVectoredIpv6;
                _GetNameIpv4 = &SharedSocketPal.GetNameIpv4;
                _GetNameIpv6 = &SharedSocketPal.GetNameIpv6;
            }

            else if (IsStatic())
            {
                ADDRESS_FAMILY_INTER_NETWORK_V4 = StaticSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
                ADDRESS_FAMILY_INTER_NETWORK_V6 = StaticSocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;

                _GetLastSocketError = &StaticSocketPal.GetLastSocketError;
                _Startup = &StaticSocketPal.Startup;
                _Cleanup = &StaticSocketPal.Cleanup;
                _Create = &StaticSocketPal.Create;
                _Close = &StaticSocketPal.Close;
                _BindIpv4 = &StaticSocketPal.BindIpv4;
                _BindIpv6 = &StaticSocketPal.BindIpv6;
                _ConnectIpv4 = &StaticSocketPal.ConnectIpv4;
                _ConnectIpv6 = &StaticSocketPal.ConnectIpv6;
                _SetOption = &StaticSocketPal.SetOption;
                _GetOption = &StaticSocketPal.GetOption;
                _SetRawOption = &StaticSocketPal.SetRawOption;
                _GetRawOption = &StaticSocketPal.GetRawOption;
                _SetBlocking = &StaticSocketPal.SetBlocking;
                _Poll = &StaticSocketPal.Poll;
                _PollFlags = &StaticSocketPal.PollFlags;
                _Send = &StaticSocketPal.Send;
                _SendToIpv4 = &StaticSocketPal.SendToIpv4;
                _SendToIpv6 = &StaticSocketPal.SendToIpv6;
                _Receive = &StaticSocketPal.Receive;
                _ReceiveFromIpv4 = &StaticSocketPal.ReceiveFromIpv4;
                _ReceiveFromIpv6 = &StaticSocketPal.ReceiveFromIpv6;
                _SendVectored = &StaticSocketPal.SendVectored;
                _SendToVectoredIpv4 = &StaticSocketPal.SendToVectoredIpv4;
                _SendToVectoredIpv6 = &StaticSocketPal.SendToVectoredIpv6;
                _ReceiveVectored = &StaticSocketPal.ReceiveVectored;
                _ReceiveFromVectoredIpv4 = &StaticSocketPal.ReceiveFromVectoredIpv4;
                _ReceiveFromVectoredIpv6 = &StaticSocketPal.ReceiveFromVectoredIpv6;
                _GetNameIpv4 = &StaticSocketPal.GetNameIpv4;
                _GetNameIpv6 = &StaticSocketPal.GetNameIpv6;
            }

            else
            {
                _GetLastSocketError = &NotSupportedSocketPal.GetLastSocketError;
                _Startup = &NotSupportedSocketPal.Startup;
                _Cleanup = &NotSupportedSocketPal.Cleanup;
                _Create = &NotSupportedSocketPal.Create;
                _Close = &NotSupportedSocketPal.Close;
                _BindIpv4 = &NotSupportedSocketPal.BindIpv4;
                _BindIpv6 = &NotSupportedSocketPal.BindIpv6;
                _ConnectIpv4 = &NotSupportedSocketPal.ConnectIpv4;
                _ConnectIpv6 = &NotSupportedSocketPal.ConnectIpv6;
                _SetOption = &NotSupportedSocketPal.SetOption;
                _GetOption = &NotSupportedSocketPal.GetOption;
                _SetRawOption = &NotSupportedSocketPal.SetRawOption;
                _GetRawOption = &NotSupportedSocketPal.GetRawOption;
                _SetBlocking = &NotSupportedSocketPal.SetBlocking;
                _Poll = &NotSupportedSocketPal.Poll;
                _PollFlags = &NotSupportedSocketPal.PollFlags;
                _Send = &NotSupportedSocketPal.Send;
                _SendToIpv4 = &NotSupportedSocketPal.SendToIpv4;
                _SendToIpv6 = &NotSupportedSocketPal.SendToIpv6;
                _Receive = &NotSupportedSocketPal.Receive;
                _ReceiveFromIpv4 = &NotSupportedSocketPal.ReceiveFromIpv4;
                _ReceiveFromIpv6 = &NotSupportedSocketPal.ReceiveFromIpv6;
                _SendVectored = &NotSupportedSocketPal.SendVectored;
                _SendToVectoredIpv4 = &NotSupportedSocketPal.SendToVectoredIpv4;
                _SendToVectoredIpv6 = &NotSupportedSocketPal.SendToVectoredIpv6;
                _ReceiveVectored = &NotSupportedSocketPal.ReceiveVectored;
                _ReceiveFromVectoredIpv4 = &NotSupportedSocketPal.ReceiveFromVectoredIpv4;
                _ReceiveFromVectoredIpv6 = &NotSupportedSocketPal.ReceiveFromVectoredIpv6;
                _GetNameIpv4 = &NotSupportedSocketPal.GetNameIpv4;
                _GetNameIpv6 = &NotSupportedSocketPal.GetNameIpv6;
            }

            IsSupported = IsShared() || IsStatic();

            return;

            static bool IsShared() => SharedSocketPal.IsSupported;
            static bool IsStatic() => StaticSocketPal.IsSupported;
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
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup() => _Cleanup();

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <param name="socket">When this method returns, contains the native socket handle, or -1 on error.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Create(bool ipv6, out nint socket) => _Create(ipv6, out socket);

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(nint socket) => _Close(socket);

        /// <summary>
        ///     Binds a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress) => _BindIpv4(socket, socketAddress);

        /// <summary>
        ///     Binds a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress) => _BindIpv6(socket, socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress) => _ConnectIpv4(socket, socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
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
        /// <remarks>
        ///     The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///     platform equivalents by the underlying socket layer. The <paramref name="value" /> bytes are
        ///     passed through unmodified; the platform interprets the buffer according to the mapped option.
        /// </remarks>
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
        /// <remarks>
        ///     The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///     platform equivalents by the underlying socket layer. The <paramref name="value" /> buffer is
        ///     passed through unmodified; the platform populates the buffer according to the mapped option.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int* length) => _GetOption(socket, level, name, value, length);

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
        public static SocketError SetRawOption(nint socket, int level, int name, byte* value, int length) => _SetRawOption(socket, level, name, value, length);

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
        public static SocketError GetRawOption(nint socket, int level, int name, byte* value, int* length) => _GetRawOption(socket, level, name, value, length);

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
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(nint socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags) => _PollFlags(socket, microseconds, inFlags, out outFlags);

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
        ///     Sends data to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress) => _SendToIpv4(socket, buffer, length, socketFlags, socketAddress);

        /// <summary>
        ///     Sends data to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
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
        ///     Receives data from an Ipv4 socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress) => _ReceiveFromIpv4(socket, buffer, length, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data from an Ipv6 socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress) => _ReceiveFromIpv6(socket, buffer, length, socketFlags, socketAddress);

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags) => _SendVectored(socket, buffers, bufferCount, socketFlags);

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in4* socketAddress) => _SendToVectoredIpv4(socket, buffers, bufferCount, socketFlags, socketAddress);

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in6* socketAddress) => _SendToVectoredIpv6(socket, buffers, bufferCount, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <remarks>
        ///     If the <c>inOutFlags</c> returned by the receive operation is not equal to <c>0</c>,
        ///     the operation is considered failed and returns <c>-1</c>,
        ///     even if <c>GetLastSocketError</c> returns <see cref="SocketError.Success" />.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags) => _ReceiveVectored(socket, buffers, bufferCount, inOutFlags);

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <remarks>
        ///     If the <c>inOutFlags</c> returned by the receive operation is not equal to <c>0</c>,
        ///     the operation is considered failed and returns <c>-1</c>,
        ///     even if <c>GetLastSocketError</c> returns <see cref="SocketError.Success" />.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in4* socketAddress) => _ReceiveFromVectoredIpv4(socket, buffers, bufferCount, inOutFlags, socketAddress);

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <remarks>
        ///     If the <c>inOutFlags</c> returned by the receive operation is not equal to <c>0</c>,
        ///     the operation is considered failed and returns <c>-1</c>,
        ///     even if <c>GetLastSocketError</c> returns <see cref="SocketError.Success" />.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in6* socketAddress) => _ReceiveFromVectoredIpv6(socket, buffers, bufferCount, inOutFlags, socketAddress);

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress) => _GetNameIpv4(socket, socketAddress);

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress) => _GetNameIpv6(socket, socketAddress);
    }
}