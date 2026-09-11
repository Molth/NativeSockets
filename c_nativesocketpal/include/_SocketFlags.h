#ifndef _SOCKETFLAGS_H
#define _SOCKETFLAGS_H

#include "_Types.h"

/// <summary>
///     Flags for socket send and receive operations.
/// </summary>
typedef enum _SocketFlags
{
    _SOCKET_FLAGS_NONE = 0,
    _SOCKET_FLAGS_OUT_OF_BAND = 1,
    _SOCKET_FLAGS_PEEK = 2,
    _SOCKET_FLAGS_DONT_ROUTE = 4,
    _SOCKET_FLAGS_TRUNCATED = 256,
    _SOCKET_FLAGS_CONTROL_DATA_TRUNCATED = 512,
    _SOCKET_FLAGS_BROADCAST = 1024,
    _SOCKET_FLAGS_PARTIAL = 32768
} _SocketFlags;

#ifndef _WIN32

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to its native unix integer representation.
/// </summary>
i32 _ToNativeSocketFlags(i32 flags);

/// <summary>
///     Converts a native unix socket flag integer value to a managed <see cref="SocketFlags" />.
/// </summary>
i32 _FromNativeSocketFlags(i32 native_flags);

#endif

#endif
