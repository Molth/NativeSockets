#ifndef _SOCKETOPTIONLEVEL_H
#define _SOCKETOPTIONLEVEL_H

#include "_Types.h"

/// <summary>
///     Socket option levels.
/// </summary>
typedef enum _SocketOptionLevel
{
    _SOCKET_OPTION_LEVEL_SOCKET = 65535,
    _SOCKET_OPTION_LEVEL_IP = 0,
    _SOCKET_OPTION_LEVEL_IPV6 = 41,
    _SOCKET_OPTION_LEVEL_TCP = 6,
    _SOCKET_OPTION_LEVEL_UDP = 17
} _SocketOptionLevel;

#ifndef _WIN32

/// <summary>
///     Converts a managed <see cref="SocketOptionLevel" /> to the native unix socket option level value.
/// </summary>
i32 _ToNativeSocketOptionLevel(i32 level);

#endif

#endif
