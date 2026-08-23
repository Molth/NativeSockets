using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using static NativeSockets.WinSocketPal;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations for sending and receiving data.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class WindowsSocketPal
    {
        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V4 = 2;

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V6 = 23;

        /// <summary>
        ///     Gets a value indicating whether any platform-specific implementation is supported.
        /// </summary>
        public static bool IsSupported { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last socket error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError() => (SocketError)_WSAGetLastError();

        /// <summary>
        ///     Initializes the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Initialize()
        {
            WSAData wsaData;
            SocketError error = _WSAStartup(514, &wsaData);
            return error;
        }

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup() => _WSACleanup();

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <returns>The native socket handle, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint Create(bool ipv6)
        {
            ushort family = ipv6 ? ADDRESS_FAMILY_INTER_NETWORK_V6 : ADDRESS_FAMILY_INTER_NETWORK_V4;
            nint socket = _WSASocketW((AddressFamily)family, SocketType.Dgram, ProtocolType.Udp, 0, 0, 1 | 128);

            if (socket != -1)
            {
                byte bNewBehavior = 0;
                int __bytesTransferred_native = 0;
                SocketError error = _WSAIoctl(socket, -1744830452, &bNewBehavior, 1, null, 0, &__bytesTransferred_native, 0, 0);
            }

            return socket;
        }

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(nint socket)
        {
            SocketError error = _closesocket(socket);
            return error;
        }

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDualModeIpv6(nint socket, bool dualMode)
        {
            int optionValue = dualMode ? 0 : 1;
            SocketError error = SetOption(socket, SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, &optionValue);
            return error;
        }

        /// <summary>
        ///     Binds a socket to an Ipv4 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            sockaddr_in4 __socketAddress_native;
            if (socketAddress == null)
            {
                __socketAddress_native = new sockaddr_in4();
                __socketAddress_native.sin4_family = ADDRESS_FAMILY_INTER_NETWORK_V4;

                socketAddress = &__socketAddress_native;
                SetIpIpv4(socketAddress, "0.0.0.0");
            }

            SocketError error = _bind(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in4));
            return error;
        }

        /// <summary>
        ///     Binds a socket to an Ipv6 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            if (socketAddress == null)
            {
                __socketAddress_native = new sockaddr_in6();
                __socketAddress_native.sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;

                socketAddress = &__socketAddress_native;
                SetIpIpv6(socketAddress, "::");
            }

            SocketError error = _bind(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in6));
            return error;
        }

        /// <summary>
        ///     Connects a socket to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            SocketError error = _connect(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in4));
            return error;
        }

        /// <summary>
        ///     Connects a socket to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            SocketError error = _connect(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in6));
            return error;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int length = sizeof(int))
        {
            SocketError error = _setsockopt(socket, level, name, value, length);

            return error == SocketError.SocketError ? GetLastSocketError() : SocketError.Success;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int* length = null)
        {
            int num = sizeof(int);
            if (length == null)
                length = &num;

            SocketError error = _getsockopt(socket, (int)level, (int)name, (byte*)value, length);
            return error == SocketError.SocketError ? GetLastSocketError() : SocketError.Success;
        }

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(nint socket, bool blocking)
        {
            int intBlocking = blocking ? 0 : -1;
            SocketError error = _ioctlsocket(socket, unchecked((int)0x8004667E), &intBlocking);

            if (error == SocketError.SocketError)
                error = GetLastSocketError();

            return error;
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(nint socket, int microseconds, SelectMode mode, out bool status)
        {
            nint* fileDescriptorSet = stackalloc nint[2] { 1, socket };

            int socketCount;
            if (microseconds != -1)
            {
                TimeValue timeout = new TimeValue();
                MicrosecondsToTimeValue(microseconds, ref timeout);
                socketCount = _select(0, mode == SelectMode.SelectRead ? fileDescriptorSet : null, mode == SelectMode.SelectWrite ? fileDescriptorSet : null, mode == SelectMode.SelectError ? fileDescriptorSet : null, &timeout);
            }
            else
            {
                socketCount = _select(0, mode == SelectMode.SelectRead ? fileDescriptorSet : null, mode == SelectMode.SelectWrite ? fileDescriptorSet : null, mode == SelectMode.SelectError ? fileDescriptorSet : null, null);
            }

            if (socketCount == -1)
            {
                status = false;
                return GetLastSocketError();
            }

            status = (int)fileDescriptorSet[0] != 0 && fileDescriptorSet[1] == socket;

            return SocketError.Success;
        }

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer in bytes.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Send(nint socket, void* buffer, int length)
        {
            int num = _send(socket, (byte*)buffer, length, SocketFlags.None);
            return num;
        }

        /// <summary>
        ///     Sends data to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToIpv4(nint socket, void* buffer, int length, sockaddr_in4* socketAddress)
        {
            if (socketAddress != null)
                return _sendto(socket, (byte*)buffer, length, SocketFlags.None, (byte*)socketAddress, sizeof(sockaddr_in4));

            int num = Send(socket, (byte*)buffer, length);
            return num;
        }

        /// <summary>
        ///     Sends data to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToIpv6(nint socket, void* buffer, int length, sockaddr_in6* socketAddress)
        {
            if (socketAddress != null)
                return _sendto(socket, (byte*)buffer, length, SocketFlags.None, (byte*)socketAddress, sizeof(sockaddr_in6));

            int num = Send(socket, (byte*)buffer, length);
            return num;
        }

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Receive(nint socket, void* buffer, int length)
        {
            int num = _recv(socket, (byte*)buffer, length, SocketFlags.None);
            return num;
        }

        /// <summary>
        ///     Receives data from an Ipv4 endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv4(nint socket, void* buffer, int length, sockaddr_in4* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            int socketAddressSize = sizeof(sockaddr_storage);

            int num = _recvfrom(socket, (byte*)buffer, length, SocketFlags.None, (byte*)&addressStorage, &socketAddressSize);

            if (num >= 0 && socketAddress != null)
            {
                sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;
                *socketAddress = *__socketAddress_native;
            }

            return num;
        }

        /// <summary>
        ///     Receives data from an Ipv6 endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv6(nint socket, void* buffer, int length, sockaddr_in6* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            int socketAddressSize = sizeof(sockaddr_storage);

            int num = _recvfrom(socket, (byte*)buffer, length, SocketFlags.None, (byte*)&addressStorage, &socketAddressSize);

            if (num >= 0 && socketAddress != null)
            {
                if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V4)
                {
                    sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;

                    socketAddress->sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;
                    socketAddress->sin6_port = addressStorage.ss_port;
                    socketAddress->sin6_flowinfo = 0;
                    WinSock2.MapToIpv6(ref Unsafe.AsRef<byte>(socketAddress->sin6_addr), __socketAddress_native->sin4_addr);
                    socketAddress->sin6_scope_id = 0;
                }
                else if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V6)
                {
                    sockaddr_in6* __socketAddress_native = (sockaddr_in6*)&addressStorage;

                    *socketAddress = *__socketAddress_native;
                }
            }

            return num;
        }

        /// <summary>
        ///     Sends a message (scatter/gather) on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessage(nint socket, NativeIoSlice* buffers, int bufferCount)
        {
            uint bytesTransferred;
            SocketError error;

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSASend(socket, __buffers_native.Buffer, (uint)bufferCount, &bytesTransferred, SocketFlags.None, null, 0);
            }

            return error == SocketError.Success ? (int)bytesTransferred : -1;
        }

        /// <summary>
        ///     Sends a message to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, sockaddr_in4* socketAddress)
        {
            if (socketAddress != null)
            {
                uint bytesTransferred;
                SocketError error;

                using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
                {
                    error = _WSASendTo(socket, __buffers_native.Buffer, (uint)bufferCount, &bytesTransferred, SocketFlags.None, (byte*)socketAddress, sizeof(sockaddr_in4), null, 0);
                }

                return error == SocketError.Success ? (int)bytesTransferred : -1;
            }

            return SendMessage(socket, buffers, bufferCount);
        }

        /// <summary>
        ///     Sends a message to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, sockaddr_in6* socketAddress)
        {
            if (socketAddress != null)
            {
                uint bytesTransferred;
                SocketError error;

                using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
                {
                    error = _WSASendTo(socket, __buffers_native.Buffer, (uint)bufferCount, &bytesTransferred, SocketFlags.None, (byte*)socketAddress, sizeof(sockaddr_in6), null, 0);
                }

                return error == SocketError.Success ? (int)bytesTransferred : -1;
            }

            return SendMessage(socket, buffers, bufferCount);
        }

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessage(nint socket, NativeIoSlice* buffers, int bufferCount)
        {
            uint bytesTransferred;
            SocketFlags socketFlags;
            SocketError error;

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSARecv(socket, __buffers_native.Buffer, (uint)bufferCount, &bytesTransferred, &socketFlags, null, 0);
            }

            if (socketFlags != 0)
                return -1;

            return error == SocketError.Success ? (int)bytesTransferred : -1;
        }

        /// <summary>
        ///     Receives a message from an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, sockaddr_in4* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            int socketAddressSize = sizeof(sockaddr_storage);

            uint bytesTransferred;
            SocketFlags socketFlags;
            SocketError error;

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSARecvFrom(socket, __buffers_native.Buffer, (uint)bufferCount, &bytesTransferred, &socketFlags, (byte*)&addressStorage, &socketAddressSize, null, 0);
            }

            if (socketFlags != 0)
                return -1;

            if (error == SocketError.Success && socketAddress != null)
            {
                sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;
                *socketAddress = *__socketAddress_native;
            }

            return error == SocketError.Success ? (int)bytesTransferred : -1;
        }

        /// <summary>
        ///     Receives a message from an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, sockaddr_in6* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            int socketAddressSize = sizeof(sockaddr_storage);

            uint bytesTransferred;
            SocketFlags socketFlags;
            SocketError error;

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSARecvFrom(socket, __buffers_native.Buffer, (uint)bufferCount, &bytesTransferred, &socketFlags, (byte*)&addressStorage, &socketAddressSize, null, 0);
            }

            if (socketFlags != 0)
                return -1;

            if (error == SocketError.Success && socketAddress != null)
            {
                if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V4)
                {
                    sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;

                    socketAddress->sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;
                    socketAddress->sin6_port = addressStorage.ss_port;
                    socketAddress->sin6_flowinfo = 0;
                    WinSock2.MapToIpv6(ref Unsafe.AsRef<byte>(socketAddress->sin6_addr), __socketAddress_native->sin4_addr);
                    socketAddress->sin6_scope_id = 0;
                }
                else if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V6)
                {
                    sockaddr_in6* __socketAddress_native = (sockaddr_in6*)&addressStorage;

                    *socketAddress = *__socketAddress_native;
                }
            }

            return error == SocketError.Success ? (int)bytesTransferred : -1;
        }

        /// <summary>
        ///     Gets the local name (address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            int socketAddressSize = sizeof(sockaddr_storage);

            SocketError error = _getsockname(socket, (sockaddr*)&addressStorage, &socketAddressSize);

            if (error == SocketError.Success)
            {
                sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;
                *socketAddress = *__socketAddress_native;
            }

            return error;
        }

        /// <summary>
        ///     Gets the local name (address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            int socketAddressSize = sizeof(sockaddr_storage);

            SocketError error = _getsockname(socket, (sockaddr*)&addressStorage, &socketAddressSize);

            if (error == SocketError.Success && socketAddress != null)
            {
                if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V4)
                {
                    sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;

                    socketAddress->sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;
                    socketAddress->sin6_port = addressStorage.ss_port;
                    socketAddress->sin6_flowinfo = 0;
                    WinSock2.MapToIpv6(ref Unsafe.AsRef<byte>(socketAddress->sin6_addr), __socketAddress_native->sin4_addr);
                    socketAddress->sin6_scope_id = 0;
                }
                else if (addressStorage.ss_family == ADDRESS_FAMILY_INTER_NETWORK_V6)
                {
                    sockaddr_in6* __socketAddress_native = (sockaddr_in6*)&addressStorage;

                    *socketAddress = *__socketAddress_native;
                }
            }

            return error;
        }

        /// <summary>
        ///     Sets the Ipv4 address in the given address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv4(sockaddr_in4* socketAddress, ReadOnlySpan<char> ip)
        {
            sockaddr_in4 __socketAddress_native = *socketAddress;

            void* pAddrBuf = &__socketAddress_native.sin4_addr;
            const int addressFamily = ADDRESS_FAMILY_INTER_NETWORK_V4;

            int error;

            using (NativeScopedArray<byte> array = WinSock2.GetBytes(stackalloc byte[256], ip))
            {
                byte* buffer = array.Buffer;

                error = _inet_pton(addressFamily, buffer, pAddrBuf);
            }

            switch (error)
            {
                case 1:
                    *socketAddress = __socketAddress_native;
                    return SocketError.Success;

                case 0:
                    return SocketError.InvalidArgument;

                default:
                    return SocketError.Fault;
            }
        }

        /// <summary>
        ///     Sets the Ipv6 address in the given address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv6(sockaddr_in6* socketAddress, ReadOnlySpan<char> ip)
        {
            sockaddr_in6 __socketAddress_native = *socketAddress;

            byte* pAddrBuf = __socketAddress_native.sin6_addr;
            ushort addressFamily = ADDRESS_FAMILY_INTER_NETWORK_V6;
            if (ip.IndexOf(':') < 0)
            {
                addressFamily = ADDRESS_FAMILY_INTER_NETWORK_V4;
                WinSock2.MapToIpv6(ref Unsafe.AsRef<byte>(pAddrBuf));
                pAddrBuf += 12;
            }

            int error;

            using (NativeScopedArray<byte> array = WinSock2.GetBytes(stackalloc byte[256], ip))
            {
                byte* buffer = array.Buffer;

                error = _inet_pton(addressFamily, buffer, pAddrBuf);
            }

            switch (error)
            {
                case 1:
                    *socketAddress = __socketAddress_native;
                    return SocketError.Success;

                case 0:
                    return SocketError.InvalidArgument;

                default:
                    return SocketError.Fault;
            }
        }

        /// <summary>
        ///     Retrieves the Ipv4 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIpIpv4(sockaddr_in4* socketAddress, Span<byte> ip)
        {
            void* pAddrBuf = &socketAddress->sin4_addr;
            const int addressFamily = ADDRESS_FAMILY_INTER_NETWORK_V4;

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(ip))
            {
                if (_inet_ntop(addressFamily, pAddrBuf, pStringBuf, (nuint)ip.Length) == null)
                    return SocketError.Fault;
            }

            return SocketError.Success;
        }

        /// <summary>
        ///     Retrieves the Ipv6 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIpIpv6(sockaddr_in6* socketAddress, Span<byte> ip)
        {
            byte* pAddrBuf = socketAddress->sin6_addr;
            ushort addressFamily = ADDRESS_FAMILY_INTER_NETWORK_V6;
            if (WinSock2.IsIpv4MappedToIpv6(ref Unsafe.AsRef<byte>(pAddrBuf)))
            {
                addressFamily = ADDRESS_FAMILY_INTER_NETWORK_V4;
                pAddrBuf += 12;
            }

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(ip))
            {
                if (_inet_ntop(addressFamily, pAddrBuf, pStringBuf, (nuint)ip.Length) == null)
                {
                    return SocketError.Fault;
                }
            }

            return SocketError.Success;
        }

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">The host name as a span of characters.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv4(sockaddr_in4* socketAddress, ReadOnlySpan<char> hostName)
        {
            addrinfo addressInfo = new addrinfo();
            addressInfo.ai_family = ADDRESS_FAMILY_INTER_NETWORK_V4;
            addrinfo* results = null;

            using (NativeScopedArray<byte> array = WinSock2.GetBytes(stackalloc byte[256], hostName))
            {
                byte* buffer = array.Buffer;

                if (_getaddrinfo(buffer, null, &addressInfo, &results) != 0)
                    return SocketError.Fault;
            }

            for (addrinfo* hint = results; hint != null; hint = hint->ai_next)
            {
                if (hint->ai_addr != null && hint->ai_addrlen >= (nuint)sizeof(sockaddr_in4))
                {
                    if (hint->ai_family == ADDRESS_FAMILY_INTER_NETWORK_V4)
                    {
                        sockaddr_in4* __socketAddress_native = (sockaddr_in4*)hint->ai_addr;

                        socketAddress->sin4_addr = __socketAddress_native->sin4_addr;

                        _freeaddrinfo(results);

                        return SocketError.Success;
                    }
                }
            }

            if (results != null)
                _freeaddrinfo(results);

            return SetIpIpv4(socketAddress, hostName);
        }

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">The host name as a span of characters.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv6(sockaddr_in6* socketAddress, ReadOnlySpan<char> hostName)
        {
            addrinfo addressInfo = new addrinfo();
            addressInfo.ai_family = ADDRESS_FAMILY_INTER_NETWORK_V6;
            addrinfo* results = null;

            using (NativeScopedArray<byte> array = WinSock2.GetBytes(stackalloc byte[256], hostName))
            {
                byte* buffer = array.Buffer;

                if (_getaddrinfo(buffer, null, &addressInfo, &results) != 0)
                    return SocketError.Fault;
            }

            for (addrinfo* hint = results; hint != null; hint = hint->ai_next)
            {
                if (hint->ai_addr != null && hint->ai_addrlen >= (nuint)sizeof(sockaddr_in6))
                {
                    if (hint->ai_family == ADDRESS_FAMILY_INTER_NETWORK_V6)
                    {
                        sockaddr_in6* __socketAddress_native = (sockaddr_in6*)hint->ai_addr;

                        SpanHelpers.Copy(socketAddress->sin6_addr, __socketAddress_native->sin6_addr, 16);

                        _freeaddrinfo(results);

                        return SocketError.Success;
                    }
                }
            }

            if (results != null)
                _freeaddrinfo(results);

            return SetIpIpv6(socketAddress, hostName);
        }

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostNameIpv4(sockaddr_in4* socketAddress, Span<byte> hostName)
        {
            int error;
            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(hostName))
            {
                error = _getnameinfo((sockaddr*)socketAddress, sizeof(sockaddr_in4), pStringBuf, (ulong)hostName.Length, null, 0, 0x4);
            }

            if (error == 0)
            {
                if (hostName.IndexOf((byte)'\0') < 0)
                    return SocketError.Fault;

                return SocketError.Success;
            }

            if (error != DNS_TRY_AGAIN)
                return SocketError.Fault;

            return GetIpIpv4(socketAddress, hostName);
        }

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostNameIpv6(sockaddr_in6* socketAddress, Span<byte> hostName)
        {
            int error;
            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(hostName))
            {
                error = _getnameinfo((sockaddr*)socketAddress, sizeof(sockaddr_in6), pStringBuf, (ulong)hostName.Length, null, 0, 0x4);
            }

            if (error == 0)
            {
                if (hostName.IndexOf((byte)'\0') < 0)
                    return SocketError.Fault;

                return SocketError.Success;
            }

            if (error != DNS_TRY_AGAIN)
                return SocketError.Fault;

            return GetIpIpv6(socketAddress, hostName);
        }
    }
}