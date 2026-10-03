/// <summary>
///     Initializes a new instance of the <see cref="IoResult" /> structure.
/// </summary>
/// <param name="tag">The tag of the operation result.</param>
/// <param name="value">The number of bytes transferred, or the socket error value.</param>
static _IoResult _IoResult_new(i32 tag, i32 value)
{
    _IoResult result;
    result._tag = tag;
    result._value = value;
    return result;
}

/// <summary>
///     Creates an <see cref="IoResult" /> representing an operation that has succeeded.
/// </summary>
/// <param name="bytesTransferred">The number of bytes transferred.</param>
/// <returns>An <see cref="IoResult" /> with <see cref="System.Net.Sockets.SocketError.Success" />.</returns>
/// <remarks>
///     The <paramref name="bytesTransferred" /> value is expected to be <c>&gt;= 0</c>.
///     No runtime validation is performed.
/// </remarks>
static _IoResult _IoResult_Ok(i32 bytesTransferred)
{
    return _IoResult_new(_IO_TAG_OK, bytesTransferred);
}

/// <summary>
///     Creates an <see cref="IoResult" /> representing an operation that has failed.
/// </summary>
/// <param name="socketError">The socket error that occurred.</param>
/// <returns>An <see cref="IoResult" /> with <c>-1</c> as the number of bytes transferred.</returns>
/// <remarks>
///     The <paramref name="socketError" /> value is expected not to be
///     <see cref="System.Net.Sockets.SocketError.Success" />.
///     No runtime validation is performed.
/// </remarks>
static _IoResult _IoResult_Err(i32 socketError)
{
    return _IoResult_new(_IO_TAG_ERR, socketError);
}
