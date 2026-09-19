using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents a native socket address structure that can hold either an <c>Ipv4</c> or <c>Ipv6</c> socket address.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 The structure has a fixed size of 28 bytes, which is sufficient for
    ///                 both <c>Ipv4</c> (16 bytes) and <c>Ipv6</c> (28 bytes) socket addresses.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 It is layout-explicit to allow direct interpretation as
    ///                 a byte buffer or as a properly aligned structure for native calls.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 This type is used for low-level socket operations that require raw address handling without allocation.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public unsafe struct NativeSocketAddress : IEquatable<NativeSocketAddress>, IComparable<NativeSocketAddress>
    {
        /// <summary>
        ///     The raw buffer containing the socket address bytes.
        /// </summary>
        [FieldOffset(0)] private fixed byte _buffer[28];

        /// <summary>
        ///     The address family.
        /// </summary>
        [FieldOffset(0)] private ushort _ss_family;

        /// <summary>
        ///     The port number in network byte order.
        /// </summary>
        [FieldOffset(2)] private ushort _ss_port;

        /// <summary>
        ///     Represents a native Ipv4 socket address structure (<c>sockaddr_in</c>).
        /// </summary>
        [FieldOffset(0)] private sockaddr_in4 _sin4;

        /// <summary>
        ///     Represents a native Ipv6 socket address structure (<c>sockaddr_in6</c>).
        /// </summary>
        [FieldOffset(0)] private sockaddr_in6 _sin6;

        /// <summary>
        ///     Gets whether the socket address is an Ipv4 socket address.
        /// </summary>
        public readonly bool IsIpv4 => _ss_family == SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;

        /// <summary>
        ///     Gets whether the socket address is an Ipv6 socket address.
        /// </summary>
        public readonly bool IsIpv6 => _ss_family == SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;

        /// <summary>
        ///     Gets whether the socket address is an Ipv4-mapped Ipv6 ip.
        /// </summary>
        /// <returns>
        ///     Returns true if the socket address is an Ipv4-mapped Ipv6 ip;
        ///     otherwise, false.
        /// </returns>
        public readonly bool IsIpv4MappedToIpv6 => IsIpv6 && WinSock2.IsIpv4MappedToIpv6(ref Unsafe.AsRef(in _sin6.sin6_addr[0]));

        /// <summary>
        ///     Gets the address family of the socket address.
        /// </summary>
        public AddressFamily Family
        {
            readonly get => GetAddressFamily();
            set => SetAddressFamily(value);
        }

        /// <summary>
        ///     Gets the ip of the socket address.
        /// </summary>
        public Span<byte> Ip => AsSpan().Slice(IsIpv6 ? 8 : IsIpv4 ? 4 : 0, IsIpv6 ? 16 : IsIpv4 ? 4 : 0);

        /// <summary>
        ///     Gets or sets the port number of the socket address.
        /// </summary>
        /// <returns>An unsigned integer value indicating the port number of the socket address.</returns>
        public ushort Port
        {
            readonly get => WinSock2.NET_TO_HOST_16(_ss_port);
            set => _ss_port = WinSock2.HOST_TO_NET_16(value);
        }

        /// <summary>
        ///     Gets or sets the Ipv6 socket address scope id.
        /// </summary>
        /// <returns>An unsigned integer that specifies the scope id of the socket address.</returns>
        public uint ScopeId
        {
            readonly get => _sin6.sin6_scope_id;
            set => _sin6.sin6_scope_id = value;
        }

        /// <summary>
        ///     Gets the underlying buffer size of this.
        /// </summary>
        /// <returns>The underlying buffer size of this.</returns>
        public readonly int Size => IsIpv6 ? 28 : IsIpv4 ? 16 : 0;

        /// <summary>
        ///     Gets or sets the specified index element in the underlying buffer.
        /// </summary>
        /// <param name="index">The array index element of the desired information.</param>
        /// <returns>The value of the specified index element in the underlying buffer.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified index does not exist in the buffer.</exception>
        public byte this[int index]
        {
            readonly get
            {
                ThrowHelpers.ThrowIfGreaterThanOrEqual((uint)index, (uint)Size, ExceptionArgument.index);
                return _buffer[index];
            }
            set
            {
                ThrowHelpers.ThrowIfGreaterThanOrEqual((uint)index, (uint)Size, ExceptionArgument.index);
                _buffer[index] = value;
            }
        }

        /// <summary>
        ///     Maps the socket address object to an Ipv6 socket address.
        /// </summary>
        /// <returns>Returns socket address. An Ipv6 socket address.</returns>
        public readonly NativeSocketAddress MapToIpv6()
        {
            if (IsIpv6)
                return this;

            NativeSocketAddress address = this;
            address._ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            WinSock2.MapIpv4ToIpv6(ref address._sin6.sin6_addr[0], address._sin4.sin4_addr);
            address._sin6.sin6_flowinfo = 0;
            address._sin6.sin6_scope_id = 0;
            return address;
        }

        /// <summary>
        ///     Maps the socket address object to an Ipv4 socket address.
        /// </summary>
        /// <returns>Returns socket address. An Ipv4 socket address.</returns>
        public readonly NativeSocketAddress MapToIpv4()
        {
            if (IsIpv4)
                return this;

            NativeSocketAddress address = this;
            address._ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
            address._sin4.sin4_addr = address._sin6.sin4_addr;
            SpanHelpers.Set(ref address._buffer[8], 0, 20);
            return address;
        }

        /// <summary>
        ///     Gets the underlying memory that can be passed to native OS calls.
        /// </summary>
        public Span<byte> Buffer => AsSpan().Slice(0, Size);

        /// <summary>
        ///     Returns a debug-friendly representation of the raw socket address bytes.
        /// </summary>
        public readonly string DebugView => GetDebugView();

        /// <summary>
        ///     Returns a debug-friendly representation of the raw socket address bytes.
        /// </summary>
        private readonly string GetDebugView()
        {
            Span<char> chars = stackalloc char[NativeSocketAddressPal.FORMAT_MAX_CHARS];
            NativeSocketAddressPal.FormatDebugView(ref chars, this);
            return chars.ToString();
        }

        /// <summary>
        ///     Returns a span that represents the raw (28 bytes) buffer of the socket address.
        /// </summary>
        /// <returns>A span of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref _buffer[0], 28);

        /// <summary>
        ///     Returns a read-only span that represents the raw (28 bytes) buffer of the socket address.
        /// </summary>
        /// <returns>A read-only span of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<byte> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _buffer[0]), 28);

        /// <summary>
        ///     Determines the address family from a raw socket address byte span.
        /// </summary>
        /// <returns>The detected <see cref="AddressFamily" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly AddressFamily GetAddressFamily()
        {
            ushort result = _ss_family;

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
                    _ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
                    break;

                case AddressFamily.InterNetworkV6:
                    _ss_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
                    break;

                default:
                    _ss_family = (ushort)value;
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
        ///     Returns the string representation of this instance in <see cref="IPEndPoint" /> format.
        /// </summary>
        public readonly override string ToString()
        {
            Span<char> chars = stackalloc char[NativeSocketAddressPal.FORMAT_MAX_CHARS];
            SocketError error = NativeSocketAddressPal.FormatAsIpEndPoint(ref chars, this);
            return error == SocketError.Success ? chars.ToString() : error.ToString();
        }

        /// <summary>
        ///     Deserializes a <see cref="NativeSocketAddress" /> from the specified byte span.
        /// </summary>
        /// <param name="bytes">
        ///     An Ipv4 socket address requires at least 8 bytes; an Ipv6 socket address requires 28 bytes.
        /// </param>
        /// <param name="result">When this method returns, contains the deserialized socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Deserialize(ReadOnlySpan<byte> bytes, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.Deserialize(ref result, bytes);
        }

        /// <summary>
        ///     Tries to parse an <see cref="IPEndPoint" /> string into a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="ipEndPointText">The <see cref="IPEndPoint" /> string to parse.</param>
        /// <param name="result">When this method returns, contains the parsed socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <para>Only complete, standard <see cref="IPEndPoint" /> string representations are accepted.</para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 Supports Ipv6 scope id parsing:
        ///                 the text after '%' may be either a numeric value or an interface name.
        ///             </para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 Unlike the standard library, which silently ignores a malformed scope id and returns success
        ///                 with the scope id set to 0,
        ///                 this implementation returns <see cref="SocketError.InvalidArgument" />
        ///                 when the scope id text is neither a valid number nor a resolvable interface name.
        ///             </para>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static SocketError TryParse(ReadOnlySpan<char> ipEndPointText, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.TryParseIpEndPoint(ref result, ipEndPointText);
        }

        /// <summary>
        ///     Tries to parse an <see cref="IPAddress" /> string into a <see cref="NativeSocketAddress" />,
        ///     using the specified port.
        /// </summary>
        /// <param name="ipAddressText">The <see cref="IPAddress" /> string to parse.</param>
        /// <param name="port">The port number.</param>
        /// <param name="result">When this method returns, contains the parsed socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <para>Only complete, standard <see cref="IPAddress" /> string representations are accepted.</para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 Supports Ipv6 scope id parsing:
        ///                 the text after '%' may be either a numeric value or an interface name.
        ///             </para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 Unlike the standard library, which silently ignores a malformed scope id and returns success
        ///                 with the scope id set to 0,
        ///                 this implementation returns <see cref="SocketError.InvalidArgument" />
        ///                 when the scope id text is neither a valid number nor a resolvable interface name.
        ///             </para>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static SocketError TryParseIpAddress(ReadOnlySpan<char> ipAddressText, ushort port, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.TryParseIpAddress(ref result, ipAddressText, port);
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPEndPoint" />.
        /// </summary>
        /// <param name="ipEndPoint">The <see cref="IPEndPoint" /> to copy from.</param>
        /// <param name="result">When this method returns, contains the populated <see cref="NativeSocketAddress" />.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="ipEndPoint" /> is null.</exception>
        public static SocketError FromIpEndPoint(IPEndPoint ipEndPoint, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.SetFromIpEndPoint(ref result, ipEndPoint);
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPAddress" /> and port.
        /// </summary>
        /// <param name="ipAddress">The <see cref="IPAddress" /> to copy from.</param>
        /// <param name="port">The port number.</param>
        /// <param name="result">When this method returns, contains the populated <see cref="NativeSocketAddress" />.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="ipAddress" /> is null.</exception>
        public static SocketError FromIpAddress(IPAddress ipAddress, ushort port, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.SetFromIpAddress(ref result, ipAddress, port);
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="SocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The <see cref="SocketAddress" /> to copy from.</param>
        /// <param name="result">When this method returns, contains the populated <see cref="NativeSocketAddress" />.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="socketAddress" /> is null.</exception>
        public static SocketError FromSocketAddress(SocketAddress socketAddress, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.SetFromSocketAddress(ref result, socketAddress);
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified Ipv4 ip and port.
        /// </summary>
        /// <param name="ip">The ip as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <param name="result">When this method returns, contains the populated <see cref="NativeSocketAddress" />.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError FromIpIpv4(ReadOnlySpan<char> ip, ushort port, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.SetFromIpIpv4(ref result, ip, port);
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified Ipv6 ip, port, and scope id.
        /// </summary>
        /// <param name="ip">The ip as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The scope id for the Ipv6 ip.</param>
        /// <param name="result">When this method returns, contains the populated <see cref="NativeSocketAddress" />.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError FromIpIpv6(ReadOnlySpan<char> ip, ushort port, uint scopeId, out NativeSocketAddress result)
        {
            Unsafe.SkipInit(out result);
            return NativeSocketAddressPal.SetFromIpIpv6(ref result, ip, port, scopeId);
        }
    }
}