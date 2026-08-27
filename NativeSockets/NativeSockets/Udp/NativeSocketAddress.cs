using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents a native socket address structure that can hold either an Ipv4 or Ipv6 address.
    /// </summary>
    /// <remarks>
    ///     The structure has a fixed size of 28 bytes, which is sufficient for
    ///     both Ipv4 (16 bytes) and Ipv6 (28 bytes) addresses.
    ///     It is layout‑explicit to allow direct interpretation as
    ///     a byte buffer or as a properly aligned structure for native calls.
    ///     This type is used for low‑level socket operations that require raw address handling without allocation.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public unsafe struct NativeSocketAddress : IEquatable<NativeSocketAddress>, IComparable<NativeSocketAddress>
#if NET6_0_OR_GREATER
        , ISpanFormattable
#endif
    {
        /// <summary>
        ///     The raw buffer containing the socket address bytes.
        /// </summary>
        [FieldOffset(0)] private fixed byte _buffer[28];

        /// <summary>
        ///     The address family.
        /// </summary>
        [FieldOffset(0)] private ushort ss_family;

        /// <summary>
        ///     The port number in network byte order.
        /// </summary>
        [FieldOffset(2)] private ushort ss_port;

        /// <summary>
        ///     Represents a native Ipv4 socket address structure (<c>sockaddr_in</c>).
        /// </summary>
        [FieldOffset(0)] private sockaddr_in4 sin4;

        /// <summary>
        ///     Represents a native Ipv6 socket address structure (<c>sockaddr_in6</c>).
        /// </summary>
        [FieldOffset(0)] private sockaddr_in6 sin6;

        /// <summary>
        ///     Gets whether the address is an Ipv4 address.
        /// </summary>
        public readonly bool IsIpv4 => ss_family == SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;

        /// <summary>
        ///     Gets whether the address is an Ipv6 address.
        /// </summary>
        public readonly bool IsIpv6 => ss_family == SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;

        /// <summary>
        ///     Gets the address family of the socket address.
        /// </summary>
        public AddressFamily Family
        {
            readonly get => GetAddressFamily();
            set => SetAddressFamily(value);
        }

        /// <summary>
        ///     Gets or sets the port number of the socket address.
        /// </summary>
        /// <returns>An unsigned integer value indicating the port number of the socket address.</returns>
        public ushort Port
        {
            readonly get => WinSock2.NET_TO_HOST_16(ss_port);
            set => ss_port = WinSock2.HOST_TO_NET_16(value);
        }

        /// <summary>
        ///     Gets or sets the Ipv6 address scope identifier.
        /// </summary>
        /// <returns>An unsigned integer that specifies the scope of the address.</returns>
        public uint ScopeId
        {
            readonly get => sin6.sin6_scope_id;
            set => sin6.sin6_scope_id = value;
        }

        /// <summary>
        ///     Gets whether the socket address is an Ipv4-mapped Ipv6 address.
        /// </summary>
        /// <returns>
        ///     Returns true if the socket address is an Ipv4-mapped Ipv6 address;
        ///     otherwise, false.
        /// </returns>
        public readonly bool IsIpv4MappedToIpv6 => IsIpv6 && WinSock2.IsIpv4MappedToIpv6(ref Unsafe.AsRef(in sin6.sin6_addr[0]));

        /// <summary>
        ///     Gets the underlying buffer size of this.
        /// </summary>
        /// <returns>The underlying buffer size of this.</returns>
        public readonly int Size => IsIpv6 ? 28 : IsIpv4 ? 16 : 0;

        /// <summary>
        ///     Gets or sets the specified index element in the underlying buffer.
        /// </summary>
        /// <param name="offset">The array index element of the desired information.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">The specified index does not exist in the buffer.</exception>
        /// <returns>The value of the specified index element in the underlying buffer.</returns>
        public byte this[int offset]
        {
            readonly get
            {
                ThrowHelpers.ThrowIfGreaterThanOrEqual((uint)offset, (uint)Size, ExceptionArgument.offset);
                return _buffer[offset];
            }
            set
            {
                ThrowHelpers.ThrowIfGreaterThanOrEqual((uint)offset, (uint)Size, ExceptionArgument.offset);
                _buffer[offset] = value;
            }
        }

        /// <summary>
        ///     Maps the socket address object to an Ipv6 address.
        /// </summary>
        /// <returns>Returns socket address. An Ipv6 address.</returns>
        public readonly NativeSocketAddress MapToIpv6()
        {
            if (IsIpv6)
                return this;

            NativeSocketAddress address = this;
            address.ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            WinSock2.MapToIpv6(ref address.sin6.sin6_addr[0], address.sin4.sin4_addr);
            address.sin6.sin6_flowinfo = 0;
            address.sin6.sin6_scope_id = 0;
            return address;
        }

        /// <summary>
        ///     Maps the socket address object to an Ipv4 address.
        /// </summary>
        /// <returns>Returns socket address. An Ipv4 address.</returns>
        public readonly NativeSocketAddress MapToIpv4()
        {
            if (IsIpv4)
                return this;

            NativeSocketAddress address = this;
            address.ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
            address.sin4.sin4_addr = address.sin6.sin4_addr;
            SpanHelpers.Set(ref address._buffer[8], 0, 20);
            return address;
        }

        /// <summary>
        ///     Gets the underlying memory that can be passed to native OS calls.
        /// </summary>
        public Span<byte> Buffer => AsSpan().Slice(0, Size);

        /// <summary>
        ///     Gets the ip address of the endpoint.
        /// </summary>
        public Span<byte> Address => AsSpan().Slice(IsIpv6 ? 8 : IsIpv4 ? 4 : 0, IsIpv6 ? 16 : IsIpv4 ? 4 : 0);

        /// <summary>
        ///     Returns a span that represents the raw byte buffer of the address.
        /// </summary>
        /// <returns>A span of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref _buffer[0], 28);

        /// <summary>
        ///     Returns a span that represents the raw byte buffer of the address.
        /// </summary>
        /// <returns>A span of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<byte> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _buffer[0]), 28);

        /// <summary>
        ///     Determines the address family from a raw socket address byte span.
        /// </summary>
        /// <returns>The detected <see cref="AddressFamily" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly AddressFamily GetAddressFamily()
        {
            ushort result = ss_family;

            if (result == SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4)
                return AddressFamily.InterNetwork;

            if (result == SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6)
                return AddressFamily.InterNetworkV6;

            return (AddressFamily)result;
        }

        /// <summary>
        ///     Sets the address family from a raw <see cref="AddressFamily" /> value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAddressFamily(AddressFamily value)
        {
            switch (value)
            {
                case AddressFamily.InterNetwork:
                    ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
                    break;

                case AddressFamily.InterNetworkV6:
                    ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
                    break;

                default:
                    ss_family = (ushort)value;
                    break;
            }
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(NativeSocketAddress other) => SpanHelpers.Equals(ref Unsafe.AsRef(in this), ref other);

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
        public readonly int CompareTo(NativeSocketAddress other) => SpanHelpers.Compare(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public readonly override bool Equals(object? obj) => obj is NativeSocketAddress other && other.Equals(this);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        public readonly override int GetHashCode() => NativeHashCode.GetHashCode(this);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public static bool operator ==(NativeSocketAddress left, NativeSocketAddress right) => left.Equals(right);

        /// <summary>
        ///     Indicates whether the current object is not equal to another object.
        /// </summary>
        public static bool operator !=(NativeSocketAddress left, NativeSocketAddress right) => !left.Equals(right);

        /// <summary>
        ///     Returns information about the socket address.
        /// </summary>
        /// <returns>A string that contains information about this.</returns>
        public readonly override string ToString()
        {
            using (NativeScopedArray<char> array = Format(stackalloc char[256], this, out int chars))
            {
                Span<char> result = array.AsSpan().Slice(0, chars);
                return result.ToString();
            }
        }

        /// <summary>
        ///     Tries to format the current socket address into the provided span.
        /// </summary>
        /// <param name="destination">When this method returns, the socket address as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters written into the span.</param>
        /// <returns>
        ///     <see langword="true" /> if the formatting was successful;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public readonly bool TryFormat(Span<char> destination, out int charsWritten)
        {
            using (NativeScopedArray<char> array = Format(stackalloc char[256], this, out int chars))
            {
                Span<char> result = array.AsSpan().Slice(0, chars);
                if (result.TryCopyTo(destination))
                {
                    charsWritten = result.Length;
                    return true;
                }

                charsWritten = 0;
                return false;
            }
        }

        /// <summary>
        ///     Returns the string representation of the current socket address.
        /// </summary>
        /// <param name="_">The format specifier (ignored).</param>
        /// <param name="__">The format provider (ignored).</param>
        /// <returns>A string representation of the socket address.</returns>
        public readonly string ToString(string? _, IFormatProvider? __) => ToString();

        /// <summary>
        ///     Tries to format the current socket address into the provided span.
        /// </summary>
        /// <param name="destination">The span to receive the formatted characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters written.</param>
        /// <param name="_">The format specifier (ignored).</param>
        /// <param name="__">The format provider (ignored).</param>
        /// <returns><see langword="true" /> if the formatting succeeded; otherwise, <see langword="false" />.</returns>
        public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> _, IFormatProvider? __) => TryFormat(destination, out charsWritten);

        /// <summary>
        ///     Formats the socket address into a human-readable string representation.
        ///     The format is: <c>Family:Size:{byte1,byte2,...}</c>, where each byte is expressed as a decimal number.
        /// </summary>
        /// <param name="span">
        ///     A temporary buffer used to hold the formatted string. If the formatted string exceeds the
        ///     capacity of the stack-allocated buffer (typically 256 characters), this span provides
        ///     fallback storage via <see cref="NativeScopedArray{T}" />.
        /// </param>
        /// <param name="socketAddress">A reference to the socket address to format.</param>
        /// <param name="chars">When this method returns, contains the number of characters written to the temporary buffer.</param>
        /// <returns>A <see cref="NativeScopedArray{Char}" /> that owns the formatted character span.</returns>
        private static NativeScopedArray<char> Format(Span<char> span, in NativeSocketAddress socketAddress, out int chars)
        {
            ReadOnlySpan<char> family = socketAddress.Family.ToString();
            int maxLength = checked(family.Length + 1 + 10 + 2 + (socketAddress.Size - 2) * 4 + 1);

            NativeScopedArray<char> array = new NativeScopedArray<char>(span, maxLength);
            Span<char> destination = array.AsSpan();

            family.CopyTo(destination);
            int length = family.Length;

            destination[length++] = ':';

            socketAddress.Size.TryFormat(destination.Slice(length), out int charsWritten);

            length += charsWritten;

            destination[length++] = ':';
            destination[length++] = '{';

            ReadOnlySpan<byte> buffer = socketAddress.AsReadOnlySpan();
            for (int i = 2; i < socketAddress.Size; ++i)
            {
                if (i > 2)
                    destination[length++] = ',';

                buffer[i].TryFormat(destination.Slice(length), out charsWritten);

                length += charsWritten;
            }

            destination[length++] = '}';
            chars = length;
            return array;
        }
    }
}