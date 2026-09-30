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

/// <summary>
///     Creates an <see cref="IoResult" /> representing an operation that has succeeded.
/// </summary>
/// <param name="bytesTransferred">The number of bytes transferred by the operation.</param>
/// <returns>An <see cref="IoResult" /> with <see cref="System.Net.Sockets.SocketError.Success" />.</returns>
static _IoResult _IoResult_Ok(i32 bytesTransferred)
{
    return _IoResult_new(bytesTransferred, _SOCKET_ERROR_SUCCESS);
}

/// <summary>
///     Creates an <see cref="IoResult" /> representing an operation that has failed.
/// </summary>
/// <param name="socketError">The socket error that occurred.</param>
/// <returns>An <see cref="IoResult" /> with <c>-1</c> as the number of bytes transferred.</returns>
static _IoResult _IoResult_Err(i32 socketError)
{
    return _IoResult_new(-1, socketError);
}
