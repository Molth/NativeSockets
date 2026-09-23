#if NET5_0_OR_GREATER
using System;
#else
using System.Runtime.InteropServices;
#endif
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using static NativeSockets.WindowsSocketLib;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations.
    /// </summary>
    internal static unsafe class WindowsSocketPal
    {
        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V4 = AF_INET_4;

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V6 = AF_INET_6;

        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        private const ushort AF_INET_4 = 2;

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        private const ushort AF_INET_6 = 23;

        /// <summary>
        ///     Gets a value indicating whether any platform-specific implementation is supported.
        /// </summary>
        public static bool IsSupported { get; } =
#if NET5_0_OR_GREATER
            OperatingSystem.IsWindows();
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError() => (SocketError)_WSAGetLastError();

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Startup()
        {
            Unsafe.SkipInit(out WSAData wsaData);
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
        /// <param name="socket">When this method returns, contains the native socket handle, or -1 on error.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Create(bool ipv6, out nint socket)
        {
            ushort family = ipv6 ? AF_INET_6 : AF_INET_4;
            socket = _WSASocketW((AddressFamily)family, SocketType.Dgram, ProtocolType.Udp, 0, 0, 1 | 128);

            if (socket != -1)
            {
                const uint IOC_IN = 0x80000000;
                const uint IOC_VENDOR = 0x18000000;
                const uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                byte bNewBehavior = 0;
                int __bytesTransferred_native = 0;
                _WSAIoctl(socket, unchecked((int)SIO_UDP_CONNRESET), &bNewBehavior, 1, null, 0, &__bytesTransferred_native, 0, 0);
            }

            return socket == -1 ? GetLastSocketError() : SocketError.Success;
        }

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(nint socket)
        {
            SocketError error = _closesocket(socket);
            return error == 0 ? SocketError.Success : GetLastSocketError();
        }

        /// <summary>
        ///     Binds a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            SocketError error;

            if (socketAddress != null)
            {
                error = _bind(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in4));
            }
            else
            {
                sockaddr_in4 __socketAddress_native = new sockaddr_in4();
                __socketAddress_native.sin4_family = ADDRESS_FAMILY_INTER_NETWORK_V4;

                error = _bind(socket, (sockaddr*)&__socketAddress_native, sizeof(sockaddr_in4));
            }

            return error == 0 ? SocketError.Success : GetLastSocketError();
        }

        /// <summary>
        ///     Binds a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            SocketError error;

            if (socketAddress != null)
            {
                error = _bind(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in6));
            }
            else
            {
                sockaddr_in6 __socketAddress_native = new sockaddr_in6();
                __socketAddress_native.sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;

                error = _bind(socket, (sockaddr*)&__socketAddress_native, sizeof(sockaddr_in6));
            }

            return error == 0 ? SocketError.Success : GetLastSocketError();
        }

        /// <summary>
        ///     Connects a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            SocketError error = _WSAConnect(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in4), 0, 0, 0, 0);
            return error == 0 ? SocketError.Success : GetLastSocketError();
        }

        /// <summary>
        ///     Connects a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            SocketError error = _WSAConnect(socket, (sockaddr*)socketAddress, sizeof(sockaddr_in6), 0, 0, 0, 0);
            return error == 0 ? SocketError.Success : GetLastSocketError();
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
        /// <remarks>
        ///     The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///     platform equivalents by the underlying socket layer. The <paramref name="value" /> bytes are
        ///     passed through unmodified; the platform interprets the buffer according to the mapped option.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int length)
        {
            SocketError error = _setsockopt(socket, level, name, value, length);
            return error == 0 ? SocketError.Success : GetLastSocketError();
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
        /// <remarks>
        ///     The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///     platform equivalents by the underlying socket layer. The <paramref name="value" /> buffer is
        ///     passed through unmodified; the platform populates the buffer according to the mapped option.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int* length)
        {
            SocketError error = _getsockopt(socket, level, name, value, length);
            return error == 0 ? SocketError.Success : GetLastSocketError();
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
        public static SocketError SetRawOption(nint socket, int level, int name, byte* value, int length) => SetOption(socket, (SocketOptionLevel)level, (SocketOptionName)name, value, length);

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
        public static SocketError GetRawOption(nint socket, int level, int name, byte* value, int* length) => GetOption(socket, (SocketOptionLevel)level, (SocketOptionName)name, value, length);

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(nint socket, bool blocking)
        {
            int intBlocking = blocking ? 0 : 1;
            SocketError error = _ioctlsocket(socket, unchecked((int)0x8004667E), &intBlocking);
            return error == 0 ? SocketError.Success : GetLastSocketError();
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
            nint* fds = stackalloc nint[2];
            fds[0] = 1;
            fds[1] = socket;

            int socketCount;
            if (microseconds != -1)
            {
                TimeValue timeout = new TimeValue();
                MicrosecondsToTimeValue(microseconds, ref timeout);

                socketCount = _select(0, mode == SelectMode.SelectRead ? fds : null, mode == SelectMode.SelectWrite ? fds : null, mode == SelectMode.SelectError ? fds : null, &timeout);
            }
            else
            {
                socketCount = _select(0, mode == SelectMode.SelectRead ? fds : null, mode == SelectMode.SelectWrite ? fds : null, mode == SelectMode.SelectError ? fds : null, null);
            }

            if (socketCount == -1)
            {
                status = false;
                return GetLastSocketError();
            }

            status = FD_ISSET(socket, fds);

            return SocketError.Success;
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(nint socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags)
        {
            nint* readFds = stackalloc nint[2];
            nint* writeFds = stackalloc nint[2];
            nint* errorFds = stackalloc nint[2];

            if ((inFlags & SelectModeFlags.SelectRead) == 0)
            {
                readFds = null;
            }
            else
            {
                readFds[0] = 1;
                readFds[1] = socket;
            }

            if ((inFlags & SelectModeFlags.SelectWrite) == 0)
            {
                writeFds = null;
            }
            else
            {
                writeFds[0] = 1;
                writeFds[1] = socket;
            }

            if ((inFlags & SelectModeFlags.SelectError) == 0)
            {
                errorFds = null;
            }
            else
            {
                errorFds[0] = 1;
                errorFds[1] = socket;
            }

            int socketCount;
            if (microseconds != -1)
            {
                TimeValue timeout = new TimeValue();
                MicrosecondsToTimeValue(microseconds, ref timeout);

                socketCount = _select(0, readFds, writeFds, errorFds, &timeout);
            }
            else
            {
                socketCount = _select(0, readFds, writeFds, errorFds, null);
            }

            outFlags = 0;

            if (socketCount == -1)
                return GetLastSocketError();

            if (readFds != null && FD_ISSET(socket, readFds))
                outFlags |= SelectModeFlags.SelectRead;

            if (writeFds != null && FD_ISSET(socket, writeFds))
                outFlags |= SelectModeFlags.SelectWrite;

            if (errorFds != null && FD_ISSET(socket, errorFds))
                outFlags |= SelectModeFlags.SelectError;

            return SocketError.Success;
        }

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer in bytes.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Send(nint socket, void* buffer, int length, SocketFlags socketFlags)
        {
            int num = _send(socket, (byte*)buffer, length, socketFlags);
            return num;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            if (socketAddress != null)
                return _sendto(socket, (byte*)buffer, length, socketFlags, (byte*)socketAddress, sizeof(sockaddr_in4));

            int num = Send(socket, (byte*)buffer, length, socketFlags);
            return num;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            if (socketAddress != null)
                return _sendto(socket, (byte*)buffer, length, socketFlags, (byte*)socketAddress, sizeof(sockaddr_in6));

            int num = Send(socket, (byte*)buffer, length, socketFlags);
            return num;
        }

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Receive(nint socket, void* buffer, int length, SocketFlags socketFlags)
        {
            int num = _recv(socket, (byte*)buffer, length, socketFlags);
            return num;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            Unsafe.SkipInit(out sockaddr_in4 storage);
            int socketAddressSize = sizeof(sockaddr_in4);

            int num = _recvfrom(socket, (byte*)buffer, length, socketFlags, (byte*)&storage, &socketAddressSize);

            if (num >= 0 && socketAddress != null)
                *socketAddress = storage;

            return num;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            Unsafe.SkipInit(out sockaddr_in6 storage);
            int socketAddressSize = sizeof(sockaddr_in6);

            int num = _recvfrom(socket, (byte*)buffer, length, socketFlags, (byte*)&storage, &socketAddressSize);

            if (num >= 0 && socketAddress != null)
                *socketAddress = storage;

            return num;
        }

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags)
        {
            Unsafe.SkipInit(out int bytesTransferred);
            SocketError error;

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSASend(socket, __buffers_native.Buffer, bufferCount, &bytesTransferred, socketFlags, null, 0);
            }

            return error == SocketError.Success ? bytesTransferred : -1;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            if (socketAddress != null)
            {
                Unsafe.SkipInit(out int bytesTransferred);
                SocketError error;

                using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
                {
                    error = _WSASendTo(socket, __buffers_native.Buffer, bufferCount, &bytesTransferred, socketFlags, (byte*)socketAddress, sizeof(sockaddr_in4), null, 0);
                }

                return error == SocketError.Success ? bytesTransferred : -1;
            }

            return SendVectored(socket, buffers, bufferCount, socketFlags);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            if (socketAddress != null)
            {
                Unsafe.SkipInit(out int bytesTransferred);
                SocketError error;

                using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
                {
                    error = _WSASendTo(socket, __buffers_native.Buffer, bufferCount, &bytesTransferred, socketFlags, (byte*)socketAddress, sizeof(sockaddr_in6), null, 0);
                }

                return error == SocketError.Success ? bytesTransferred : -1;
            }

            return SendVectored(socket, buffers, bufferCount, socketFlags);
        }

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
        public static int ReceiveVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags)
        {
            Unsafe.SkipInit(out int bytesTransferred);
            SocketFlags flags = inOutFlags != null ? *inOutFlags : 0;
            SocketError error;

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSARecv(socket, __buffers_native.Buffer, bufferCount, &bytesTransferred, &flags, null, 0);
            }

            if (inOutFlags != null)
                *inOutFlags = flags;

            if (flags != 0)
                return -1;

            return error == SocketError.Success ? bytesTransferred : -1;
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
        /// <remarks>
        ///     If the <c>inOutFlags</c> returned by the receive operation is not equal to <c>0</c>,
        ///     the operation is considered failed and returns <c>-1</c>,
        ///     even if <c>GetLastSocketError</c> returns <see cref="SocketError.Success" />.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in4* socketAddress)
        {
            Unsafe.SkipInit(out int bytesTransferred);
            SocketFlags flags = inOutFlags != null ? *inOutFlags : 0;
            SocketError error;

            Unsafe.SkipInit(out sockaddr_in4 storage);
            int socketAddressSize = sizeof(sockaddr_in4);

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSARecvFrom(socket, __buffers_native.Buffer, bufferCount, &bytesTransferred, &flags, (byte*)&storage, &socketAddressSize, null, 0);
            }

            if (inOutFlags != null)
                *inOutFlags = flags;

            if (flags != 0)
                return -1;

            if (error == SocketError.Success && socketAddress != null)
                *socketAddress = storage;

            return error == SocketError.Success ? bytesTransferred : -1;
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
        /// <remarks>
        ///     If the <c>inOutFlags</c> returned by the receive operation is not equal to <c>0</c>,
        ///     the operation is considered failed and returns <c>-1</c>,
        ///     even if <c>GetLastSocketError</c> returns <see cref="SocketError.Success" />.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in6* socketAddress)
        {
            Unsafe.SkipInit(out int bytesTransferred);
            SocketFlags flags = inOutFlags != null ? *inOutFlags : 0;
            SocketError error;

            Unsafe.SkipInit(out sockaddr_in6 storage);
            int socketAddressSize = sizeof(sockaddr_in6);

            using (NativeScopedArray<WSABuffer> __buffers_native = Build(stackalloc WSABuffer[16], buffers, bufferCount))
            {
                error = _WSARecvFrom(socket, __buffers_native.Buffer, bufferCount, &bytesTransferred, &flags, (byte*)&storage, &socketAddressSize, null, 0);
            }

            if (inOutFlags != null)
                *inOutFlags = flags;

            if (flags != 0)
                return -1;

            if (error == SocketError.Success && socketAddress != null)
                *socketAddress = storage;

            return error == SocketError.Success ? bytesTransferred : -1;
        }

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            Unsafe.SkipInit(out sockaddr_in4 storage);
            int socketAddressSize = sizeof(sockaddr_in4);

            SocketError error = _getsockname(socket, (sockaddr*)&storage, &socketAddressSize);

            if (error == SocketError.Success && socketAddress != null)
                *socketAddress = storage;

            return error == 0 ? SocketError.Success : GetLastSocketError();
        }

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            Unsafe.SkipInit(out sockaddr_in6 storage);
            int socketAddressSize = sizeof(sockaddr_in6);

            SocketError error = _getsockname(socket, (sockaddr*)&storage, &socketAddressSize);

            if (error == SocketError.Success && socketAddress != null)
                *socketAddress = storage;

            return error == 0 ? SocketError.Success : GetLastSocketError();
        }
    }
}