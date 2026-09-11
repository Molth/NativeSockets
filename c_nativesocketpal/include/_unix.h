#ifndef _UNIX_H
#define _UNIX_H

#ifndef _WIN32

#include <sys/socket.h>

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
#define _ADDRESS_FAMILY_INTER_NETWORK_V4 ((u16)((_AF_INET_4 << 8) | 16))

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
#define _ADDRESS_FAMILY_INTER_NETWORK_V6 ((u16)((_AF_INET_6 << 8) | 28))

#else

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
#define _ADDRESS_FAMILY_INTER_NETWORK_V4 ((u16)_AF_INET_4)

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
#define _ADDRESS_FAMILY_INTER_NETWORK_V6 ((u16)_AF_INET_6)

#endif

typedef socklen_t _socklen_t;

#endif

#endif
