#ifndef _WIN32

/// <summary>
///     Retrieves the last socket error code from the underlying platform.
/// </summary>
/// <returns>The last <see cref="SocketError" />.</returns>
i32 _GetLastSocketError(void)
{
    return _FromNativeErrno(errno);
}

/// <summary>
///     Starts up the platform-specific socket subsystem.
/// </summary>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Startup(void)
{
    return _SOCKET_ERROR_SUCCESS;
}

/// <summary>
///     Cleans up the platform-specific socket subsystem.
/// </summary>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _Cleanup(void)
{
    return _SOCKET_ERROR_SUCCESS;
}

/// <summary>
///     Creates a native socket handle.
/// </summary>
/// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
/// <returns>The native socket handle, or -1 on error.</returns>
isize _Create(i32 ipv6)
{
    i32 family = ipv6 ? _AF_INET_6 : _AF_INET_4;
    i32 s = socket(family, SOCK_DGRAM, IPPROTO_UDP);
    return (isize)s;
}

/// <summary>
///     Closes a native socket handle.
/// </summary>
/// <param name="socket">The native socket handle to close.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _Close(isize socket)
{
    return close((i32)socket);
}

/// <summary>
///     Binds a socket to an Ipv4 address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _BindIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 local_addr;
    if (socketAddress == NULL)
    {
        memset(&local_addr, 0, sizeof(_sockaddr_in4));
        local_addr.sin4_family = _ADDRESS_FAMILY_INTER_NETWORK_V4;
        socketAddress = &local_addr;
    }
    return bind((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
}

/// <summary>
///     Binds a socket to an Ipv6 address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _BindIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 local_addr;
    if (socketAddress == NULL)
    {
        memset(&local_addr, 0, sizeof(_sockaddr_in6));
        local_addr.sin6_family = _ADDRESS_FAMILY_INTER_NETWORK_V6;
        socketAddress = &local_addr;
    }
    return bind((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
}

/// <summary>
///     Connects a socket to an Ipv4 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _ConnectIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    return connect((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
}

/// <summary>
///     Connects a socket to an Ipv6 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _ConnectIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
    return connect((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
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
i32 _SetOption(isize socket, i32 level, i32 name, u8 *value, i32 length)
{
    i32 native_level = _ToNativeSocketOptionLevel(level);
    i32 native_name = _ToNativeSocketOptionName(level, name);
    i32 result = setsockopt((i32)socket, native_level, native_name, value, (socklen_t)length);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
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
i32 _GetOption(isize socket, i32 level, i32 name, u8 *value, i32 *length)
{
    i32 native_level = _ToNativeSocketOptionLevel(level);
    i32 native_name = _ToNativeSocketOptionName(level, name);
    i32 result = getsockopt((i32)socket, native_level, native_name, value, (socklen_t *)length);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Sets a socket's blocking mode.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="blocking">true for blocking; false for non-blocking.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _SetBlocking(isize socket, i32 blocking)
{
    i32 nonBlocking = blocking ? 0 : 1;
    i32 result = ioctl((i32)socket, FIONBIO, &nonBlocking);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Polls a socket for pending events.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="microseconds">The timeout in microseconds.</param>
/// <param name="mode">The select mode.</param>
/// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Poll(isize socket, i32 microseconds, i32 mode, i32 *status)
{
    short events = 0;
    switch (mode)
    {
    case _SELECT_MODE_SELECT_READ:
        events = POLLIN;
        break;
    case _SELECT_MODE_SELECT_WRITE:
        events = POLLOUT;
        break;
    case _SELECT_MODE_SELECT_ERROR:
        events = POLLPRI;
        break;
    }
    struct pollfd pfd;
    pfd.fd = (i32)socket;
    pfd.events = events;
    pfd.revents = 0;
    i32 timeout = (microseconds == -1) ? -1 : (microseconds / 1000);
    i32 result = poll(&pfd, 1, timeout);
    if (result == -1)
    {
        *status = 0;
        return _GetLastSocketError();
    }
    switch (mode)
    {
    case _SELECT_MODE_SELECT_READ:
        *status = (pfd.revents & (POLLIN | POLLHUP)) ? 1 : 0;
        break;
    case _SELECT_MODE_SELECT_WRITE:
        *status = (pfd.revents & POLLOUT) ? 1 : 0;
        break;
    case _SELECT_MODE_SELECT_ERROR:
        *status = (pfd.revents & (POLLERR | POLLPRI)) ? 1 : 0;
        break;
    default:
        *status = 0;
        break;
    }
    return _SOCKET_ERROR_SUCCESS;
}

/// <summary>
///     Polls a socket for pending events.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="microseconds">The timeout in microseconds.</param>
/// <param name="inFlags">The select inFlags.</param>
/// <param name="outFlags">When this method returns, contains true if the socket is ready, false otherwise.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _PollFlags(isize socket, i32 microseconds, i32 inFlags, i32 *outFlags)
{
    short events = 0;
    if ((inFlags & _SELECT_MODE_FLAGS_READ) != 0)
    {
        events |= POLLIN;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_WRITE) != 0)
    {
        events |= POLLOUT;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_ERROR) != 0)
    {
        events |= POLLPRI;
    }
    struct pollfd pfd;
    pfd.fd = (i32)socket;
    pfd.events = events;
    pfd.revents = 0;
    i32 timeout = (microseconds == -1) ? -1 : (microseconds / 1000);
    *outFlags = 0;
    i32 result = poll(&pfd, 1, timeout);
    if (result == -1)
    {
        return _GetLastSocketError();
    }
    if ((pfd.revents & (POLLIN | POLLHUP)) != 0)
    {
        *outFlags |= _SELECT_MODE_FLAGS_READ;
    }
    if ((pfd.revents & POLLOUT) != 0)
    {
        *outFlags |= _SELECT_MODE_FLAGS_WRITE;
    }
    if ((pfd.revents & (POLLERR | POLLPRI)) != 0)
    {
        *outFlags |= _SELECT_MODE_FLAGS_ERROR;
    }
    return _SOCKET_ERROR_SUCCESS;
}

/// <summary>
///     Sends data on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the data buffer.</param>
/// <param name="length">Length of the buffer in bytes.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <returns>The number of bytes sent, or -1 on error.</returns>
i32 _Send(isize socket, void *buffer, i32 length, i32 socketFlags)
{
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    return (i32)send((i32)socket, buffer, (usize)length, native_flags);
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
i32 _SendToIpv4(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    if (socketAddress != NULL)
    {
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        return (i32)sendto((i32)socket, buffer, (usize)length, native_flags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
    }
    return _Send(socket, buffer, length, socketFlags);
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
i32 _SendToIpv6(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    if (socketAddress != NULL)
    {
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        return (i32)sendto((i32)socket, buffer, (usize)length, native_flags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
    }
    return _Send(socket, buffer, length, socketFlags);
}

/// <summary>
///     Receives data on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the receive buffer.</param>
/// <param name="length">Length of the buffer.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <returns>The number of bytes received, or -1 on error.</returns>
i32 _Receive(isize socket, void *buffer, i32 length, i32 socketFlags)
{
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    return (i32)recv((i32)socket, buffer, (usize)length, native_flags);
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
i32 _ReceiveFromIpv4(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in4);
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    i32 result = (i32)recvfrom((i32)socket, buffer, (usize)length, native_flags, (struct sockaddr *)&storage, &addr_len);
    if (result >= 0 && socketAddress != NULL)
    {
        memcpy(socketAddress, &storage, sizeof(_sockaddr_in4));
    }
    return result;
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
i32 _ReceiveFromIpv6(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in6);
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    i32 result = (i32)recvfrom((i32)socket, buffer, (usize)length, native_flags, (struct sockaddr *)&storage, &addr_len);
    if (result >= 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return result;
}

/// <summary>
///     Builds a <see cref="NativeScopedArray{struct iovec}" /> from an array of <see cref="NativeIoSlice" /> structures.
/// </summary>
/// <param name="buffer">A span that can be used for temporary storage (e.g., stackalloc).</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <returns>A <see cref="NativeScopedArray{struct iovec}" /> that wraps the converted buffers.</returns>
static i32 _Build(_NativeIoSlice *buffers, i32 bufferCount, struct iovec *out_vecs)
{
    i32 i;
    for (i = 0; i < bufferCount; ++i)
    {
        out_vecs[i].iov_base = buffers[i]._buffer;
        out_vecs[i].iov_len = (usize)buffers[i]._length;
    }
    return bufferCount;
}

/// <summary>
///     Sends data from multiple buffers on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <returns>The number of bytes sent, or -1 on error.</returns>
i32 _SendVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags)
{
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    struct iovec iovecs[16];
    struct iovec *piovecs = (bufferCount <= 16) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (!piovecs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, piovecs);
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)sendmsg((i32)socket, &msg, native_flags);
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
}

/// <summary>
///     Sends data from multiple buffers to an Ipv4 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
/// <returns>The number of bytes sent, or -1 on error.</returns>
i32 _SendToVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    if (socketAddress == NULL)
    {
        return _SendVectored(socket, buffers, bufferCount, socketFlags);
    }
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    struct iovec iovecs[16];
    struct iovec *piovecs = (bufferCount <= 16) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (!piovecs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, piovecs);
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_name = socketAddress;
    msg.msg_namelen = sizeof(_sockaddr_in4);
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)sendmsg((i32)socket, &msg, native_flags);
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
}

/// <summary>
///     Sends data from multiple buffers to an Ipv6 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
/// <returns>The number of bytes sent, or -1 on error.</returns>
i32 _SendToVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    if (socketAddress == NULL)
    {
        return _SendVectored(socket, buffers, bufferCount, socketFlags);
    }
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    struct iovec iovecs[16];
    struct iovec *piovecs = (bufferCount <= 16) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (!piovecs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, piovecs);
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_name = socketAddress;
    msg.msg_namelen = sizeof(_sockaddr_in6);
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)sendmsg((i32)socket, &msg, native_flags);
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
}

/// <summary>
///     Receives data into multiple buffers on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
/// <returns>The number of bytes received, or -1 on error.</returns>
i32 _ReceiveVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags)
{
    i32 native_flags = (inOutFlags != NULL) ? _ToNativeSocketFlags(*inOutFlags) : 0;
    struct iovec iovecs[16];
    struct iovec *piovecs = (bufferCount <= 16) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (!piovecs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, piovecs);
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (inOutFlags != NULL)
    {
        *inOutFlags = _FromNativeSocketFlags(msg.msg_flags);
    }
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
}

/// <summary>
///     Receives data into multiple buffers from an Ipv4 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
/// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
/// <returns>The number of bytes received, or -1 on error.</returns>
i32 _ReceiveFromVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    i32 native_flags = (inOutFlags != NULL) ? _ToNativeSocketFlags(*inOutFlags) : 0;
    struct iovec iovecs[16];
    struct iovec *piovecs = (bufferCount <= 16) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (!piovecs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, piovecs);
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_name = &storage;
    msg.msg_namelen = sizeof(_sockaddr_in4);
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (inOutFlags != NULL)
    {
        *inOutFlags = _FromNativeSocketFlags(msg.msg_flags);
    }
    if (result >= 0 && socketAddress != NULL)
    {
        memcpy(socketAddress, &storage, sizeof(_sockaddr_in4));
    }
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
}

i32 _ReceiveFromVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    i32 native_flags = (inOutFlags != NULL) ? _ToNativeSocketFlags(*inOutFlags) : 0;
    struct iovec iovecs[16];
    struct iovec *piovecs = (bufferCount <= 16) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (!piovecs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, piovecs);
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_name = &storage;
    msg.msg_namelen = sizeof(_sockaddr_in6);
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (inOutFlags != NULL)
    {
        *inOutFlags = _FromNativeSocketFlags(msg.msg_flags);
    }
    if (result >= 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
}

/// <summary>
///     Gets the local name (address) of an Ipv4 socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 address structure to receive the name.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _GetNameIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in4);
    i32 result = getsockname((i32)socket, (struct sockaddr *)&storage, &addr_len);
    if (result == 0 && socketAddress != NULL)
    {
        memcpy(socketAddress, &storage, sizeof(_sockaddr_in4));
    }
    return result;
}

/// <summary>
///     Gets the local name (address) of an Ipv6 socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 address structure to receive the name.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _GetNameIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in6);
    i32 result = getsockname((i32)socket, (struct sockaddr *)&storage, &addr_len);
    if (result == 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return result;
}

/// <summary>
///     Gets the host name (reverse DNS) from an Ipv4 address.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <param name="hostName">A span to receive the host name bytes.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
i32 _GetHostNameIpv4(_sockaddr_in4 *socketAddress, u8 *hostName, i32 hostNameLength)
{
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), (char *)hostName, (socklen_t)hostNameLength, NULL, 0, 0) == 0)
    {
        return _SOCKET_ERROR_SUCCESS;
    }
    return _SOCKET_ERROR_FAULT;
}

/// <summary>
///     Gets the host name (reverse DNS) from an Ipv6 address.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <param name="hostName">A span to receive the host name bytes.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
i32 _GetHostNameIpv6(_sockaddr_in6 *socketAddress, u8 *hostName, i32 hostNameLength)
{
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), (char *)hostName, (socklen_t)hostNameLength, NULL, 0, 0) == 0)
    {
        return _SOCKET_ERROR_SUCCESS;
    }
    return _SOCKET_ERROR_FAULT;
}

#endif
