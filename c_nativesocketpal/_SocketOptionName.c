#ifndef _SOCKETOPTIONNAME_C
#define _SOCKETOPTIONNAME_C

#ifndef _WIN32

#include "include/_SocketOptionName.h"

#ifdef __APPLE__
#define __APPLE_USE_RFC_3542
#endif

#include <netinet/in.h>
#include <netinet/tcp.h>
#include <sys/socket.h>

/// <summary>
///     Converts a managed <see cref="SocketOptionName" /> to the native unix socket option name value
///     for the specified level, handling level‑specific mappings.
/// </summary>
/// <param name="level">The socket option level, which determines the namespace of the option name.</param>
/// <param name="name">The managed socket option name.</param>
/// <returns>
///     The native integer value for the socket option name, or -1 when the option is not supported
///     on the current platform. Returning -1 guarantees that the subsequent option call fails
///     instead of passing through an unmapped value that could behave unexpectedly.
/// </returns>
i32 _ToNativeSocketOptionName(i32 level, i32 name)
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
        case _SOCKET_OPTION_NAME_USE_LOOPBACK:
#ifdef SO_USELOOPBACK
            return SO_USELOOPBACK;
#else
            return -1;
#endif
        case _SOCKET_OPTION_NAME_REUSE_UNICAST_PORT:
            return SO_REUSEPORT;
        case _SOCKET_OPTION_NAME_EXCLUSIVE_ADDRESS_USE:
            return SO_REUSEADDR;
        default:
            return -1;
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
#elif defined(IP_MTU_DISCOVER)
            return IP_MTU_DISCOVER;
#else
            return -1;
#endif
        case _SOCKET_OPTION_NAME_PACKET_INFORMATION:
#ifdef IP_PKTINFO
            return IP_PKTINFO;
#else
            return -1;
#endif
        case _SOCKET_OPTION_NAME_ADD_SOURCE_MEMBERSHIP:
            return IP_ADD_SOURCE_MEMBERSHIP;
        case _SOCKET_OPTION_NAME_DROP_SOURCE_MEMBERSHIP:
            return IP_DROP_SOURCE_MEMBERSHIP;
        case _SOCKET_OPTION_NAME_BLOCK_SOURCE:
            return IP_BLOCK_SOURCE;
        case _SOCKET_OPTION_NAME_UNBLOCK_SOURCE:
            return IP_UNBLOCK_SOURCE;
        default:
            return -1;
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
        case _SOCKET_OPTION_NAME_MULTICAST_INTERFACE:
            return IPV6_MULTICAST_IF;
        case _SOCKET_OPTION_NAME_MULTICAST_TIME_TO_LIVE:
            return IPV6_MULTICAST_HOPS;
        case _SOCKET_OPTION_NAME_MULTICAST_LOOPBACK:
            return IPV6_MULTICAST_LOOP;
        case _SOCKET_OPTION_NAME_ADD_MEMBERSHIP:
            return IPV6_JOIN_GROUP;
        case _SOCKET_OPTION_NAME_DROP_MEMBERSHIP:
            return IPV6_LEAVE_GROUP;
        case _SOCKET_OPTION_NAME_PACKET_INFORMATION:
#ifdef IPV6_RECVPKTINFO
            return IPV6_RECVPKTINFO;
#else
            return -1;
#endif
        default:
            return -1;
        }
    }
    if (level == _SOCKET_OPTION_LEVEL_TCP)
    {
        switch (name)
        {
        case _SOCKET_OPTION_NAME_NO_DELAY:
            return TCP_NODELAY;
        case _SOCKET_OPTION_NAME_DONT_ROUTE:
            return TCP_KEEPCNT;
        case _SOCKET_OPTION_NAME_BLOCK_SOURCE:
            return TCP_KEEPINTVL;
        case _SOCKET_OPTION_NAME_ADD_SOURCE_MEMBERSHIP:
            return TCP_FASTOPEN;
        case _SOCKET_OPTION_NAME_TCP_KEEPALIVE_TIME:
#ifdef TCP_KEEPALIVE
            return TCP_KEEPALIVE;
#elif defined(TCP_KEEPIDLE)
            return TCP_KEEPIDLE;
#else
            return -1;
#endif
        default:
            return -1;
        }
    }
    return -1;
}

#endif

#endif
