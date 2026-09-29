/// <summary>
///     Gets the maximum number of vectored buffers that can be stack-allocated for vectored socket operations.
/// </summary>
#define MAX_STACKALLOC_VECTORED_BUFFERS 32

/// <summary>
///     Initializes a new instance of the <see cref="IoResult" /> structure.
/// </summary>
/// <param name="bytesTransferred">The number of bytes transferred by the operation.</param>
/// <param name="socketError">
///     The socket error that occurred,
///     or <see cref="System.Net.Sockets.SocketError.Success" /> if the operation succeeded.
/// </param>
static _IoResult _IoResult_new(i32 bytesTransferred, i32 socketError)
{
    _IoResult result;
    result.BytesTransferred = bytesTransferred;
    result.SocketError = socketError;
    return result;
}
