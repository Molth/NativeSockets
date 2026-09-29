#ifndef _WIN32

/// <summary>
///     Converts an array of <see cref="NativeIoSlice" /> to <c>iovec</c> entries.
/// </summary>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="out_vecs">Pointer to the output array of <c>iovec</c> structures to fill.</param>
static void _Build(_NativeIoSlice *buffers, i32 bufferCount, struct iovec *out_vecs)
{
    i32 i;
    for (i = 0; i < bufferCount; ++i)
    {
        out_vecs[i].iov_base = buffers[i]._buffer;
        out_vecs[i].iov_len = (usize)buffers[i]._length;
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
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Cleanup(void)
{
    return _SOCKET_ERROR_SUCCESS;
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
    i32 s = socket(family, SOCK_DGRAM, IPPROTO_UDP);
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
    i32 result = close((i32)socket);
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
        result = bind((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
    }
    else
    {
        _sockaddr_in4 local_addr;
        memset(&local_addr, 0, sizeof(_sockaddr_in4));
        local_addr.sin4_family = _ADDRESS_FAMILY_INTER_NETWORK_V4;
        result = bind((i32)socket, (const struct sockaddr *)&local_addr, sizeof(_sockaddr_in4));
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
        result = bind((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
    }
    else
    {
        _sockaddr_in6 local_addr;
        memset(&local_addr, 0, sizeof(_sockaddr_in6));
        local_addr.sin6_family = _ADDRESS_FAMILY_INTER_NETWORK_V6;
        result = bind((i32)socket, (const struct sockaddr *)&local_addr, sizeof(_sockaddr_in6));
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
    i32 result = connect((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
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
    i32 result = connect((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
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
///     <list type="bullet">
///         <item>
///             <description>
///                 The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
///                 platform equivalents by the underlying socket layer.
///             </description>
///         </item>
///         <item>
///             <description>
///                 The <paramref name="value" /> bytes are passed through unmodified; the platform interprets the
///                 buffer according to the mapped option.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
///                 <paramref name="level" /> and <paramref name="name" /> is guaranteed.
///             </description>
///         </item>
///     </list>
/// </remarks>
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
/// <remarks>
///     <list type="bullet">
///         <item>
///             <description>
///                 The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
///                 platform equivalents by the underlying socket layer.
///             </description>
///         </item>
///         <item>
///             <description>
///                 The <paramref name="value" /> buffer is passed through unmodified; the platform populates the
///                 buffer according to the mapped option.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
///                 <paramref name="level" /> and <paramref name="name" /> is guaranteed.
///             </description>
///         </item>
///     </list>
/// </remarks>
i32 _GetOption(isize socket, i32 level, i32 name, u8 *value, i32 *length)
{
    i32 native_level = _ToNativeSocketOptionLevel(level);
    i32 native_name = _ToNativeSocketOptionName(level, name);
    i32 result = getsockopt((i32)socket, native_level, native_name, value, (socklen_t *)length);
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
    i32 result = setsockopt((i32)socket, level, name, value, (socklen_t)length);
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
    i32 result = getsockopt((i32)socket, level, name, value, (socklen_t *)length);
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
/// <param name="status">When this method returns, contains non-zero if the socket is ready, 0 otherwise.</param>
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
    i32 timeout = (microseconds == -1) ? -1 : (microseconds / 1000);
    struct pollfd pfd;
    pfd.fd = (i32)socket;
    pfd.events = events;
    pfd.revents = 0;
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
/// <param name="inFlags">The select mode.</param>
/// <param name="outFlags">When this method returns, contains the poll result flags.</param>
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
    i32 timeout = (microseconds == -1) ? -1 : (microseconds / 1000);
    struct pollfd pfd;
    pfd.fd = (i32)socket;
    pfd.events = events;
    pfd.revents = 0;
    *outFlags = 0;
    i32 result = poll(&pfd, 1, timeout);
    if (result == -1)
    {
        return _GetLastSocketError();
    }
    if ((inFlags & _SELECT_MODE_FLAGS_READ) != 0 && (pfd.revents & (POLLIN | POLLHUP)) != 0)
    {
        *outFlags |= _SELECT_MODE_FLAGS_READ;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_WRITE) != 0 && (pfd.revents & POLLOUT) != 0)
    {
        *outFlags |= _SELECT_MODE_FLAGS_WRITE;
    }
    if ((inFlags & _SELECT_MODE_FLAGS_ERROR) != 0 && (pfd.revents & (POLLERR | POLLPRI)) != 0)
    {
        *outFlags |= _SELECT_MODE_FLAGS_ERROR;
    }
    return _SOCKET_ERROR_SUCCESS;
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
    i32 result = getsockname((i32)socket, (struct sockaddr *)&storage, &addr_len);
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
    i32 result = getsockname((i32)socket, (struct sockaddr *)&storage, &addr_len);
    if (result == 0 && socketAddress != NULL)
    {
        *socketAddress = storage;
    }
    return (result == 0) ? _SOCKET_ERROR_SUCCESS : _GetLastSocketError();
}

/// <summary>
///     Sends data on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the data buffer.</param>
/// <param name="length">Length of the buffer in bytes.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _Send(isize socket, void *buffer, i32 length, i32 socketFlags)
{
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    i32 num = (i32)send((i32)socket, buffer, (usize)length, native_flags);
    _IoResult result;
    if (num >= 0)
    {
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
    }
    return result;
}

/// <summary>
///     Sends data to an Ipv4 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the data buffer.</param>
/// <param name="length">Length of the buffer.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _SendToIpv4(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    if (socketAddress != NULL)
    {
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        i32 num = (i32)sendto((i32)socket, buffer, (usize)length, native_flags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
        _IoResult result;
        if (num >= 0)
        {
            result.BytesTransferred = num;
            result.SocketError = _SOCKET_ERROR_SUCCESS;
        }
        else
        {
            result.BytesTransferred = -1;
            result.SocketError = _GetLastSocketError();
        }
        return result;
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
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _SendToIpv6(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    if (socketAddress != NULL)
    {
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        i32 num = (i32)sendto((i32)socket, buffer, (usize)length, native_flags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
        _IoResult result;
        if (num >= 0)
        {
            result.BytesTransferred = num;
            result.SocketError = _SOCKET_ERROR_SUCCESS;
        }
        else
        {
            result.BytesTransferred = -1;
            result.SocketError = _GetLastSocketError();
        }
        return result;
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
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _Receive(isize socket, void *buffer, i32 length, i32 socketFlags)
{
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    i32 num = (i32)recv((i32)socket, buffer, (usize)length, native_flags);
    _IoResult result;
    if (num >= 0)
    {
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
    }
    return result;
}

/// <summary>
///     Receives data from an Ipv4 socket address, filling the provided socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffer">Pointer to the receive buffer.</param>
/// <param name="length">Length of the buffer.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _ReceiveFromIpv4(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in4);
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    i32 num = (i32)recvfrom((i32)socket, buffer, (usize)length, native_flags, (struct sockaddr *)&storage, &addr_len);
    _IoResult result;
    if (num >= 0)
    {
        if (socketAddress != NULL)
        {
            *socketAddress = storage;
        }
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
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
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _ReceiveFromIpv6(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    _socklen_t addr_len = sizeof(_sockaddr_in6);
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    i32 num = (i32)recvfrom((i32)socket, buffer, (usize)length, native_flags, (struct sockaddr *)&storage, &addr_len);
    _IoResult result;
    if (num >= 0)
    {
        if (socketAddress != NULL)
        {
            *socketAddress = storage;
        }
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
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
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _SendVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags)
{
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_iovlen = bufferCount;
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    i32 num;
    struct iovec iovecs[MAX_STACKALLOC_VECTORED_BUFFERS];
    struct iovec *piovecs = (bufferCount <= MAX_STACKALLOC_VECTORED_BUFFERS) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (piovecs == NULL)
    {
        errno = ENOMEM;
        _IoResult oom_result;
        oom_result.BytesTransferred = -1;
        oom_result.SocketError = _GetLastSocketError();
        return oom_result;
    }
    _Build(buffers, bufferCount, piovecs);
    msg.msg_iov = (struct iovec *)piovecs;
    num = (i32)sendmsg((i32)socket, &msg, native_flags);
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    _IoResult result;
    if (num >= 0)
    {
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
    }
    return result;
}

/// <summary>
///     Sends data from multiple buffers to an Ipv4 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _SendToVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    if (socketAddress != NULL)
    {
        struct msghdr msg;
        memset(&msg, 0, sizeof(struct msghdr));
        msg.msg_name = socketAddress;
        msg.msg_namelen = sizeof(_sockaddr_in4);
        msg.msg_iovlen = bufferCount;
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        i32 num;
        struct iovec iovecs[MAX_STACKALLOC_VECTORED_BUFFERS];
        struct iovec *piovecs = (bufferCount <= MAX_STACKALLOC_VECTORED_BUFFERS) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
        if (piovecs == NULL)
        {
            errno = ENOMEM;
            _IoResult oom_result;
            oom_result.BytesTransferred = -1;
            oom_result.SocketError = _GetLastSocketError();
            return oom_result;
        }
        _Build(buffers, bufferCount, piovecs);
        msg.msg_iov = (struct iovec *)piovecs;
        num = (i32)sendmsg((i32)socket, &msg, native_flags);
        if (piovecs != iovecs)
        {
            free(piovecs);
        }
        _IoResult result;
        if (num >= 0)
        {
            result.BytesTransferred = num;
            result.SocketError = _SOCKET_ERROR_SUCCESS;
        }
        else
        {
            result.BytesTransferred = -1;
            result.SocketError = _GetLastSocketError();
        }
        return result;
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
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _SendToVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    if (socketAddress != NULL)
    {
        struct msghdr msg;
        memset(&msg, 0, sizeof(struct msghdr));
        msg.msg_name = socketAddress;
        msg.msg_namelen = sizeof(_sockaddr_in6);
        msg.msg_iovlen = bufferCount;
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        i32 num;
        struct iovec iovecs[MAX_STACKALLOC_VECTORED_BUFFERS];
        struct iovec *piovecs = (bufferCount <= MAX_STACKALLOC_VECTORED_BUFFERS) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
        if (piovecs == NULL)
        {
            errno = ENOMEM;
            _IoResult oom_result;
            oom_result.BytesTransferred = -1;
            oom_result.SocketError = _GetLastSocketError();
            return oom_result;
        }
        _Build(buffers, bufferCount, piovecs);
        msg.msg_iov = (struct iovec *)piovecs;
        num = (i32)sendmsg((i32)socket, &msg, native_flags);
        if (piovecs != iovecs)
        {
            free(piovecs);
        }
        _IoResult result;
        if (num >= 0)
        {
            result.BytesTransferred = num;
            result.SocketError = _SOCKET_ERROR_SUCCESS;
        }
        else
        {
            result.BytesTransferred = -1;
            result.SocketError = _GetLastSocketError();
        }
        return result;
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
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _ReceiveVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags)
{
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_iovlen = bufferCount;
    i32 native_flags = (inOutFlags != NULL) ? _ToNativeSocketFlags(*inOutFlags) : 0;
    i32 num;
    struct iovec iovecs[MAX_STACKALLOC_VECTORED_BUFFERS];
    struct iovec *piovecs = (bufferCount <= MAX_STACKALLOC_VECTORED_BUFFERS) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (piovecs == NULL)
    {
        errno = ENOMEM;
        _IoResult oom_result;
        oom_result.BytesTransferred = -1;
        oom_result.SocketError = _GetLastSocketError();
        return oom_result;
    }
    _Build(buffers, bufferCount, piovecs);
    msg.msg_iov = (struct iovec *)piovecs;
    num = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    _IoResult result;
    if (num >= 0)
    {
        if (inOutFlags != NULL)
        {
            *inOutFlags = _FromNativeSocketFlags(msg.msg_flags);
        }
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
    }
    return result;
}

/// <summary>
///     Receives data into multiple buffers from an Ipv4 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
/// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _ReceiveFromVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in4 *socketAddress)
{
    _sockaddr_in4 storage;
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_name = &storage;
    msg.msg_namelen = sizeof(_sockaddr_in4);
    msg.msg_iovlen = bufferCount;
    i32 native_flags = (inOutFlags != NULL) ? _ToNativeSocketFlags(*inOutFlags) : 0;
    i32 num;
    struct iovec iovecs[MAX_STACKALLOC_VECTORED_BUFFERS];
    struct iovec *piovecs = (bufferCount <= MAX_STACKALLOC_VECTORED_BUFFERS) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (piovecs == NULL)
    {
        errno = ENOMEM;
        _IoResult oom_result;
        oom_result.BytesTransferred = -1;
        oom_result.SocketError = _GetLastSocketError();
        return oom_result;
    }
    _Build(buffers, bufferCount, piovecs);
    msg.msg_iov = (struct iovec *)piovecs;
    num = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    _IoResult result;
    if (num >= 0)
    {
        if (inOutFlags != NULL)
        {
            *inOutFlags = _FromNativeSocketFlags(msg.msg_flags);
        }
        if (socketAddress != NULL)
        {
            *socketAddress = storage;
        }
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
    }
    return result;
}

/// <summary>
///     Receives data into multiple buffers from an Ipv6 socket address.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
/// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
/// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
/// <remarks>
///     Only the following flag values are honored:
///     <list type="bullet">
///         <item>
///             <description>
///                 <see cref="SocketFlags.OutOfBand" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Peek" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.DontRoute" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.Truncated" />
///             </description>
///         </item>
///         <item>
///             <description>
///                 <see cref="SocketFlags.ControlDataTruncated" />
///             </description>
///         </item>
///     </list>
///     Any other flags are silently ignored.
/// </remarks>
_IoResult _ReceiveFromVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in6 *socketAddress)
{
    _sockaddr_in6 storage;
    struct msghdr msg;
    memset(&msg, 0, sizeof(struct msghdr));
    msg.msg_name = &storage;
    msg.msg_namelen = sizeof(_sockaddr_in6);
    msg.msg_iovlen = bufferCount;
    i32 native_flags = (inOutFlags != NULL) ? _ToNativeSocketFlags(*inOutFlags) : 0;
    i32 num;
    struct iovec iovecs[MAX_STACKALLOC_VECTORED_BUFFERS];
    struct iovec *piovecs = (bufferCount <= MAX_STACKALLOC_VECTORED_BUFFERS) ? iovecs : (struct iovec *)malloc(sizeof(struct iovec) * bufferCount);
    if (piovecs == NULL)
    {
        errno = ENOMEM;
        _IoResult oom_result;
        oom_result.BytesTransferred = -1;
        oom_result.SocketError = _GetLastSocketError();
        return oom_result;
    }
    _Build(buffers, bufferCount, piovecs);
    msg.msg_iov = (struct iovec *)piovecs;
    num = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    _IoResult result;
    if (num >= 0)
    {
        if (inOutFlags != NULL)
        {
            *inOutFlags = _FromNativeSocketFlags(msg.msg_flags);
        }
        if (socketAddress != NULL)
        {
            *socketAddress = storage;
        }
        result.BytesTransferred = num;
        result.SocketError = _SOCKET_ERROR_SUCCESS;
    }
    else
    {
        result.BytesTransferred = -1;
        result.SocketError = _GetLastSocketError();
    }
    return result;
}

#endif
