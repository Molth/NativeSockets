using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents the result of a socket I/O operation:
    ///     the number of bytes transferred if the operation succeeded,
    ///     or the socket error that occurred if the operation failed.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct IoResult : IEquatable<IoResult>, IComparable<IoResult>
    {
        /// <summary>
        ///     The tag of the operation result.
        /// </summary>
        private readonly IoTag _tag;

        /// <summary>
        ///     The number of bytes transferred, or the socket error value.
        /// </summary>
        private readonly int _value;

        /// <summary>
        ///     Gets the tag of the operation result.
        /// </summary>
        public IoTag Tag => _tag;

        /// <summary>
        ///     Gets the number of bytes transferred.
        /// </summary>
        /// <returns>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>The number of bytes transferred if the operation succeeded.</description>
        ///         </item>
        ///         <item>
        ///             <description><c>-1</c> if the operation failed.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="int.MinValue" /> if no tag has been assigned.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public int BytesTransferred => GetBytesTransferred();

        /// <summary>
        ///     Gets the socket error that occurred.
        /// </summary>
        /// <returns>
        ///     <list type="bullet">
        ///         <item>
        ///             <description><see cref="System.Net.Sockets.SocketError.Success" /> if the operation succeeded.</description>
        ///         </item>
        ///         <item>
        ///             <description>The socket error if the operation failed.</description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="System.Net.Sockets.SocketError" /> with value <see cref="int.MinValue" />
        ///                 if no tag has been assigned.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </returns>
        public SocketError SocketError => GetSocketError();

        /// <summary>
        ///     Initializes a new instance of the <see cref="IoResult" /> structure.
        /// </summary>
        /// <param name="tag">The tag of the operation result.</param>
        /// <param name="value">The number of bytes transferred, or the socket error value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IoResult(IoTag tag, int value)
        {
            _tag = tag;
            _value = value;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Ok(int bytesTransferred) => new(IoTag.Ok, bytesTransferred);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IoResult Err(SocketError socketError) => new(IoTag.Err, (int)socketError);

        /// <summary>
        ///     Gets the number of bytes transferred.
        /// </summary>
        /// <returns>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>The number of bytes transferred if the operation succeeded.</description>
        ///         </item>
        ///         <item>
        ///             <description><c>-1</c> if the operation failed.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="int.MinValue" /> if no tag has been assigned.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     Do not use a <c>switch</c> statement here;
        ///     keep the <c>if</c>-based checks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetBytesTransferred()
        {
            if (_tag == IoTag.Ok)
                return _value;

            if (_tag == IoTag.Err)
                return -1;

            return int.MinValue;
        }

        /// <summary>
        ///     Gets the socket error that occurred.
        /// </summary>
        /// <returns>
        ///     <list type="bullet">
        ///         <item>
        ///             <description><see cref="System.Net.Sockets.SocketError.Success" /> if the operation succeeded.</description>
        ///         </item>
        ///         <item>
        ///             <description>The socket error if the operation failed.</description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="System.Net.Sockets.SocketError" /> with value <see cref="int.MinValue" />
        ///                 if no tag has been assigned.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     Do not use a <c>switch</c> statement here;
        ///     keep the <c>if</c>-based checks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SocketError GetSocketError()
        {
            if (_tag == IoTag.Err)
                return (SocketError)_value;

            if (_tag == IoTag.Ok)
                return SocketError.Success;

            return (SocketError)int.MinValue;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public bool Equals(IoResult other) => SpanHelpers.Equals(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates
        ///     whether the current instance precedes, follows, or occurs in the same position in the sort order as the other
        ///     object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     <list type="table">
        ///         <listheader>
        ///             <term> Value</term><description> Meaning</description>
        ///         </listheader>
        ///         <item>
        ///             <term> Less than zero</term>
        ///             <description> This instance precedes <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///         <item>
        ///             <term> Zero</term>
        ///             <description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description>
        ///         </item>
        ///         <item>
        ///             <term> Greater than zero</term>
        ///             <description> This instance follows <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public int CompareTo(IoResult other) => SpanHelpers.Compare(ref Unsafe.AsRef(in this), ref other);

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