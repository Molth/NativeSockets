#ifndef _SOCKETFLAGS_C
#define _SOCKETFLAGS_C

#include "include/_SocketFlags.h"

#ifdef _WIN32

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to
///     its native Windows integer representation for send operations.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
i32 _ToNativeSendSocketFlags(i32 flags)
{
    return flags & _SOCKET_FLAGS_DONT_ROUTE;
}

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to
///     its native Windows integer representation for receive operations.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
i32 _ToNativeReceiveSocketFlags(i32 flags)
{
    return flags & _SOCKET_FLAGS_PEEK;
}

/// <summary>
///     Converts a native Windows socket flag integer value to
///     a managed <see cref="SocketFlags" /> for receive operations.
/// </summary>
/// <param name="native_flags">The native integer value.</param>
/// <returns>The managed <see cref="SocketFlags" /> value.</returns>
i32 _FromNativeReceiveSocketFlags(i32 native_flags)
{
    return native_flags & (_SOCKET_FLAGS_PEEK | _SOCKET_FLAGS_TRUNCATED | _SOCKET_FLAGS_PARTIAL);
}

#else

#include <sys/socket.h>

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to
///     its native integer representation for send operations.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
i32 _ToNativeSendSocketFlags(i32 flags)
{
    return (flags & _SOCKET_FLAGS_DONT_ROUTE) ? MSG_DONTROUTE : 0;
}

/// <summary>
///     Converts a managed <see cref="SocketFlags" /> value to
///     its native integer representation for receive operations.
/// </summary>
/// <param name="flags">The managed flags.</param>
/// <returns>The native integer value.</returns>
i32 _ToNativeReceiveSocketFlags(i32 flags)
{
    return (flags & _SOCKET_FLAGS_PEEK) ? MSG_PEEK : 0;
}

/// <summary>
///     Converts a native socket flag integer value to
///     a managed <see cref="SocketFlags" /> for receive operations.
/// </summary>
/// <param name="native_flags">The native integer value.</param>
/// <returns>The managed <see cref="SocketFlags" /> value.</returns>
i32 _FromNativeReceiveSocketFlags(i32 native_flags)
{
    return (native_flags & MSG_TRUNC) ? _SOCKET_FLAGS_TRUNCATED : 0;
}

#endif

#endif
