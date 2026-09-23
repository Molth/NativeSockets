#ifdef _WIN32

/// <summary>
///     Converts a time duration in microseconds to a <c>struct timeval</c> structure.
/// </summary>
/// <param name="microseconds">The duration in microseconds.</param>
/// <param name="socketTime">Pointer to the <c>struct timeval</c> structure to fill.</param>
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
///     Converts an array of <see cref="NativeIoSlice" /> to WinSock <c>WSABUF</c> entries.
/// </summary>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="out_bufs">Pointer to the output array of <c>WSABUF</c> structures to fill.</param>
static void _Build(_NativeIoSlice *buffers, i32 bufferCount, WSABUF *out_bufs)
{
    i32 i;
    for (i = 0; i < bufferCount; ++i)
    {
        out_bufs[i].buf = (u8 *)buffers[i]._buffer;
        out_bufs[i].len = (u32)buffers[i]._length;
    }
}

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
/// <returns>The Ipv4 address family value.</returns>
u16 _GetAddressFamilyInterNetworkV4(void)
{
    return _ADDRESS_FAMILY_INTER_NETWORK_V4;
}

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
/// <returns>The Ipv6 address family value.</returns>
u16 _GetAddressFamilyInterNetworkV6(void)
{
    return _ADDRESS_FAMILY_INTER_NETWORK_V6;
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
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Cleanup(void)
{
    return WSACleanup();
}

/// <summary>
///     Creates a native socket handle.
/// </summary>
/// <param name="ipv6">Non-zero for Ipv6; 0 for Ipv4.</param>
/// <param name="out_socket">When this method returns, contains the native socket handle, or -1 on error.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Create(i32 ipv6, isize *out_socket)
{
    i32 family = ipv6 ? _AF_INET_6 : _AF_INET_4;
    SOCKET s = WSASocketW(family, SOCK_DGRAM, IPPROTO_UDP, NULL, 0, 1 | 128);
    if (s != -1)
    {
        DWORD dwBytesReturned = 0;
        BOOL bNewBehavior = FALSE;
        WSAIoctl(s, SIO_UDP_CONNRESET, &bNewBehavior, sizeof(BOOL), NULL, 0, &dwBytesReturned, NULL, NULL);
    }
    *out_socket = (isize)s;
    return (s == -1) ? _GetLastSocketError() : _SOCKET_ERROR_SUCCESS;
}

/// <summary>
///     Closes a native socket handle.
/// </summary>
/// <param name="socket">The native socket handle to close.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Close(isize socket)
{
    i32 result = closesocket((SOCKET)socket);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Binds a socket to an Ipv4 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _BindIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    i32 result;
    if (socketAddress != NULL)
    {
        result = bind((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
    }
    else
    {
        _sockaddr_in4 local_addr;
        memset(&local_addr, 0, sizeof(_sockaddr_in4));
        local_addr.sin4_family = _ADDRESS_FAMILY_INTER_NETWORK_V4;
        result = bind((SOCKET)socket, (const struct sockaddr *)&local_addr, sizeof(_sockaddr_in4));
    }
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Binds a socket to an Ipv6 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _BindIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
    i32 result;
    if (socketAddress != NULL)
    {
        result = bind((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
    }
    else
    {
        _sockaddr_in6 local_addr;
        memset(&local_addr, 0, sizeof(_sockaddr_in6));
        local_addr.sin6_family = _ADDRESS_FAMILY_INTER_NETWORK_V6;
        result = bind((SOCKET)socket, (const struct sockaddr *)&local_addr, sizeof(_sockaddr_in6));
    }
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Connects a socket to an Ipv4 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _ConnectIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    i32 result = WSAConnect((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), NULL, NULL, NULL, NULL);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Connects a socket to an Ipv6 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _ConnectIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
    i32 result = WSAConnect((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), NULL, NULL, NULL, NULL);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
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
i32 _SetOption(isize socket, i32 level, i32 name, u8 *value, i32 length)
{
    i32 result = setsockopt((SOCKET)socket, level, name, (const u8 *)value, (_socklen_t)length);
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
/// <remarks>
///     The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
///     platform equivalents by the underlying socket layer. The <paramref name="value" /> buffer is
///     passed through unmodified; the platform populates the buffer according to the mapped option.
/// </remarks>
i32 _GetOption(isize socket, i32 level, i32 name, u8 *value, i32 *length)
{
    i32 result = getsockopt((SOCKET)socket, level, name, (u8 *)value, (_socklen_t *)length);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
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
i32 _SetRawOption(isize socket, i32 level, i32 name, u8 *value, i32 length)
{
    i32 result = setsockopt((SOCKET)socket, level, name, (const u8 *)value, (_socklen_t)length);
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
i32 _GetRawOption(isize socket, i32 level, i32 name, u8 *value, i32 *length)
{
    i32 result = getsockopt((SOCKET)socket, level, name, (u8 *)value, (_socklen_t *)length);
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Sets a socket's blocking mode.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="blocking">Non-zero for blocking; 0 for non-blocking.</param>
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
/// <param name="status">When this method returns, contains non-zero if the socket is ready, 0 otherwise.</param>
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
/// <param name="inFlags">The select mode.</param>
/// <param name="outFlags">When this method returns, contains the poll result flags.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _PollFlags(isize socket, i32 microseconds, i32 inFlags, i32 *outFlags)
{
    isize _readFds[2];
    isize *readFds = _readFds;
    isize _writeFds[2];
    isize *writeFds = _writeFds;
    isize _errorFds[2];
    isize *errorFds = _errorFds;
    if ((inFlags & _SELECT_MODE_FLAGS_READ) == 0)
    {
        readFds = NULL;
    }
    else
    {
        readFds[0] = 1;
        readFds[1] = socket;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_WRITE) == 0)
    {
        writeFds = NULL;
    }
    else
    {
        writeFds[0] = 1;
        writeFds[1] = socket;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_ERROR) == 0)
    {
        errorFds = NULL;
    }
    else
    {
        errorFds[0] = 1;
        errorFds[1] = socket;
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
///     Sends data to an Ipv4 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the data buffer.</param>
/// <param name="length">Length of the buffer.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
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
///     Sends data to an Ipv6 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the data buffer.</param>
/// <param name="length">Length of the buffer.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
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
///     Receives data from an Ipv4 socket address, filling the provided socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the receive buffer.</param>
/// <param name="length">Length of the buffer.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
/// <returns>The number of bytes received, or -1 on error.</returns>
i32 _ReceiveFromIpv4(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in4);
    i32 result = (i32)recvfrom((SOCKET)socket, (u8 *)buffer, length, socketFlags, (struct sockaddr *)&storage, &addr_len);
    if (result >= 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return result;
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
///     Sends data from multiple buffers on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <returns>The number of bytes sent, or -1 on error.</returns>
i32 _SendVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags)
{
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (pwsabufs == NULL)
    {
        WSASetLastError(WSAENOBUFS);
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
///     Sends data from multiple buffers to an Ipv4 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
/// <returns>The number of bytes sent, or -1 on error.</returns>
i32 _SendToVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    if (socketAddress != NULL)
    {
        WSABUF wsabufs[16];
        WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
        if (pwsabufs == NULL)
        {
            WSASetLastError(WSAENOBUFS);
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
    return _SendVectored(socket, buffers, bufferCount, socketFlags);
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
i32 _SendToVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    if (socketAddress != NULL)
    {
        WSABUF wsabufs[16];
        WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
        if (pwsabufs == NULL)
        {
            WSASetLastError(WSAENOBUFS);
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
    return _SendVectored(socket, buffers, bufferCount, socketFlags);
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
i32 _ReceiveVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags)
{
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (pwsabufs == NULL)
    {
        WSASetLastError(WSAENOBUFS);
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
i32 _ReceiveFromVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (pwsabufs == NULL)
    {
        WSASetLastError(WSAENOBUFS);
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
        *socketAddress = storage;
    }
    return bytesRecv;
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
i32 _ReceiveFromVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (pwsabufs == NULL)
    {
        WSASetLastError(WSAENOBUFS);
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
/// <param name="socketAddress">Pointer to the Ipv4 socket address to receive the name.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _GetNameIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in4);
    i32 result = getsockname((SOCKET)socket, (struct sockaddr *)&storage, &addr_len);
    if (result == 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Gets the local name (address) of an Ipv6 socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 socket address to receive the name.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _GetNameIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in6);
    i32 result = getsockname((SOCKET)socket, (struct sockaddr *)&storage, &addr_len);
    if (result == 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

#endif
