#ifndef _SOCKETERROR_C
#define _SOCKETERROR_C

#ifndef _WIN32

#include "include/_SocketError.h"

#include <errno.h>

/// <summary>
///     Maps a native error number to the corresponding <see cref="SocketError" /> value.
/// </summary>
/// <param name="e">The native error number (errno).</param>
/// <returns>The corresponding <see cref="SocketError" /> value.</returns>
i32 _FromNativeErrno(i32 e)
{
    switch (e)
    {
    case 0:
        return _SOCKET_ERROR_SUCCESS;
    case EINTR:
        return _SOCKET_ERROR_INTERRUPTED;
    case EPERM:
        return _SOCKET_ERROR_ACCESS_DENIED;
    case EACCES:
        return _SOCKET_ERROR_ACCESS_DENIED;
    case EFAULT:
        return _SOCKET_ERROR_FAULT;
    case ENOENT:
        return _SOCKET_ERROR_ADDRESS_NOT_AVAILABLE;
    case EINVAL:
        return _SOCKET_ERROR_INVALID_ARGUMENT;
    case ENXIO:
        return _SOCKET_ERROR_HOST_NOT_FOUND;
    case EMFILE:
        return _SOCKET_ERROR_TOO_MANY_OPEN_SOCKETS;
    case ENFILE:
        return _SOCKET_ERROR_TOO_MANY_OPEN_SOCKETS;
    case EWOULDBLOCK:
        return _SOCKET_ERROR_WOULD_BLOCK;
    case EINPROGRESS:
        return _SOCKET_ERROR_IN_PROGRESS;
    case EBADF:
        return _SOCKET_ERROR_OPERATION_ABORTED;
    case ECANCELED:
        return _SOCKET_ERROR_OPERATION_ABORTED;
    case EALREADY:
        return _SOCKET_ERROR_ALREADY_IN_PROGRESS;
    case ENOTSOCK:
        return _SOCKET_ERROR_NOT_SOCKET;
    case EDESTADDRREQ:
        return _SOCKET_ERROR_DESTINATION_ADDRESS_REQUIRED;
    case EMSGSIZE:
        return _SOCKET_ERROR_MESSAGE_SIZE;
    case EPROTO:
        return _SOCKET_ERROR_PROTOCOL_NOT_SUPPORTED;
    case EPROTOTYPE:
        return _SOCKET_ERROR_PROTOCOL_TYPE;
    case ENOPROTOOPT:
        return _SOCKET_ERROR_PROTOCOL_OPTION;
    case ESOCKTNOSUPPORT:
        return _SOCKET_ERROR_SOCKET_NOT_SUPPORTED;
    case ENOTSUP:
        return _SOCKET_ERROR_OPERATION_NOT_SUPPORTED;
#if EOPNOTSUPP != ENOTSUP
    case EOPNOTSUPP:
        return _SOCKET_ERROR_OPERATION_NOT_SUPPORTED;
#endif
    case EPFNOSUPPORT:
        return _SOCKET_ERROR_PROTOCOL_FAMILY_NOT_SUPPORTED;
    case EAFNOSUPPORT:
        return _SOCKET_ERROR_ADDRESS_FAMILY_NOT_SUPPORTED;
    case EADDRINUSE:
        return _SOCKET_ERROR_ADDRESS_ALREADY_IN_USE;
    case EADDRNOTAVAIL:
        return _SOCKET_ERROR_ADDRESS_NOT_AVAILABLE;
    case ENETDOWN:
        return _SOCKET_ERROR_NETWORK_DOWN;
    case ENETUNREACH:
        return _SOCKET_ERROR_NETWORK_UNREACHABLE;
    case ENETRESET:
        return _SOCKET_ERROR_NETWORK_RESET;
    case ECONNABORTED:
        return _SOCKET_ERROR_CONNECTION_ABORTED;
    case ECONNRESET:
        return _SOCKET_ERROR_CONNECTION_RESET;
    case ENOMEM:
        return _SOCKET_ERROR_NO_BUFFER_SPACE_AVAILABLE;
    case ENOBUFS:
        return _SOCKET_ERROR_NO_BUFFER_SPACE_AVAILABLE;
    case EISCONN:
        return _SOCKET_ERROR_IS_CONNECTED;
    case ENOTCONN:
        return _SOCKET_ERROR_NOT_CONNECTED;
    case EPIPE:
        return _SOCKET_ERROR_SHUTDOWN;
    case ESHUTDOWN:
        return _SOCKET_ERROR_SHUTDOWN;
    case ETIMEDOUT:
        return _SOCKET_ERROR_TIMED_OUT;
    case ECONNREFUSED:
        return _SOCKET_ERROR_CONNECTION_REFUSED;
    case EHOSTDOWN:
        return _SOCKET_ERROR_HOST_DOWN;
    case EHOSTUNREACH:
        return _SOCKET_ERROR_HOST_UNREACHABLE;
#ifdef EPROCLIM
    case EPROCLIM:
        return _SOCKET_ERROR_PROCESS_LIMIT;
#endif
    case EPROTONOSUPPORT:
        return _SOCKET_ERROR_PROTOCOL_NOT_SUPPORTED;
    default:
        return _SOCKET_ERROR_SOCKET_ERROR;
    }
}

#endif

#endif
