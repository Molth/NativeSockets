#include "include/_nativesocketpal.h"

#include <string.h>
#include <stdlib.h>

#ifdef _WIN32

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>

#else

#include <sys/socket.h>
#include <sys/uio.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <poll.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include <errno.h>

#endif

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
#define _AF_INET_4 AF_INET

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
#define _AF_INET_6 AF_INET6

#if defined(__APPLE__) || defined(__FreeBSD__) || defined(__NetBSD__) || defined(__OpenBSD__) || defined(__DragonFly__)

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V4 = (_AF_INET_4 << 8) | 16;

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V6 = (_AF_INET_6 << 8) | 28;

#else

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V4 = _AF_INET_4;

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
const u16 _ADDRESS_FAMILY_INTER_NETWORK_V6 = _AF_INET_6;

#endif

#ifdef _WIN32
#include "_win32.c"
#else
#include "_unix.c"
#include "_SelectMode.c"
#include "_SelectModeFlags.c"
#include "_SocketError.c"
#include "_SocketFlags.c"
#include "_SocketOptionLevel.c"
#include "_SocketOptionName.c"
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
    i32 result = inet_pton(_AF_INET_4, (char *)ip, &__socketAddress_native.sin4_addr);
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
    i32 addressFamily = _AF_INET_6;
    if (strchr((char *)ip, ':') == NULL)
    {
        addressFamily = _AF_INET_4;
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
    if (inet_ntop(_AF_INET_4, &socketAddress->sin4_addr, (char *)ip, (socklen_t)ipLength) == NULL)
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
    if (inet_ntop(_AF_INET_6, socketAddress->sin6_addr, (char *)ip, (socklen_t)ipLength) == NULL)
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
    struct addrinfo *result = NULL;
    hints.ai_family = _AF_INET_4;
    hints.ai_socktype = SOCK_DGRAM;
    hints.ai_protocol = IPPROTO_UDP;
    if (getaddrinfo((char *)hostName, NULL, &hints, &result) != 0)
    {
        return _SOCKET_ERROR_FAULT;
    }
    struct addrinfo *p;
    for (p = result; p != NULL; p = p->ai_next)
    {
        if (p->ai_addr != NULL && p->ai_addrlen >= sizeof(struct sockaddr_in) && p->ai_family == _AF_INET_4)
        {
            struct sockaddr_in *sin = (struct sockaddr_in *)p->ai_addr;
            socketAddress->sin4_addr = sin->sin_addr.s_addr;
            freeaddrinfo(result);
            return _SOCKET_ERROR_SUCCESS;
        }
    }
    freeaddrinfo(result);
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
    struct addrinfo *result = NULL;
    hints.ai_family = _AF_INET_6;
    hints.ai_socktype = SOCK_DGRAM;
    hints.ai_protocol = IPPROTO_UDP;
    if (getaddrinfo((char *)hostName, NULL, &hints, &result) != 0)
    {
        return _SOCKET_ERROR_FAULT;
    }
    struct addrinfo *p;
    for (p = result; p != NULL; p = p->ai_next)
    {
        if (p->ai_addr != NULL && p->ai_addrlen >= sizeof(struct sockaddr_in6) && p->ai_family == _AF_INET_6)
        {
            struct sockaddr_in6 *sin6 = (struct sockaddr_in6 *)p->ai_addr;
            memcpy(socketAddress->sin6_addr, &sin6->sin6_addr, 16);
            freeaddrinfo(result);
            return _SOCKET_ERROR_SUCCESS;
        }
    }
    freeaddrinfo(result);
    return _SOCKET_ERROR_HOST_NOT_FOUND;
}
