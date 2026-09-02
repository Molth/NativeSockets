#include "_nativesocketpal.h"

#include <string.h>
#include <stdlib.h>

#ifdef _WIN32

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>

typedef i32 _socklen_t;

#else

#include <sys/types.h>
#include <sys/socket.h>
#include <sys/uio.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <fcntl.h>
#include <poll.h>
#include <unistd.h>
#include <errno.h>

#ifndef MSG_TRUNC
#define MSG_TRUNC 0x20
#endif

#ifndef MSG_CTRUNC
#define MSG_CTRUNC 0x08
#endif

typedef socklen_t _socklen_t;

#endif

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
#define _AF_INET4 AF_INET

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
#define _AF_INET6 AF_INET6

#if defined(__APPLE__) || defined(__FreeBSD__) || defined(__NetBSD__) || defined(__OpenBSD__) || defined(__DragonFly__)

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V4 = (_AF_INET4 << 8) | 16;

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V6 = (_AF_INET6 << 8) | 28;

#else

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V4 = _AF_INET4;

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V6 = _AF_INET6;

#endif

#ifndef _WIN32
/// <summary>
///     Maps a native error number to the corresponding <see cref="SocketError" /> value.
/// </summary>
/// <param name="e">The native error number (errno).</param>
/// <returns>The corresponding <see cref="SocketError" /> value.</returns>
static i32 _FromNativeErrno(i32 e)
{
    switch (e)
    {
    case 0:
        return _SOCKET_ERROR_SUCCESS;
    case EINTR:
        return _SOCKET_ERROR_INTERRUPTED;
    case EACCES:
        return _SOCKET_ERROR_ACCESS_DENIED;
    case EFAULT:
        return _SOCKET_ERROR_FAULT;
    case EINVAL:
        return _SOCKET_ERROR_INVALID_ARGUMENT;
    case EMFILE:
        return _SOCKET_ERROR_TOO_MANY_OPEN_SOCKETS;
    case EWOULDBLOCK:
        return _SOCKET_ERROR_WOULD_BLOCK;
    case EINPROGRESS:
        return _SOCKET_ERROR_IN_PROGRESS;
    case EALREADY:
        return _SOCKET_ERROR_ALREADY_IN_PROGRESS;
    case ENOTSOCK:
        return _SOCKET_ERROR_NOT_SOCKET;
    case EDESTADDRREQ:
        return _SOCKET_ERROR_DESTINATION_ADDRESS_REQUIRED;
    case EMSGSIZE:
        return _SOCKET_ERROR_MESSAGE_SIZE;
    case EPROTOTYPE:
        return _SOCKET_ERROR_PROTOCOL_TYPE;
    case ENOPROTOOPT:
        return _SOCKET_ERROR_PROTOCOL_OPTION;
    case ESOCKTNOSUPPORT:
        return _SOCKET_ERROR_SOCKET_NOT_SUPPORTED;
    case EOPNOTSUPP:
        return _SOCKET_ERROR_OPERATION_NOT_SUPPORTED;
    case EPFNOSUPPORT:
        return _SOCKET_ERROR_ADDRESS_FAMILY_NOT_SUPPORTED;
    case EAFNOSUPPORT:
        return _SOCKET_ERROR_ADDRESS_FAMILY_NOT_SUPPORTED;
    case EADDRINUSE:
        return _SOCKET_ERROR_ADDRESS_ALREADY_IN_USE;
    case EADDRNOTAVAIL:
        return _SOCKET_ERROR_ADDRESS_NOT_AVAILABLE;
    case ENETDOWN:
        return _SOCKET_ERROR_NETWORK_DOWN;
    case ENETUNREACH:
        return _SOCKET_ERROR_NETWORK_UNREACHABLE;
    case ENETRESET:
        return _SOCKET_ERROR_NETWORK_RESET;
    case ECONNABORTED:
        return _SOCKET_ERROR_CONNECTION_ABORTED;
    case ECONNRESET:
        return _SOCKET_ERROR_CONNECTION_RESET;
    case ENOBUFS:
        return _SOCKET_ERROR_NO_BUFFER_SPACE_AVAILABLE;
    case EISCONN:
        return _SOCKET_ERROR_IS_CONNECTED;
    case ENOTCONN:
        return _SOCKET_ERROR_NOT_CONNECTED;
    case ESHUTDOWN:
        return _SOCKET_ERROR_SHUTDOWN;
    case ETIMEDOUT:
        return _SOCKET_ERROR_TIMED_OUT;
    case ECONNREFUSED:
        return _SOCKET_ERROR_CONNECTION_REFUSED;
    case EHOSTDOWN:
        return _SOCKET_ERROR_HOST_DOWN;
    case EHOSTUNREACH:
        return _SOCKET_ERROR_HOST_UNREACHABLE;
#ifdef EPROCLIM
    case EPROCLIM:
        return _SOCKET_ERROR_PROCESS_LIMIT;
#endif
    case EPROTONOSUPPORT:
        return _SOCKET_ERROR_PROTOCOL_NOT_SUPPORTED;
    default:
        return _SOCKET_ERROR_SOCKET_ERROR;
    }
}
#endif

#ifndef _WIN32
/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to its native unix integer representation.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
static i32 _ToNativeSocketFlags(i32 flags)
{
    i32 native = 0;
    if (flags & _SOCKET_FLAGS_OUT_OF_BAND)
    {
        native |= MSG_OOB;
    }
    if (flags & _SOCKET_FLAGS_PEEK)
    {
        native |= MSG_PEEK;
    }
    if (flags & _SOCKET_FLAGS_DONT_ROUTE)
    {
        native |= MSG_DONTROUTE;
    }
    if (flags & _SOCKET_FLAGS_TRUNCATED)
    {
        native |= MSG_TRUNC;
    }
    if (flags & _SOCKET_FLAGS_CONTROL_DATA_TRUNCATED)
    {
        native |= MSG_CTRUNC;
    }
    return native;
}
#endif

#ifndef _WIN32
/// <summary>
///     Converts a native unix socket flag integer value to a managed <see cref="SocketFlags" />.
/// </summary>
/// <param name="native_flags">The native integer value.</param>
/// <returns>The managed <see cref="SocketFlags" /> value.</returns>
static i32 _FromNativeSocketFlags(i32 native_flags)
{
    i32 flags = 0;
    if (native_flags & MSG_OOB)
    {
        flags |= _SOCKET_FLAGS_OUT_OF_BAND;
    }
    if (native_flags & MSG_PEEK)
    {
        flags |= _SOCKET_FLAGS_PEEK;
    }
    if (native_flags & MSG_DONTROUTE)
    {
        flags |= _SOCKET_FLAGS_DONT_ROUTE;
    }
    if (native_flags & MSG_TRUNC)
    {
        flags |= _SOCKET_FLAGS_TRUNCATED;
    }
    if (native_flags & MSG_CTRUNC)
    {
        flags |= _SOCKET_FLAGS_CONTROL_DATA_TRUNCATED;
    }
    return flags;
}
#endif

#ifndef _WIN32
/// <summary>
///     Converts a managed <see cref="SocketOptionLevel" /> to the native unix socket option level value.
/// </summary>
/// <param name="level">The managed socket option level.</param>
/// <returns>
///     The native integer value for the socket option level.
///     For <see cref="SocketOptionLevel.Socket" />, returns 1 (SOL_SOCKET).
/// </returns>
static i32 _ToNativeSocketOptionLevel(i32 level)
{
    if (level == _SOCKET_OPTION_LEVEL_SOCKET)
    {
        return SOL_SOCKET;
    }
    if (level == _SOCKET_OPTION_LEVEL_IP)
    {
        return IPPROTO_IP;
    }
    if (level == _SOCKET_OPTION_LEVEL_IPV6)
    {
        return IPPROTO_IPV6;
    }
    if (level == _SOCKET_OPTION_LEVEL_TCP)
    {
        return IPPROTO_TCP;
    }
    if (level == _SOCKET_OPTION_LEVEL_UDP)
    {
        return IPPROTO_UDP;
    }
    return level;
}
#endif

#ifndef _WIN32
/// <summary>
///     Converts a managed <see cref="SocketOptionName" /> to the native unix socket option name value
///     for the specified level, handling level‑specific mappings.
/// </summary>
/// <param name="level">The socket option level, which determines the namespace of the option name.</param>
/// <param name="name">The managed socket option name.</param>
/// <returns>The native integer value for the socket option name.</returns>
static i32 _ToNativeSocketOptionName(i32 level, i32 name)
{
    if (level == _SOCKET_OPTION_LEVEL_SOCKET)
    {
        switch (name)
        {
        case _SOCKET_OPTION_NAME_DEBUG:
            return SO_DEBUG;
        case _SOCKET_OPTION_NAME_ACCEPT_CONNECTION:
            return SO_ACCEPTCONN;
        case _SOCKET_OPTION_NAME_REUSE_ADDRESS:
            return SO_REUSEADDR;
        case _SOCKET_OPTION_NAME_KEEP_ALIVE:
            return SO_KEEPALIVE;
        case _SOCKET_OPTION_NAME_DONT_ROUTE:
            return SO_DONTROUTE;
        case _SOCKET_OPTION_NAME_BROADCAST:
            return SO_BROADCAST;
        case _SOCKET_OPTION_NAME_LINGER:
            return SO_LINGER;
        case _SOCKET_OPTION_NAME_OUT_OF_BAND_INLINE:
            return SO_OOBINLINE;
        case _SOCKET_OPTION_NAME_SEND_BUFFER:
            return SO_SNDBUF;
        case _SOCKET_OPTION_NAME_RECEIVE_BUFFER:
            return SO_RCVBUF;
        case _SOCKET_OPTION_NAME_SEND_LOW_WATER:
            return SO_SNDLOWAT;
        case _SOCKET_OPTION_NAME_RECEIVE_LOW_WATER:
            return SO_RCVLOWAT;
        case _SOCKET_OPTION_NAME_SEND_TIMEOUT:
            return SO_SNDTIMEO;
        case _SOCKET_OPTION_NAME_RECEIVE_TIMEOUT:
            return SO_RCVTIMEO;
        case _SOCKET_OPTION_NAME_ERROR:
            return SO_ERROR;
        case _SOCKET_OPTION_NAME_TYPE:
            return SO_TYPE;
        default:
            return name;
        }
    }
    if (level == _SOCKET_OPTION_LEVEL_IP)
    {
        switch (name)
        {
        case _SOCKET_OPTION_NAME_IP_OPTIONS:
            return IP_OPTIONS;
        case _SOCKET_OPTION_NAME_HEADER_INCLUDED:
            return IP_HDRINCL;
        case _SOCKET_OPTION_NAME_TYPE_OF_SERVICE:
            return IP_TOS;
        case _SOCKET_OPTION_NAME_IP_TIME_TO_LIVE:
            return IP_TTL;
        case _SOCKET_OPTION_NAME_MULTICAST_INTERFACE:
            return IP_MULTICAST_IF;
        case _SOCKET_OPTION_NAME_MULTICAST_TIME_TO_LIVE:
            return IP_MULTICAST_TTL;
        case _SOCKET_OPTION_NAME_MULTICAST_LOOPBACK:
            return IP_MULTICAST_LOOP;
        case _SOCKET_OPTION_NAME_ADD_MEMBERSHIP:
            return IP_ADD_MEMBERSHIP;
        case _SOCKET_OPTION_NAME_DROP_MEMBERSHIP:
            return IP_DROP_MEMBERSHIP;
        case _SOCKET_OPTION_NAME_DONT_FRAGMENT:
#ifdef IP_DONTFRAG
            return IP_DONTFRAG;
#else
            return name;
#endif
        default:
            return name;
        }
    }
    if (level == _SOCKET_OPTION_LEVEL_IPV6)
    {
        switch (name)
        {
        case _SOCKET_OPTION_NAME_IPV6_HOP_LIMIT:
            return IPV6_UNICAST_HOPS;
        case _SOCKET_OPTION_NAME_IPV6_V6ONLY:
            return IPV6_V6ONLY;
        default:
            return name;
        }
    }
    if (level == _SOCKET_OPTION_LEVEL_TCP)
    {
        switch (name)
        {
        case _SOCKET_OPTION_NAME_NO_DELAY:
            return TCP_NODELAY;
        default:
            return name;
        }
    }
    return name;
}
#endif

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
#endif

/// <summary>
///     Writes the 12‑byte prefix to an Ipv6 address.
/// </summary>
/// <param name="addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
static void _WriteIpv6Prefix(u8 *addr)
{
    memset(addr, 0, 10);
    addr[10] = 0xFF;
    addr[11] = 0xFF;
}

/// <summary>
///     Maps the Ipv4 address to an Ipv6 address.
/// </summary>
/// <param name="out_addr">The 16‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
/// <param name="sin4_addr">The 4‑byte span containing the Ipv4 address data.</param>
static void _MapIpv4ToIpv6(u8 *sin6_addr, u32 sin4_addr)
{
    _WriteIpv6Prefix(sin6_addr);
    memcpy(sin6_addr + 12, &sin4_addr, 4);
}

/// <summary>
///     Normalizes the address to an Ipv6 address.
/// </summary>
/// <param name="out_addr">Pointer to the target Ipv6 socket address structure to fill.</param>
/// <param name="storage">Reference to the source address storage, which may contain an Ipv4 or Ipv6 address.</param>
static void _NormalizeToIpv6(_sockaddr_in6 *out_addr, const _sockaddr_storage *storage)
{
    if (storage->ss_family == _ADDRESS_FAMILY_INTER_NETWORK_V4)
    {
        const _sockaddr_in4 *in4 = (const _sockaddr_in4 *)storage;
        out_addr->sin6_family = _ADDRESS_FAMILY_INTER_NETWORK_V6;
        out_addr->sin6_port = in4->sin4_port;
        out_addr->sin6_flowinfo = 0;
        _MapIpv4ToIpv6(out_addr->sin6_addr, in4->sin4_addr);
        out_addr->sin6_scope_id = 0;
    }
    else if (storage->ss_family == _ADDRESS_FAMILY_INTER_NETWORK_V6)
    {
        const _sockaddr_in6 *in6 = (const _sockaddr_in6 *)storage;
        *out_addr = *in6;
    }
}

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
u16 _GetAddressFamilyInterNetworkV4(void)
{
    return _ADDRESS_FAMILY_INTER_NETWORK_V4;
}

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
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
#ifdef _WIN32
    return (i32)WSAGetLastError();
#else
    return _FromNativeErrno(errno);
#endif
}

/// <summary>
///     Starts up the platform-specific socket subsystem.
/// </summary>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _Startup(void)
{
#ifdef _WIN32
    WSADATA wsaData;
    return WSAStartup(514, &wsaData);
#else
    return _SOCKET_ERROR_SUCCESS;
#endif
}

/// <summary>
///     Cleans up the platform-specific socket subsystem.
/// </summary>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _Cleanup(void)
{
#ifdef _WIN32
    return WSACleanup();
#else
    return _SOCKET_ERROR_SUCCESS;
#endif
}

/// <summary>
///     Creates a native socket handle.
/// </summary>
/// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
/// <returns>The native socket handle, or -1 on error.</returns>
isize _Create(i32 ipv6)
{
    i32 family = ipv6 ? _AF_INET6 : _AF_INET4;
#ifdef _WIN32
    SOCKET s = WSASocketW(family, SOCK_DGRAM, IPPROTO_UDP, NULL, 0, WSA_FLAG_OVERLAPPED);
    if (s != -1)
    {
        DWORD dwBytesReturned = 0;
        BOOL bNewBehavior = FALSE;
        DWORD ioctl = IOC_IN | IOC_VENDOR | 12; /* SIO_UDP_CONNRESET */
        WSAIoctl(s, ioctl, &bNewBehavior, sizeof(BOOL), NULL, 0, &dwBytesReturned, NULL, NULL);
    }
    return (isize)s;
#else
    i32 s = socket(family, SOCK_DGRAM, IPPROTO_UDP);
    return (isize)s;
#endif
}

/// <summary>
///     Closes a native socket handle.
/// </summary>
/// <param name="socket">The native socket handle to close.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _Close(isize socket)
{
#ifdef _WIN32
    return closesocket((SOCKET)socket);
#else
    return close((i32)socket);
#endif
}

/// <summary>
///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="dualMode">true to enable dual-mode; false to disable.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _SetDualModeIpv6(isize socket, i32 dualMode)
{
    i32 optionValue = dualMode ? 0 : 1;
    return _SetOption(socket, _SOCKET_OPTION_LEVEL_IPV6, _SOCKET_OPTION_NAME_IPV6_V6ONLY, (u8 *)&optionValue, sizeof(i32));
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
#ifdef _WIN32
    return bind((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
#else
    return bind((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
#endif
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
#ifdef _WIN32
    return bind((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
#else
    return bind((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
#endif
}

/// <summary>
///     Connects a socket to an Ipv4 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _ConnectIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
#ifdef _WIN32
    return WSAConnect((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), NULL, NULL, NULL, NULL);
#else
    return connect((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
#endif
}

/// <summary>
///     Connects a socket to an Ipv6 endpoint.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _ConnectIpv6(isize socket, _sockaddr_in6 *socketAddress)
{
#ifdef _WIN32
    return WSAConnect((SOCKET)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), NULL, NULL, NULL, NULL);
#else
    return connect((i32)socket, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
#endif
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
#ifdef _WIN32
    i32 result = setsockopt((SOCKET)socket, level, name, (const u8 *)value, (socklen_t)length);
#else
    i32 native_level = _ToNativeSocketOptionLevel(level);
    i32 native_name = _ToNativeSocketOptionName(level, name);
    i32 result = setsockopt((i32)socket, native_level, native_name, value, (socklen_t)length);
#endif
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
#ifdef _WIN32
    i32 result = getsockopt((SOCKET)socket, level, name, (u8 *)value, (socklen_t *)length);
#else
    i32 native_level = _ToNativeSocketOptionLevel(level);
    i32 native_name = _ToNativeSocketOptionName(level, name);
    i32 result = getsockopt((i32)socket, native_level, native_name, value, (socklen_t *)length);
#endif
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
#ifdef _WIN32
    u_long nonBlocking = blocking ? 0 : 1;
    i32 result = ioctlsocket((SOCKET)socket, FIONBIO, &nonBlocking);
#else
    i32 flags = fcntl((i32)socket, F_GETFL, 0);
    if (flags == -1)
    {
        return _GetLastSocketError();
    }
    flags = blocking ? (flags & ~O_NONBLOCK) : (flags | O_NONBLOCK);
    i32 result = fcntl((i32)socket, F_SETFL, flags);
#endif
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
#ifdef _WIN32
    isize fdset[2];
    fdset[0] = 1;
    fdset[1] = socket;
    i32 result;
    if (microseconds != -1)
    {
        struct timeval tv;
        _MicrosecondsToTimeValue(microseconds, &tv);
        result = select(0,
                        (mode == _SELECT_MODE_SELECT_READ) ? (fd_set *)fdset : NULL,
                        (mode == _SELECT_MODE_SELECT_WRITE) ? (fd_set *)fdset : NULL,
                        (mode == _SELECT_MODE_SELECT_ERROR) ? (fd_set *)fdset : NULL,
                        &tv);
    }
    else
    {
        result = select(0,
                        (mode == _SELECT_MODE_SELECT_READ) ? (fd_set *)fdset : NULL,
                        (mode == _SELECT_MODE_SELECT_WRITE) ? (fd_set *)fdset : NULL,
                        (mode == _SELECT_MODE_SELECT_ERROR) ? (fd_set *)fdset : NULL,
                        NULL);
    }
    if (result == SOCKET_ERROR)
    {
        *status = 0;
        return _GetLastSocketError();
    }
    *status = FD_ISSET(socket, (fd_set *)fdset);
    return _SOCKET_ERROR_SUCCESS;
#else
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
#endif
}

/// <summary>
///     Polls a socket for pending events.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="microseconds">The timeout in microseconds.</param>
/// <param name="mode">The select mode.</param>
/// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _PollFlags(isize socket, i32 microseconds, i32 mode, i32 *status)
{
#ifdef _WIN32
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
    if ((mode & _SELECT_MODE_FLAGS_READ) == 0)
    {
        readFds = NULL;
    }
    if ((mode & _SELECT_MODE_FLAGS_WRITE) == 0)
    {
        writeFds = NULL;
    }
    if ((mode & _SELECT_MODE_FLAGS_ERROR) == 0)
    {
        errorFds = NULL;
    }
    i32 result;
    if (microseconds != -1)
    {
        struct timeval tv;
        _MicrosecondsToTimeValue(microseconds, &tv);
        result = select(0,
                        (fd_set *)readFds,
                        (fd_set *)writeFds,
                        (fd_set *)errorFds,
                        &tv);
    }
    else
    {
        result = select(0,
                        (fd_set *)readFds,
                        (fd_set *)writeFds,
                        (fd_set *)errorFds,
                        NULL);
    }
    *status = 0;
    if (result == SOCKET_ERROR)
    {
        return _GetLastSocketError();
    }
    if (readFds != NULL && FD_ISSET(socket, (fd_set *)readFds))
    {
        *status |= _SELECT_MODE_FLAGS_READ;
    }
    if (writeFds != NULL && FD_ISSET(socket, (fd_set *)writeFds))
    {
        *status |= _SELECT_MODE_FLAGS_WRITE;
    }
    if (errorFds != NULL && FD_ISSET(socket, (fd_set *)errorFds))
    {
        *status |= _SELECT_MODE_FLAGS_ERROR;
    }
    return _SOCKET_ERROR_SUCCESS;
#else
    short events = 0;
    if ((mode & _SELECT_MODE_FLAGS_READ) != 0)
    {
        events |= POLLIN;
    }
    if ((mode & _SELECT_MODE_FLAGS_WRITE) != 0)
    {
        events |= POLLOUT;
    }
    if ((mode & _SELECT_MODE_FLAGS_ERROR) != 0)
    {
        events |= POLLPRI;
    }
    struct pollfd pfd;
    pfd.fd = (i32)socket;
    pfd.events = events;
    pfd.revents = 0;
    i32 timeout = (microseconds == -1) ? -1 : (microseconds / 1000);
    *status = 0;
    i32 result = poll(&pfd, 1, timeout);
    if (result == -1)
    {
        return _GetLastSocketError();
    }
    if ((pfd.revents & (POLLIN | POLLHUP)) != 0)
    {
        *status |= _SELECT_MODE_FLAGS_READ;
    }
    if ((pfd.revents & POLLOUT) != 0)
    {
        *status |= _SELECT_MODE_FLAGS_WRITE;
    }
    if ((pfd.revents & (POLLERR | POLLPRI)) != 0)
    {
        *status |= _SELECT_MODE_FLAGS_ERROR;
    }
    return _SOCKET_ERROR_SUCCESS;
#endif
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
#ifdef _WIN32
    return (i32)send((SOCKET)socket, (const u8 *)buffer, length, socketFlags);
#else
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    return (i32)send((i32)socket, buffer, (usize)length, native_flags);
#endif
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
#ifdef _WIN32
        return (i32)sendto((SOCKET)socket, (const u8 *)buffer, length, socketFlags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
#else
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        return (i32)sendto((i32)socket, buffer, (usize)length, native_flags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4));
#endif
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
#ifdef _WIN32
        return (i32)sendto((SOCKET)socket, (const u8 *)buffer, length, socketFlags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
#else
        i32 native_flags = _ToNativeSocketFlags(socketFlags);
        return (i32)sendto((i32)socket, buffer, (usize)length, native_flags, (const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6));
#endif
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
#ifdef _WIN32
    return (i32)recv((SOCKET)socket, (u8 *)buffer, length, socketFlags);
#else
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    return (i32)recv((i32)socket, buffer, (usize)length, native_flags);
#endif
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
    _sockaddr_storage storage;
    memset(&storage, 0, sizeof(_sockaddr_storage));
    _socklen_t addr_len = sizeof(_sockaddr_storage);
    i32 result;
#ifdef _WIN32
    result = (i32)recvfrom((SOCKET)socket, (u8 *)buffer, length, socketFlags, (struct sockaddr *)&storage, &addr_len);
#else
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    result = (i32)recvfrom((i32)socket, buffer, (usize)length, native_flags, (struct sockaddr *)&storage, &addr_len);
#endif
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
    _sockaddr_storage storage;
    memset(&storage, 0, sizeof(_sockaddr_storage));
    _socklen_t addr_len = sizeof(_sockaddr_storage);
    i32 result;
#ifdef _WIN32
    result = (i32)recvfrom((SOCKET)socket, (u8 *)buffer, length, socketFlags, (struct sockaddr *)&storage, &addr_len);
#else
    i32 native_flags = _ToNativeSocketFlags(socketFlags);
    result = (i32)recvfrom((i32)socket, buffer, (usize)length, native_flags, (struct sockaddr *)&storage, &addr_len);
#endif
    if (result >= 0 && socketAddress != NULL)
    {
        _NormalizeToIpv6(socketAddress, &storage);
    }
    return result;
}

/* ========================================================================= */
/* SendMessage / ReceiveMessage (scatter/gather I/O)                         */
/* ========================================================================= */

#ifdef _WIN32

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

#else

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

#endif

/// <summary>
///     Sends a message on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
/// <returns>The number of bytes sent, or -1 on error.</returns>
i32 _SendMessage(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags)
{
#ifdef _WIN32
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
        free(pwsabufs);
    return (result == 0) ? bytesSent : -1;
#else
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
#endif
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
i32 _SendMessageToIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in4 *socketAddress)
{
    if (socketAddress == NULL)
    {
        return _SendMessage(socket, buffers, bufferCount, socketFlags);
    }
#ifdef _WIN32
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
#else
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
#endif
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
i32 _SendMessageToIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in6 *socketAddress)
{
    if (socketAddress == NULL)
    {
        return _SendMessage(socket, buffers, bufferCount, socketFlags);
    }
#ifdef _WIN32
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
#else
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
#endif
}

/// <summary>
///     Receives a message on a connected socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
/// <param name="bufferCount">The number of buffers.</param>
/// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
/// <returns>The number of bytes received, or -1 on error.</returns>
i32 _ReceiveMessage(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *socketFlags)
{
#ifdef _WIN32
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesRecv = 0;
    DWORD flags = (socketFlags != NULL) ? *socketFlags : 0;
    i32 result = WSARecv((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesRecv, &flags, NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    if (socketFlags != NULL)
    {
        *socketFlags = (i32)flags;
    }
    if (result != 0)
    {
        return -1;
    }
    return bytesRecv;
#else
    i32 native_flags = (socketFlags != NULL) ? _ToNativeSocketFlags(*socketFlags) : 0;
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
    if (socketFlags != NULL)
    {
        *socketFlags = _FromNativeSocketFlags(msg.msg_flags);
    }
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
#endif
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
i32 _ReceiveMessageFromIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *socketFlags, _sockaddr_in4 *socketAddress)
{
    _sockaddr_storage storage;
    memset(&storage, 0, sizeof(_sockaddr_storage));
#ifdef _WIN32
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesRecv = 0;
    DWORD flags = (socketFlags != NULL) ? *socketFlags : 0;
    INT addr_len = sizeof(_sockaddr_storage);
    i32 result = WSARecvFrom((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesRecv, &flags, (struct sockaddr *)&storage, &addr_len, NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    if (socketFlags != NULL)
    {
        *socketFlags = (i32)flags;
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
#else
    i32 native_flags = (socketFlags != NULL) ? _ToNativeSocketFlags(*socketFlags) : 0;
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
    msg.msg_namelen = sizeof(_sockaddr_storage);
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (socketFlags != NULL)
    {
        *socketFlags = _FromNativeSocketFlags(msg.msg_flags);
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
#endif
}

i32 _ReceiveMessageFromIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *socketFlags, _sockaddr_in6 *socketAddress)
{
    _sockaddr_storage storage;
    memset(&storage, 0, sizeof(_sockaddr_storage));
#ifdef _WIN32
    WSABUF wsabufs[16];
    WSABUF *pwsabufs = (bufferCount <= 16) ? wsabufs : (WSABUF *)malloc(sizeof(WSABUF) * bufferCount);
    if (!pwsabufs)
    {
        return -1;
    }
    _Build(buffers, bufferCount, pwsabufs);
    i32 bytesRecv = 0;
    DWORD flags = (socketFlags != NULL) ? *socketFlags : 0;
    INT addr_len = sizeof(_sockaddr_storage);
    i32 result = WSARecvFrom((SOCKET)socket, (LPWSABUF)pwsabufs, bufferCount, &bytesRecv, &flags, (struct sockaddr *)&storage, &addr_len, NULL, NULL);
    if (pwsabufs != wsabufs)
    {
        free(pwsabufs);
    }
    if (socketFlags != NULL)
    {
        *socketFlags = (i32)flags;
    }
    if (result != 0)
    {
        return -1;
    }
    if (socketAddress != NULL)
    {
        _NormalizeToIpv6(socketAddress, &storage);
    }
    return bytesRecv;
#else
    i32 native_flags = (socketFlags != NULL) ? _ToNativeSocketFlags(*socketFlags) : 0;
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
    msg.msg_namelen = sizeof(_sockaddr_storage);
    msg.msg_iov = (struct iovec *)piovecs;
    msg.msg_iovlen = bufferCount;
    i32 result = (i32)recvmsg((i32)socket, &msg, native_flags);
    if (socketFlags != NULL)
    {
        *socketFlags = _FromNativeSocketFlags(msg.msg_flags);
    }
    if (result >= 0 && socketAddress != NULL)
    {
        _NormalizeToIpv6(socketAddress, &storage);
    }
    if (piovecs != iovecs)
    {
        free(piovecs);
    }
    return result;
#endif
}

/// <summary>
///     Gets the local name (address) of an Ipv4 socket.
/// </summary>
/// <param name="socket">The socket handle.</param>
/// <param name="socketAddress">Pointer to the Ipv4 address structure to receive the name.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
i32 _GetNameIpv4(isize socket, _sockaddr_in4 *socketAddress)
{
    _sockaddr_storage storage;
    _socklen_t addr_len = sizeof(_sockaddr_storage);
    memset(&storage, 0, sizeof(_sockaddr_storage));
#ifdef _WIN32
    i32 result = getsockname((SOCKET)socket, (struct sockaddr *)&storage, &addr_len);
#else
    i32 result = getsockname((i32)socket, (struct sockaddr *)&storage, &addr_len);
#endif
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
    _sockaddr_storage storage;
    _socklen_t addr_len = sizeof(_sockaddr_storage);
    memset(&storage, 0, sizeof(_sockaddr_storage));
#ifdef _WIN32
    i32 result = getsockname((SOCKET)socket, (struct sockaddr *)&storage, &addr_len);
#else
    i32 result = getsockname((i32)socket, (struct sockaddr *)&storage, &addr_len);
#endif
    if (result == 0 && socketAddress != NULL)
    {
        _NormalizeToIpv6(socketAddress, &storage);
    }
    return result;
}

/// <summary>
///     Sets the Ipv4 address in the given address structure.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <param name="ip">The ip address as a span of bytes.</param>
/// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
i32 _SetIpIpv4(_sockaddr_in4 *socketAddress, const u8 *ip, i32 ipLength)
{
    if (ip[ipLength - 1] != '\0')
    {
        return _SOCKET_ERROR_INVALID_ARGUMENT;
    }
    _sockaddr_in4 __socketAddress_native = *socketAddress;
    i32 result = inet_pton(_AF_INET4, (char *)ip, &__socketAddress_native.sin4_addr);
    if (result == 1)
    {
        *socketAddress = __socketAddress_native;
        return _SOCKET_ERROR_SUCCESS;
    }
    return (result == 0) ? _SOCKET_ERROR_INVALID_ARGUMENT : _SOCKET_ERROR_FAULT;
}

/// <summary>
///     Sets the Ipv6 address in the given address structure.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <param name="ip">The ip address as a span of bytes.</param>
/// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
i32 _SetIpIpv6(_sockaddr_in6 *socketAddress, const u8 *ip, i32 ipLength)
{
    if (ip[ipLength - 1] != '\0')
    {
        return _SOCKET_ERROR_INVALID_ARGUMENT;
    }
    _sockaddr_in6 __socketAddress_native = *socketAddress;
    u8 *addr = __socketAddress_native.sin6_addr;
    i32 addressFamily = _AF_INET6;
    if (strchr((char *)ip, ':') == NULL)
    {
        addressFamily = _AF_INET4;
        _WriteIpv6Prefix(addr);
        addr += 12;
    }
    i32 result = inet_pton(addressFamily, (char *)ip, addr);
    if (result == 1)
    {
        *socketAddress = __socketAddress_native;
        return _SOCKET_ERROR_SUCCESS;
    }
    return (result == 0) ? _SOCKET_ERROR_INVALID_ARGUMENT : _SOCKET_ERROR_FAULT;
}

/// <summary>
///     Retrieves the Ipv4 address from a socket address structure.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <param name="ip">A span to receive the address bytes.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
i32 _GetIpIpv4(_sockaddr_in4 *socketAddress, u8 *ip, i32 ipLength)
{
    if (inet_ntop(_AF_INET4, &socketAddress->sin4_addr, (char *)ip, (socklen_t)ipLength) == NULL)
    {
        return _SOCKET_ERROR_FAULT;
    }
    return _SOCKET_ERROR_SUCCESS;
}

/// <summary>
///     Retrieves the Ipv6 address from a socket address structure.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <param name="ip">A span to receive the address bytes.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
i32 _GetIpIpv6(_sockaddr_in6 *socketAddress, u8 *ip, i32 ipLength)
{
    if (inet_ntop(_AF_INET6, socketAddress->sin6_addr, (char *)ip, (socklen_t)ipLength) == NULL)
    {
        return _SOCKET_ERROR_FAULT;
    }
    return _SOCKET_ERROR_SUCCESS;
}

/// <summary>
///     Sets the host name (reverse DNS) for an Ipv4 address.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <param name="hostName">The host name as a span of bytes.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _SetHostNameIpv4(_sockaddr_in4 *socketAddress, const u8 *hostName, i32 hostNameLength)
{
    if (hostName[hostNameLength - 1] != '\0')
    {
        return _SOCKET_ERROR_INVALID_ARGUMENT;
    }
    struct addrinfo hints;
    memset(&hints, 0, sizeof(struct addrinfo));
    struct addrinfo *results = NULL;
    hints.ai_family = _AF_INET4;
    if (getaddrinfo((char *)hostName, NULL, &hints, &results) != 0)
    {
        return _SOCKET_ERROR_FAULT;
    }
    struct addrinfo *p;
    for (p = results; p != NULL; p = p->ai_next)
    {
        if (p->ai_addr != NULL && p->ai_addrlen >= sizeof(struct sockaddr_in) && p->ai_family == _AF_INET4)
        {
            struct sockaddr_in *sin = (struct sockaddr_in *)p->ai_addr;
            socketAddress->sin4_addr = sin->sin_addr.s_addr;
            freeaddrinfo(results);
            return _SOCKET_ERROR_SUCCESS;
        }
    }
    freeaddrinfo(results);
    return _SOCKET_ERROR_HOST_NOT_FOUND;
}

/// <summary>
///     Sets the host name (reverse DNS) for an Ipv6 address.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
/// <param name="hostName">The host name as a span of bytes.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
i32 _SetHostNameIpv6(_sockaddr_in6 *socketAddress, const u8 *hostName, i32 hostNameLength)
{
    if (hostName[hostNameLength - 1] != '\0')
    {
        return _SOCKET_ERROR_INVALID_ARGUMENT;
    }
    struct addrinfo hints;
    memset(&hints, 0, sizeof(struct addrinfo));
    struct addrinfo *results = NULL;
    hints.ai_family = _AF_INET6;
    if (getaddrinfo((char *)hostName, NULL, &hints, &results) != 0)
    {
        return _SOCKET_ERROR_FAULT;
    }
    struct addrinfo *p;
    for (p = results; p != NULL; p = p->ai_next)
    {
        if (p->ai_addr != NULL && p->ai_addrlen >= sizeof(struct sockaddr_in6) && p->ai_family == _AF_INET6)
        {
            struct sockaddr_in6 *sin6 = (struct sockaddr_in6 *)p->ai_addr;
            memcpy(socketAddress->sin6_addr, &sin6->sin6_addr, 16);
            freeaddrinfo(results);
            return _SOCKET_ERROR_SUCCESS;
        }
    }
    freeaddrinfo(results);
    return _SOCKET_ERROR_HOST_NOT_FOUND;
}

/// <summary>
///     Gets the host name (reverse DNS) from an Ipv4 address.
/// </summary>
/// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
/// <param name="hostName">A span to receive the host name bytes.</param>
/// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
i32 _GetHostNameIpv4(_sockaddr_in4 *socketAddress, u8 *hostName, i32 hostNameLength)
{
#ifdef _WIN32
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), (char *)hostName, (DWORD)hostNameLength, NULL, 0, 0) == 0)
#else
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in4), (char *)hostName, (socklen_t)hostNameLength, NULL, 0, 0) == 0)
#endif
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
#ifdef _WIN32
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), (char *)hostName, (DWORD)hostNameLength, NULL, 0, 0) == 0)
#else
    if (getnameinfo((const struct sockaddr *)socketAddress, sizeof(_sockaddr_in6), (char *)hostName, (socklen_t)hostNameLength, NULL, 0, 0) == 0)
#endif
    {
        return _SOCKET_ERROR_SUCCESS;
    }
    return _SOCKET_ERROR_FAULT;
}
