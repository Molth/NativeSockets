#if NET5_0_OR_GREATER
using System;
#endif
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NativeSockets.UnixSocketLib;
using static NativeSockets.BsdSocketLib;
using static NativeSockets.OsxSocketLib;
using static NativeSockets.OsxSocketError;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations.
    /// </summary>
    internal static unsafe class OsxSocketPal
    {
        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V4 = (AF_INET_4 << 8) | 16;

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        public const ushort ADDRESS_FAMILY_INTER_NETWORK_V6 = (AF_INET_6 << 8) | 28;

        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        private const ushort AF_INET_4 = 2;

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        private const ushort AF_INET_6 = 30;

        /// <summary>
        ///     Gets a value indicating whether any platform-specific implementation is supported.
        /// </summary>
        public static bool IsSupported { get; } =
#if NET5_0_OR_GREATER
            OperatingSystem.IsMacOS() ||
            OperatingSystem.IsIOS() ||
            OperatingSystem.IsTvOS() ||
            OperatingSystem.IsWatchOS() ||
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST")) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("TVOS")) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("WATCHOS")) ||
#endif
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("VISIONOS"));

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
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup() => SocketError.Success;

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
            socket = _socket(family, (int)SocketType.Dgram, (int)ProtocolType.Udp);

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
            int errno = _close((int)socket);
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            if (socketAddress == null)
            {
                __socketAddress_native = new sockaddr_in4();
                __socketAddress_native.sin4_family = ADDRESS_FAMILY_INTER_NETWORK_V4;

                socketAddress = &__socketAddress_native;
            }

            int errno = _bind((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in4));
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            if (socketAddress == null)
            {
                __socketAddress_native = new sockaddr_in6();
                __socketAddress_native.sin6_family = ADDRESS_FAMILY_INTER_NETWORK_V6;

                socketAddress = &__socketAddress_native;
            }

            int errno = _bind((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in6));
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
            int errno = _connect((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in4));
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
            int errno = _connect((int)socket, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in6));
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
            int errno = __setsockopt((int)socket, level, name, value, (uint)length);
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
        /// <remarks>
        ///     The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///     platform equivalents by the underlying socket layer. The <paramref name="value" /> buffer is
        ///     passed through unmodified; the platform populates the buffer according to the mapped option.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int* length)
        {
            int errno = __getsockopt((int)socket, level, name, value, (uint*)length);
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
        public static SocketError SetRawOption(nint socket, int level, int name, byte* value, int length)
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
        public static SocketError GetRawOption(nint socket, int level, int name, byte* value, int* length)
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
            int intBlocking = blocking ? 0 : 1;
            int errno = _ioctl((int)socket, FIONBIO, &intBlocking);
            return errno == 0 ? SocketError.Success : GetLastSocketError();
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

            int errno = __poll(&fd, 1, milliseconds);
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
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
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

            int errno = __poll(&fd, 1, milliseconds);
            if (errno == -1)
                return GetLastSocketError();

            PollEvents outEvents = (PollEvents)fd.revents;

            if ((inFlags & SelectModeFlags.SelectRead) != 0 && (outEvents & (PollEvents.POLLIN | PollEvents.POLLHUP)) != 0)
                outFlags |= SelectModeFlags.SelectRead;

            if ((inFlags & SelectModeFlags.SelectWrite) != 0 && (outEvents & PollEvents.POLLOUT) != 0)
                outFlags |= SelectModeFlags.SelectWrite;

            if ((inFlags & SelectModeFlags.SelectError) != 0 && (outEvents & (PollEvents.POLLERR | PollEvents.POLLPRI)) != 0)
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
            int num = (int)__send((int)socket, (byte*)buffer, (nuint)length, socketFlags);
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
                return (int)__sendto((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in4));

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
                return (int)__sendto((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)socketAddress, (uint)sizeof(sockaddr_in6));

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
            int num = (int)__recv((int)socket, (byte*)buffer, (nuint)length, socketFlags);
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
            uint socketAddressSize = (uint)sizeof(sockaddr_in4);

            int num = (int)__recvfrom((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)&storage, &socketAddressSize);

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
            uint socketAddressSize = (uint)sizeof(sockaddr_in6);

            int num = (int)__recvfrom((int)socket, (byte*)buffer, (nuint)length, socketFlags, (sockaddr*)&storage, &socketAddressSize);

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
            msghdr msg = new msghdr();
            msg.msg_iovlen = __get_msg_iovlen(bufferCount);

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)__sendmsg((int)socket, &msg, socketFlags);
            }

            return num;
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
                msghdr msg = new msghdr();
                msg.msg_name = socketAddress;
                msg.msg_namelen = (uint)sizeof(sockaddr_in4);
                msg.msg_iovlen = __get_msg_iovlen(bufferCount);

                int num;

                using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
                {
                    msg.msg_iov = __buffers_native.Buffer;
                    num = (int)__sendmsg((int)socket, &msg, socketFlags);
                }

                return num;
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
                msghdr msg = new msghdr();
                msg.msg_name = socketAddress;
                msg.msg_namelen = (uint)sizeof(sockaddr_in6);
                msg.msg_iovlen = __get_msg_iovlen(bufferCount);

                int num;

                using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
                {
                    msg.msg_iov = __buffers_native.Buffer;
                    num = (int)__sendmsg((int)socket, &msg, socketFlags);
                }

                return num;
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
            msghdr msg = new msghdr();
            msg.msg_iovlen = __get_msg_iovlen(bufferCount);

            SocketFlags flags = inOutFlags != null ? *inOutFlags : 0;

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)__recvmsg((int)socket, &msg, flags);
            }

            if (inOutFlags != null)
                *inOutFlags = (SocketFlags)msg.msg_flags;

            if (msg.msg_flags != 0)
                return -1;

            return num;
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
            Unsafe.SkipInit(out sockaddr_in4 storage);

            msghdr msg = new msghdr();
            msg.msg_name = &storage;
            msg.msg_namelen = (uint)sizeof(sockaddr_in4);
            msg.msg_iovlen = __get_msg_iovlen(bufferCount);

            SocketFlags flags = inOutFlags != null ? *inOutFlags : 0;

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)__recvmsg((int)socket, &msg, flags);
            }

            if (inOutFlags != null)
                *inOutFlags = (SocketFlags)msg.msg_flags;

            if (msg.msg_flags != 0)
                return -1;

            if (num >= 0 && socketAddress != null)
                *socketAddress = storage;

            return num;
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
            Unsafe.SkipInit(out sockaddr_in6 storage);

            msghdr msg = new msghdr();
            msg.msg_name = &storage;
            msg.msg_namelen = (uint)sizeof(sockaddr_in6);
            msg.msg_iovlen = __get_msg_iovlen(bufferCount);

            SocketFlags flags = inOutFlags != null ? *inOutFlags : 0;

            int num;

            using (NativeScopedArray<iovec> __buffers_native = Build(stackalloc iovec[16], buffers, bufferCount))
            {
                msg.msg_iov = __buffers_native.Buffer;
                num = (int)__recvmsg((int)socket, &msg, flags);
            }

            if (inOutFlags != null)
                *inOutFlags = (SocketFlags)msg.msg_flags;

            if (msg.msg_flags != 0)
                return -1;

            if (num >= 0 && socketAddress != null)
                *socketAddress = storage;

            return num;
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
            uint socketAddressSize = (uint)sizeof(sockaddr_in4);

            int errno = _getsockname((int)socket, (sockaddr*)&storage, &socketAddressSize);

            if (errno == 0 && socketAddress != null)
                *socketAddress = storage;

            return errno == 0 ? SocketError.Success : GetLastSocketError();
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
            uint socketAddressSize = (uint)sizeof(sockaddr_in6);

            int errno = _getsockname((int)socket, (sockaddr*)&storage, &socketAddressSize);

            if (errno == 0 && socketAddress != null)
                *socketAddress = storage;

            return errno == 0 ? SocketError.Success : GetLastSocketError();
        }
    }
}