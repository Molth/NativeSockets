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
    _SOCKET_FLAGS_MULTICAST = 2048,
    _SOCKET_FLAGS_PARTIAL = 32768
} _SocketFlags;

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to
///     its native integer representation for send operations.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
i32 _ToNativeSendSocketFlags(i32 flags);

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to
///     its native integer representation for receive operations.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
i32 _ToNativeReceiveSocketFlags(i32 flags);

/// <summary>
///     Converts a native socket flag integer value to
///     a managed <see cref="SocketFlags" /> for receive operations.
/// </summary>
/// <param name="native_flags">The native integer value.</param>
/// <returns>The managed <see cref="SocketFlags" /> value.</returns>
i32 _FromNativeReceiveSocketFlags(i32 native_flags);

#endif
