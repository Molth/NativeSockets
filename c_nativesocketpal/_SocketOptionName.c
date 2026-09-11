#ifndef _SOCKETOPTIONNAME_C
#define _SOCKETOPTIONNAME_C

#ifndef _WIN32

#include "include/_SocketOptionName.h"

#include <netinet/in.h>
#include <netinet/tcp.h>
#include <sys/socket.h>

/// <summary>
///     Converts a managed <see cref="SocketOptionName" /> to the native unix socket option name value
///     for the specified level, handling level‑specific mappings.
/// </summary>
/// <param name="level">The socket option level, which determines the namespace of the option name.</param>
/// <param name="name">The managed socket option name.</param>
/// <returns>The native integer value for the socket option name.</returns>
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

#endif
