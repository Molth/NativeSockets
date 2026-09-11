#ifndef _SOCKETOPTIONLEVEL_C
#define _SOCKETOPTIONLEVEL_C

#ifndef _WIN32

#include "include/_SocketOptionLevel.h"

#include <netinet/in.h>
#include <sys/socket.h>

/// <summary>
///     Converts a managed <see cref="SocketOptionLevel" /> to the native unix socket option level value.
/// </summary>
/// <param name="level">The managed socket option level.</param>
/// <returns>
///     The native integer value for the socket option level.
///     For <see cref="SocketOptionLevel.Socket" />, returns 1 (SOL_SOCKET).
/// </returns>
i32 _ToNativeSocketOptionLevel(i32 level)
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

#endif
