#ifdef _WIN32

/// <summary>
///     Converts a time duration in microseconds to a <see cref="TimeValue" /> structure.
/// </summary>
/// <param name="microseconds">The duration in microseconds.</param>
/// <param name="socketTime">The <see cref="TimeValue" /> structure to fill.</param>
static void _MicrosecondsToTimeValue(i64 microseconds, struct timeval *socketTime)
{
    const i64 microcnv = 1000000;
    i64 quotient = microseconds / microcnv;
    i64 remainder = microseconds - quotient * microcnv;
    memset(socketTime, 0, sizeof(struct timeval));
    socketTime->tv_sec = (i32)quotient;
    socketTime->tv_usec = (i32)remainder;
}

/// <summary>
///     Retrieves the last socket error code from the underlying platform.
/// </summary>
/// <returns>The last <see cref="SocketError" />.</returns>
i32 _GetLastSocketError(void)
{
    return (i32)WSAGetLastError();
}

/// <summary>
///     Starts up the platform-specific socket subsystem.
/// </summary>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Startup(void)
{
    WSADATA wsaData;
    return WSAStartup(514, &wsaData);
}

/// <summary>
///     Cleans up the platform-specific socket subsystem.
/// </summary>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _Cleanup(void)
{
    return WSACleanup();
}

/// <summary>
///     Creates a native socket handle.
/// </summary>
/// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
/// <returns>The native socket handle, or -1 on error.</returns>
isize _Create(i32 ipv6)
{
    i32 family = ipv6 ? _AF_INET_6 : _AF_INET_4;
    SOCKET s = WSASocketW(family, SOCK_DGRAM, IPPROTO_UDP, NULL, 0, 1 | 128);
    if (s != -1)
    {
        DWORD dwBytesReturned = 0;
        BOOL bNewBehavior = FALSE;
        WSAIoctl(s, SIO_UDP_CONNRESET, &bNewBehavior, sizeof(BOOL), NULL, 0, &dwBytesReturned, NULL, NULL);
    }
    return (isize)s;
}

/// <summary>
///     Closes a native socket handle.
/// </summary>
/// <param name="socket">The native socket handle to close.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _Close(isize socket)
{
    return closesocket((SOCKET)socket);
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
    return bind((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
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
    return bind((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
}

/// <summary>
///     Connects a socket to an Ipv4 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _ConnectIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    return WSAConnect((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), NULL, NULL, NULL, NULL);
}

/// <summary>
///     Connects a socket to an Ipv6 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _ConnectIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
    return WSAConnect((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), NULL, NULL, NULL, NULL);
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
    i32 result = setsockopt((SOCKET)socket, level, name, (const u8 *)value, (socklen_t)length);
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
    i32 result = getsockopt((SOCKET)socket, level, name, (u8 *)value, (socklen_t *)length);
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
    u_long nonBlocking = blocking ? 0 : 1;
    i32 result = ioctlsocket((SOCKET)socket, FIONBIO, &nonBlocking);
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
    isize fdset[2];
    fdset[0] = 1;
    fdset[1] = socket;
    i32 result;
    if (microseconds != -1)
    {
        struct timeval tv;
        _MicrosecondsToTimeValue(microseconds, &tv);
        result = select(0, (mode == _SELECT_MODE_SELECT_READ) ? (fd_set *)fdset : NULL, (mode == _SELECT_MODE_SELECT_WRITE) ? (fd_set *)fdset : NULL, (mode == _SELECT_MODE_SELECT_ERROR) ? (fd_set *)fdset : NULL, &tv);
    }
    else
    {
        result = select(0, (mode == _SELECT_MODE_SELECT_READ) ? (fd_set *)fdset : NULL, (mode == _SELECT_MODE_SELECT_WRITE) ? (fd_set *)fdset : NULL, (mode == _SELECT_MODE_SELECT_ERROR) ? (fd_set *)fdset : NULL, NULL);
    }
    if (result == SOCKET_ERROR)
    {
        *status = 0;
        return _GetLastSocketError();
    }
    *status = FD_ISSET(socket, (fd_set *)fdset);
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
    isize _readFds[2];
    _readFds[0] = 1;
    _readFds[1] = socket;
    isize *readFds = _readFds;
    isize _writeFds[2];
    _writeFds[0] = 1;
    _writeFds[1] = socket;
    isize *writeFds = _writeFds;
    isize _errorFds[2];
    _errorFds[0] = 1;
    _errorFds[1] = socket;
    isize *errorFds = _errorFds;
    if ((inFlags & _SELECT_MODE_FLAGS_READ) == 0)
    {
        readFds = NULL;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_WRITE) == 0)
    {
        writeFds = NULL;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_ERROR) == 0)
    {
        errorFds = NULL;
    }
    i32 result;
    if (microseconds != -1)
    {
        struct timeval tv;
        _MicrosecondsToTimeValue(microseconds, &tv);
        result = select(0, (fd_set *)readFds, (fd_set *)writeFds, (fd_set *)errorFds, &tv);
    }
    else
    {
        result = select(0, (fd_set *)readFds, (fd_set *)writeFds, (fd_set *)errorFds, NULL);
    }
    *outFlags = 0;
    if (result == SOCKET_ERROR)
    {
        return _GetLastSocketError();
    }
    if (readFds != NULL && FD_ISSET(socket, (fd_set *)readFds))
    {
        *outFlags |= _SELECT_MODE_FLAGS_READ;
    }
    if (writeFds != NULL && FD_ISSET(socket, (fd_set *)writeFds))
    {
        *outFlags |= _SELECT_MODE_FLAGS_WRITE;
    }
    if (errorFds != NULL && FD_ISSET(socket, (fd_set *)errorFds))
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
    return (i32)send((SOCKET)socket, (const u8 *)buffer, length, socketFlags);
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
        return (i32)sendto((SOCKET)socket, (const u8 *)buffer, length, socketFlags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
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
        return (i32)sendto((SOCKET)socket, (const u8 *)buffer, length, socketFlags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
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
    return (i32)recv((SOCKET)socket, (u8 *)buffer, length, socketFlags);
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
    i32 result = (i32)recvfrom((SOCKET)socket, (u8 *)buffer, length, socketFlags, (struct sockaddr *)&storage, &addr_len);
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
    i32 result = (i32)recvfrom((SOCKET)socket, (u8 *)buffer, length, socketFlags, (struct sockaddr *)&storage, &addr_len);
    if (result >= 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return result;
}

/// <summary>
///     Builds a <see cref="NativeScopedArray{WSABuffer}" /> from an array of <see cref="NativeIoSlice" /> structures.
/// </summary>
/// <param name="buffer">A span that can be used for temporary storage (e.g., stackalloc).</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <returns>A <see cref="NativeScopedArray{WSABuffer}" /> that wraps the converted buffers.</returns>
static i32 _Build(_NativeIoSlice *buffers, i32 bufferCount, WSABUF *out_bufs)
{
    i32 i;
    for (i = 0; i < bufferCount; ++i)
    {
        out_bufs[i].buf = (u8 *)buffers[i]._buffer;
        out_bufs[i].len = (u32)buffers[i]._length;
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
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesSent = 0;
    i32 result = WSASend((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesSent, socketFlags, NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    return (result == 0) ? bytesSent : -1;
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
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesSent = 0;
    i32 result = WSASendTo((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesSent, socketFlags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    return (result == 0) ? bytesSent : -1;
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
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesSent = 0;
    i32 result = WSASendTo((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesSent, socketFlags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    return (result == 0) ? bytesSent : -1;
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
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesRecv = 0;
    DWORD flags = (inOutFlags != NULL) ? *inOutFlags : 0;
    i32 result = WSARecv((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesRecv, &flags, NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    if (inOutFlags != NULL)
    {
        *inOutFlags = (i32)flags;
    }
    if (result != 0)
    {
        return -1;
    }
    return bytesRecv;
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
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesRecv = 0;
    DWORD flags = (inOutFlags != NULL) ? *inOutFlags : 0;
    INT addr_len = sizeof(_sockaddr_in4);
    i32 result = WSARecvFrom((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesRecv, &flags, (struct sockaddr *)&storage, &addr_len, NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    if (inOutFlags != NULL)
    {
        *inOutFlags = (i32)flags;
    }
    if (result != 0)
    {
        return -1;
    }
    if (socketAddress != NULL)
    {
        memcpy(socketAddress, &storage, sizeof(_sockaddr_in4));
    }
    return bytesRecv;
}

i32 _ReceiveFromVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesRecv = 0;
    DWORD flags = (inOutFlags != NULL) ? *inOutFlags : 0;
    INT addr_len = sizeof(_sockaddr_in6);
    i32 result = WSARecvFrom((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesRecv, &flags, (struct sockaddr *)&storage, &addr_len, NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    if (inOutFlags != NULL)
    {
        *inOutFlags = (i32)flags;
    }
    if (result != 0)
    {
        return -1;
    }
    if (socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return bytesRecv;
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
    i32 result = getsockname((SOCKET)socket, (struct sockaddr *)&storage, &addr_len);
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
    i32 result = getsockname((SOCKET)socket, (struct sockaddr *)&storage, &addr_len);
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
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), (char *)hostName, (DWORD)hostNameLength, NULL, 0, 0) == 0)
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
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), (char *)hostName, (DWORD)hostNameLength, NULL, 0, 0) == 0)
    {
        return _SOCKET_ERROR_SUCCESS;
    }
    return _SOCKET_ERROR_FAULT;
}

#endif
