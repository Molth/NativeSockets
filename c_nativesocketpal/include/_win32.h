#ifndef _WIN32_H
#define _WIN32_H

#ifdef _WIN32

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
#define _AF_INET_4 AF_INET

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
#define _AF_INET_6 AF_INET6

/// <summary>
///     Gets the address family value for Ipv4 used by the current platform.
/// </summary>
#define _ADDRESS_FAMILY_INTER_NETWORK_V4 ((u16)_AF_INET_4)

/// <summary>
///     Gets the address family value for Ipv6 used by the current platform.
/// </summary>
#define _ADDRESS_FAMILY_INTER_NETWORK_V6 ((u16)_AF_INET_6)

typedef i32 _socklen_t;

/// <summary>
///     Winsock IOCTL code used with <c>WSAIoctl</c> to control whether a UDP socket returns
///     <c>WSAECONNRESET</c> when an ICMP port unreachable message is received.
///     By default, Windows UDP sockets report connection reset errors; this code is used to disable
///     that behavior by setting the associated Boolean option to <c>FALSE</c>.
/// </summary>
#define SIO_UDP_CONNRESET (IOC_IN | IOC_VENDOR | 12)

#endif

#endif
