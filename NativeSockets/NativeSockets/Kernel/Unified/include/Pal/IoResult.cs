using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents the result of a socket I/O operation,
    ///     containing the number of bytes transferred,
    ///     and the socket error that occurred.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct IoResult : IEquatable<IoResult>
    {
        /// <summary>
        ///     The number of bytes transferred by the operation.
        /// </summary>
        public readonly int BytesTransferred;

        /// <summary>
        ///     The socket error that occurred,
        ///     or <see cref="System.Net.Sockets.SocketError.Success" /> if the operation succeeded.
        /// </summary>
        public readonly SocketError SocketError;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IoResult" /> structure.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred by the operation.</param>
        /// <param name="socketError">
        ///     The socket error that occurred,
        ///     or <see cref="System.Net.Sockets.SocketError.Success" /> if the operation succeeded.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IoResult(int bytesTransferred, SocketError socketError)
        {
            BytesTransferred = bytesTransferred;
            SocketError = socketError;
        }

        /// <summary>
        ///     Creates an <see cref="IoResult" /> representing an operation that has succeeded.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred by the operation.</param>
        /// <returns>An <see cref="IoResult" /> with <see cref="System.Net.Sockets.SocketError.Success" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Ok(int bytesTransferred)
        {
            Debug.Assert(bytesTransferred >= 0);
            return new IoResult(bytesTransferred, SocketError.Success);
        }

        /// <summary>
        ///     Creates an <see cref="IoResult" /> representing an operation that has failed.
        /// </summary>
        /// <param name="socketError">The socket error that occurred.</param>
        /// <returns>An <see cref="IoResult" /> with <c>-1</c> as the number of bytes transferred.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Err(SocketError socketError)
        {
            Debug.Assert(socketError != SocketError.Success);
            return new IoResult(-1, socketError);
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public bool Equals(IoResult other) => SpanHelpers.Equals(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) => obj is IoResult other && other.Equals(this);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => NativeHashCode.GetHashCode(this);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public static bool operator ==(IoResult left, IoResult right) => left.Equals(right);

        /// <summary>
        ///     Indicates whether the current object is not equal to another object.
        /// </summary>
        public static bool operator !=(IoResult left, IoResult right) => !left.Equals(right);
    }
}