using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NativeSockets.UnixNativeLib;
using static NativeSockets.LinuxNativeLib;
using static NativeSockets.LinuxSocketError;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations for sending and receiving data.
    /// </summary>
    internal static unsafe class LinuxSocketPal
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
        private const ushort AF_INET_6 = 10;

        /// <summary>
        ///     Gets a value indicating whether any platform-specific implementation is supported.
        /// </summary>
        public static bool IsSupported { get; } =
#if NET5_0_OR_GREATER
            OperatingSystem.IsLinux() ||
            OperatingSystem.IsAndroid();
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
#endif

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError() => GetLastError();

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Startup() => SocketError.Success;

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup() => SocketError.Success;

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <returns>The native socket handle, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint Create(bool ipv6)
        {
            ushort family = ipv6 ? AF_INET_6 : AF_INET_4;
            nint socket = _socket(family, (int)SocketType.Dgram, (int)ProtocolType.Udp);

            return socket;
        }

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(nint socket)
        {
            int errno = _close((int)socket);
            return (SocketError)errno;
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
            SocketError error = SetOption(socket, SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, (byte*)&optionValue, 4);
            return error;
        }

        /// <summary>
        ///     Binds a socket to an Ipv4 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            if (socketAddress == null)
            {
                __socketAddress_native = new sockaddr_in4();
                __socketAddress_native.sin4_family = ADDRESS_FAMILY_INTER_NETWORK_V4;

                socketAddress = &__socketAddress_native;
            }

            int errno = _bind((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in4));
            return (SocketError)errno;
        }

        /// <summary>
        ///     Binds a socket to an Ipv6 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            if (socketAddress == null)
            {
                __socketAddress_native = new sockaddr_in6();
                __socketAddress_native.sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;

                socketAddress = &__socketAddress_native;
            }

            int errno = _bind((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in6));
            return (SocketError)errno;
        }

        /// <summary>
        ///     Connects a socket to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            int errno = _connect((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in4));
            return (SocketError)errno;
        }

        /// <summary>
        ///     Connects a socket to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError ConnectIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            int errno = _connect((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in6));
            return (SocketError)errno;
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
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int length)
        {
            int errno = _setsockopt((int)socket, level, name, value, (uint)length);
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int* length)
        {
            int errno = _getsockopt((int)socket, level, name, value, (uint*)length);
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
            int flags = _fcntl((int)socket, F_GETFL, 0);
            if (flags == -1)
                return GetLastSocketError();

            flags = blocking ? flags & ~O_NONBLOCK : flags | O_NONBLOCK;
            if (_fcntl((int)socket, F_SETFL, flags) == -1)
                return GetLastSocketError();

            return SocketError.Success;
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
            PollEvents inEvent = 0;
            switch (mode)
            {
                case SelectMode.SelectRead:
                    inEvent = PollEvents.POLLIN;
                    break;
                case SelectMode.SelectWrite:
                    inEvent = PollEvents.POLLOUT;
                    break;
                case SelectMode.SelectError:
                    inEvent = PollEvents.POLLPRI;
                    break;
            }

            int milliseconds = microseconds == -1 ? -1 : microseconds / 1000;

            pollfd fd;
            fd.fd = (int)socket;
            fd.events = (short)inEvent;
            fd.revents = 0;

            int errno = _poll(&fd, 1, milliseconds);
            if (errno == -1)
            {
                status = false;
                return GetLastSocketError();
            }

            PollEvents outEvents = (PollEvents)fd.revents;
            switch (mode)
            {
                case SelectMode.SelectRead:
                    status = (outEvents & (PollEvents.POLLIN | PollEvents.POLLHUP)) != 0;
                    break;
                case SelectMode.SelectWrite:
                    status = (outEvents & PollEvents.POLLOUT) != 0;
                    break;
                case SelectMode.SelectError:
                    status = (outEvents & (PollEvents.POLLERR | PollEvents.POLLPRI)) != 0;
                    break;
                default:
                    status = false;
                    break;
            }

            return SocketError.Success;
        }

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(nint socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags)
        {
            PollEvents inEvent = 0;

            if ((inFlags & SelectModeFlags.SelectRead) != 0)
                inEvent |= PollEvents.POLLIN;

            if ((inFlags & SelectModeFlags.SelectWrite) != 0)
                inEvent |= PollEvents.POLLOUT;

            if ((inFlags & SelectModeFlags.SelectError) != 0)
                inEvent |= PollEvents.POLLPRI;

            int milliseconds = microseconds == -1 ? -1 : microseconds / 1000;

            pollfd fd;
            fd.fd = (int)socket;
            fd.events = (short)inEvent;
            fd.revents = 0;

            outFlags = 0;

            int errno = _poll(&fd, 1, milliseconds);
            if (errno == -1)
                return GetLastSocketError();

            PollEvents outEvents = (PollEvents)fd.revents;

            if ((outEvents & (PollEvents.POLLIN | PollEvents.POLLHUP)) != 0)
                outFlags |= SelectModeFlags.SelectRead;

            if ((outEvents & PollEvents.POLLOUT) != 0)
                outFlags |= SelectModeFlags.SelectWrite;

            if ((outEvents & (PollEvents.POLLERR | PollEvents.POLLPRI)) != 0)
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
            int num = (int)_send((int)socket, (byte*)buffer, (nuint)length, socketFlags);
            return num;
        }

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
        public static int SendToIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            if (socketAddress != null)
                return (int)_sendto((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in4));

            int num = Send(socket, (byte*)buffer, length, socketFlags);
            return num;
        }

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
        public static int SendToIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            if (socketAddress != null)
                return (int)_sendto((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in6));

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
            int num = (int)_recv((int)socket, (byte*)buffer, (nuint)length, socketFlags);
            return num;
        }

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
        public static int ReceiveFromIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            uint socketAddressSize = (uint)sizeof(sockaddr_storage);

            int num = (int)_recvfrom((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)&addressStorage, &socketAddressSize);

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
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            uint socketAddressSize = (uint)sizeof(sockaddr_storage);

            int num = (int)_recvfrom((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)&addressStorage, &socketAddressSize);

            if (num >= 0 && socketAddress != null)
                WinSock2.NormalizeToIpv6(socketAddress, addressStorage, ADDRESS_FAMILY_INTER_NETWORK_V4, ADDRESS_FAMILY_INTER_NETWORK_V6);

            return num;
        }

        /// <summary>
        ///     Sends a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessage(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags)
        {
            msghdr msg = new msghdr();
            msg.msg_iovlen = _get_msg_iovlen(bufferCount);

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)_sendmsg((int)socket, &msg, socketFlags);
            }

            return num;
        }

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
        public static int SendMessageToIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in4* socketAddress)
        {
            if (socketAddress != null)
            {
                msghdr msg = new msghdr();
                msg.msg_name = socketAddress;
                msg.msg_namelen = (uint)sizeof(sockaddr_in4);
                msg.msg_iovlen = _get_msg_iovlen(bufferCount);

                int num;

                using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
                {
                    msg.msg_iov = __buffers_native.Buffer;
                    num = (int)_sendmsg((int)socket, &msg, socketFlags);
                }

                return num;
            }

            return SendMessage(socket, buffers, bufferCount, socketFlags);
        }

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
        public static int SendMessageToIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in6* socketAddress)
        {
            if (socketAddress != null)
            {
                msghdr msg = new msghdr();
                msg.msg_name = socketAddress;
                msg.msg_namelen = (uint)sizeof(sockaddr_in6);
                msg.msg_iovlen = _get_msg_iovlen(bufferCount);

                int num;

                using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
                {
                    msg.msg_iov = __buffers_native.Buffer;
                    num = (int)_sendmsg((int)socket, &msg, socketFlags);
                }

                return num;
            }

            return SendMessage(socket, buffers, bufferCount, socketFlags);
        }

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessage(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags)
        {
            msghdr msg = new msghdr();
            msg.msg_iovlen = _get_msg_iovlen(bufferCount);

            SocketFlags flags = socketFlags != null ? *socketFlags : 0;

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)_recvmsg((int)socket, &msg, flags);
            }

            if (socketFlags != null)
                *socketFlags = (SocketFlags)msg.msg_flags;

            if (msg.msg_flags != 0)
                return -1;

            return num;
        }

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
        public static int ReceiveMessageFromIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags, sockaddr_in4* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();

            msghdr msg = new msghdr();
            msg.msg_name = &addressStorage;
            msg.msg_namelen = (uint)sizeof(sockaddr_storage);
            msg.msg_iovlen = _get_msg_iovlen(bufferCount);

            SocketFlags flags = socketFlags != null ? *socketFlags : 0;

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)_recvmsg((int)socket, &msg, flags);
            }

            if (socketFlags != null)
                *socketFlags = (SocketFlags)msg.msg_flags;

            if (msg.msg_flags != 0)
                return -1;

            if (num >= 0 && socketAddress != null)
            {
                sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;
                *socketAddress = *__socketAddress_native;
            }

            return num;
        }

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
        public static int ReceiveMessageFromIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags, sockaddr_in6* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();

            msghdr msg = new msghdr();
            msg.msg_name = &addressStorage;
            msg.msg_namelen = (uint)sizeof(sockaddr_storage);
            msg.msg_iovlen = _get_msg_iovlen(bufferCount);

            SocketFlags flags = socketFlags != null ? *socketFlags : 0;

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)_recvmsg((int)socket, &msg, flags);
            }

            if (socketFlags != null)
                *socketFlags = (SocketFlags)msg.msg_flags;

            if (msg.msg_flags != 0)
                return -1;

            if (num >= 0 && socketAddress != null)
                WinSock2.NormalizeToIpv6(socketAddress, addressStorage, ADDRESS_FAMILY_INTER_NETWORK_V4, ADDRESS_FAMILY_INTER_NETWORK_V6);

            return num;
        }

        /// <summary>
        ///     Gets the local name (address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            uint socketAddressSize = (uint)sizeof(sockaddr_storage);

            int errno = _getsockname((int)socket, (sockaddr*)&addressStorage, &socketAddressSize);

            if (errno == 0 && socketAddress != null)
            {
                sockaddr_in4* __socketAddress_native = (sockaddr_in4*)&addressStorage;
                *socketAddress = *__socketAddress_native;
            }

            return (SocketError)errno;
        }

        /// <summary>
        ///     Gets the local name (address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress)
        {
            sockaddr_storage addressStorage = new sockaddr_storage();
            uint socketAddressSize = (uint)sizeof(sockaddr_storage);

            int errno = _getsockname((int)socket, (sockaddr*)&addressStorage, &socketAddressSize);

            if (errno == 0 && socketAddress != null)
                WinSock2.NormalizeToIpv6(socketAddress, addressStorage, ADDRESS_FAMILY_INTER_NETWORK_V4, ADDRESS_FAMILY_INTER_NETWORK_V6);

            return (SocketError)errno;
        }

        /// <summary>
        ///     Sets the Ipv4 address in the given address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">The ip address as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv4(sockaddr_in4* socketAddress, ReadOnlySpan<byte> ip)
        {
            sockaddr_in4 __socketAddress_native = *socketAddress;

            void* pAddrBuf = &__socketAddress_native.sin4_addr;
            const int addressFamily = AF_INET_4;

            int error;

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(ip))
            {
                error = _inet_pton(addressFamily, pStringBuf, pAddrBuf);
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
        /// <param name="ip">The ip address as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv6(sockaddr_in6* socketAddress, ReadOnlySpan<byte> ip)
        {
            sockaddr_in6 __socketAddress_native = *socketAddress;

            byte* pAddrBuf = __socketAddress_native.sin6_addr;
            ushort addressFamily = AF_INET_6;
            if (ip.IndexOf((byte)':') < 0)
            {
                addressFamily = AF_INET_4;
                WinSock2.WriteIpv6Prefix(ref Unsafe.AsRef<byte>(pAddrBuf));
                pAddrBuf += 12;
            }

            int error;

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(ip))
            {
                error = _inet_pton(addressFamily, pStringBuf, pAddrBuf);
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
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIpIpv4(sockaddr_in4* socketAddress, Span<byte> ip)
        {
            void* pAddrBuf = &socketAddress->sin4_addr;
            const int addressFamily = AF_INET_4;

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(ip))
            {
                if (_inet_ntop(addressFamily, pAddrBuf, pStringBuf, (uint)ip.Length) == null)
                    return SocketError.Fault;
            }

            return SocketError.Success;
        }

        /// <summary>
        ///     Retrieves the Ipv6 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIpIpv6(sockaddr_in6* socketAddress, Span<byte> ip)
        {
            byte* pAddrBuf = socketAddress->sin6_addr;
            const int addressFamily = AF_INET_6;

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(ip))
            {
                if (_inet_ntop(addressFamily, pAddrBuf, pStringBuf, (uint)ip.Length) == null)
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
        /// <param name="hostName">The host name as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv4(sockaddr_in4* socketAddress, ReadOnlySpan<byte> hostName)
        {
            addrinfo hints = new addrinfo();
            hints.ai_family = AF_INET_4;
            addrinfo* results = null;

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(hostName))
            {
                if (_getaddrinfo(pStringBuf, null, &hints, &results) != 0)
                    return SocketError.Fault;
            }

            for (addrinfo* hint = results; hint != null; hint = hint->ai_next)
            {
                if (hint->ai_addr != null && hint->ai_addrlen >= (nuint)sizeof(sockaddr_in4))
                {
                    if (hint->ai_family == AF_INET_4)
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

            return SocketError.HostNotFound;
        }

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">The host name as a span of bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv6(sockaddr_in6* socketAddress, ReadOnlySpan<byte> hostName)
        {
            addrinfo hints = new addrinfo();
            hints.ai_family = AF_INET_6;
            addrinfo* results = null;

            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(hostName))
            {
                if (_getaddrinfo(pStringBuf, null, &hints, &results) != 0)
                    return SocketError.Fault;
            }

            for (addrinfo* hint = results; hint != null; hint = hint->ai_next)
            {
                if (hint->ai_addr != null && hint->ai_addrlen >= (nuint)sizeof(sockaddr_in6))
                {
                    if (hint->ai_family == AF_INET_6)
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

            return SocketError.HostNotFound;
        }

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostNameIpv4(sockaddr_in4* socketAddress, Span<byte> hostName)
        {
            int error;
            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(hostName))
            {
                error = _getnameinfo((sockaddr*)socketAddress, (uint)sizeof(sockaddr_in4), pStringBuf, (uint)hostName.Length, null, 0, 0);
            }

            return error == 0 ? SocketError.Success : SocketError.Fault;
        }

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostNameIpv6(sockaddr_in6* socketAddress, Span<byte> hostName)
        {
            int error;
            fixed (byte* pStringBuf = &MemoryMarshal.GetReference(hostName))
            {
                error = _getnameinfo((sockaddr*)socketAddress, (uint)sizeof(sockaddr_in6), pStringBuf, (uint)hostName.Length, null, 0, 0);
            }

            return error == 0 ? SocketError.Success : SocketError.Fault;
        }
    }
}