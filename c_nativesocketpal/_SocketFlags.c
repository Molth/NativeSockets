#ifndef _SOCKETFLAGS_C
#define _SOCKETFLAGS_C

#ifndef _WIN32

#include "include/_SocketFlags.h"

#include <sys/socket.h>

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to its native unix integer representation.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
i32 _ToNativeSocketFlags(i32 flags)
{
    i32 native_flags = 0;
    if (flags & _SOCKET_FLAGS_OUT_OF_BAND)
    {
        native_flags |= MSG_OOB;
    }
    if (flags & _SOCKET_FLAGS_PEEK)
    {
        native_flags |= MSG_PEEK;
    }
    if (flags & _SOCKET_FLAGS_DONT_ROUTE)
    {
        native_flags |= MSG_DONTROUTE;
    }
#ifdef MSG_TRUNC
    if (flags & _SOCKET_FLAGS_TRUNCATED)
    {
        native_flags |= MSG_TRUNC;
    }
#endif
#ifdef MSG_CTRUNC
    if (flags & _SOCKET_FLAGS_CONTROL_DATA_TRUNCATED)
    {
        native_flags |= MSG_CTRUNC;
    }
#endif
    return native_flags;
}

/// <summary>
///     Converts a native unix socket flag integer value to a managed <see cref="SocketFlags" />.
/// </summary>
/// <param name="native_flags">The native integer value.</param>
/// <returns>The managed <see cref="SocketFlags" /> value.</returns>
i32 _FromNativeSocketFlags(i32 native_flags)
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
#ifdef MSG_TRUNC
    if (native_flags & MSG_TRUNC)
    {
        flags |= _SOCKET_FLAGS_TRUNCATED;
    }
#endif
#ifdef MSG_CTRUNC
    if (native_flags & MSG_CTRUNC)
    {
        flags |= _SOCKET_FLAGS_CONTROL_DATA_TRUNCATED;
    }
#endif
    return flags;
}

#endif

#endif
