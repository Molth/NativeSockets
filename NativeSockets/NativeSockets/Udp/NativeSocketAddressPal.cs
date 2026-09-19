using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides methods for <see cref="NativeSocketAddress" />.
    /// </summary>
    internal static class NativeSocketAddressPal
    {
        /// <summary>
        ///     Size of the stack-allocated character buffer used when formatting a socket address as
        ///     a socket address string. The longest possible output (an expanded Ipv6 ip with a
        ///     scope id and port) is well under this size.
        /// </summary>
        public const int FORMAT_MAX_CHARS = 256;

        /// <summary>
        ///     Tries to parse an <see cref="IPEndPoint" /> string into a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="destination">When this method returns, contains the parsed socket address.</param>
        /// <param name="ipEndPointText">The <see cref="IPEndPoint" /> string to parse.</param>
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
        public static SocketError TryParseIpEndPoint(ref NativeSocketAddress destination, ReadOnlySpan<char> ipEndPointText)
        {
            if (ipEndPointText.Length < 3)
                return SocketError.InvalidArgument;

            if (ipEndPointText[0] == '[')
            {
                int closeBracket = ipEndPointText.IndexOf(']');
                if (closeBracket <= 2 || closeBracket >= ipEndPointText.Length - 2 || ipEndPointText[closeBracket + 1] != ':' || !ushort.TryParse(ipEndPointText.Slice(closeBracket + 2), NumberStyles.None, CultureInfo.InvariantCulture, out ushort port))
                    return SocketError.InvalidArgument;

                return TryParseIpAddress(ref destination, ipEndPointText.Slice(0, closeBracket + 1), port);
            }
            else
            {
                int lastColon = ipEndPointText.LastIndexOf(':');
                if (lastColon <= 0 || lastColon >= ipEndPointText.Length - 1 || !ushort.TryParse(ipEndPointText.Slice(lastColon + 1), NumberStyles.None, CultureInfo.InvariantCulture, out ushort port))
                    return SocketError.InvalidArgument;

                return TryParseIpAddress(ref destination, ipEndPointText.Slice(0, lastColon), port);
            }
        }

        /// <summary>
        ///     Tries to parse an <see cref="IPAddress" /> string into a <see cref="NativeSocketAddress" />,
        ///     using the specified port.
        /// </summary>
        /// <param name="destination">When this method returns, contains the parsed socket address.</param>
        /// <param name="ipAddressText">The <see cref="IPAddress" /> string to parse.</param>
        /// <param name="port">The port number.</param>
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
        public static SocketError TryParseIpAddress(ref NativeSocketAddress destination, ReadOnlySpan<char> ipAddressText, ushort port)
        {
            if (ipAddressText.IndexOf(':') >= 0)
            {
                if (ipAddressText.Length >= 2 && ipAddressText[0] == '[' && ipAddressText[^1] == ']')
                    ipAddressText = ipAddressText.Slice(1, ipAddressText.Length - 2);

                uint scopeId = 0;
                int percent = ipAddressText.IndexOf('%');
                if (percent >= 0)
                {
                    ReadOnlySpan<char> scopeIdText = ipAddressText.Slice(percent + 1);
                    if (!InterfaceInfoPal.TryParseScopeId(scopeIdText, out scopeId))
                        return SocketError.InvalidArgument;

                    ipAddressText = ipAddressText.Slice(0, percent);
                }

                return SetFromIpIpv6(ref destination, ipAddressText, port, scopeId);
            }

            return SetFromIpIpv4(ref destination, ipAddressText, port);
        }

        /// <summary>
        ///     Serializes a <see cref="NativeSocketAddress" /> into the specified byte span.
        /// </summary>
        /// <param name="source">The socket address to serialize.</param>
        /// <param name="destination">
        ///     The byte span to receive the serialized socket address. On return, it is sliced
        ///     to the number of bytes actually written.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     An Ipv4 socket address is serialized as 8 bytes (family, port, ip),
        ///     an Ipv6 socket address as 28 bytes (the full socket address).
        ///     The family field is stored as the managed <see cref="AddressFamily" /> value,
        ///     so the serialized bytes are independent of the native platform constants.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Serialize(ref Span<byte> destination, in NativeSocketAddress source)
        {
            if (!source.IsIpv4 && !source.IsIpv6)
                return SocketError.AddressFamilyNotSupported;

            ReadOnlySpan<byte> buffer = source.AsReadOnlySpan().Slice(0, source.IsIpv4 ? 8 : 28);
            if (!buffer.TryCopyTo(destination))
                return SocketError.NoBufferSpaceAvailable;

            if (source.IsIpv6)
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref MemoryMarshal.GetReference(destination), 24), WinSock2.HOST_TO_NET_32(source.ScopeId));

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), WinSock2.HOST_TO_NET_16((ushort)source.Family));

            destination = destination.Slice(0, buffer.Length);
            return SocketError.Success;
        }

        /// <summary>
        ///     Deserializes a <see cref="NativeSocketAddress" /> from the specified byte span.
        /// </summary>
        /// <param name="source">
        ///     An Ipv4 socket address requires at least 8 bytes; an Ipv6 socket address requires 28 bytes.
        /// </param>
        /// <param name="destination">When this method returns, contains the deserialized socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Deserialize(ref NativeSocketAddress destination, ReadOnlySpan<byte> source)
        {
            if (source.Length < 8)
                return SocketError.NoBufferSpaceAvailable;

            AddressFamily family = (AddressFamily)WinSock2.NET_TO_HOST_16(Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(source)));

            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
                return SocketError.AddressFamilyNotSupported;

            if (family == AddressFamily.InterNetworkV6 && source.Length < 28)
                return SocketError.NoBufferSpaceAvailable;

            if (family == AddressFamily.InterNetwork)
            {
                SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref destination), ref MemoryMarshal.GetReference(source), 8);
                SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref destination), 8), 0, 20);
            }
            else
            {
                SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref destination), ref MemoryMarshal.GetReference(source), 28);
                destination.ScopeId = WinSock2.NET_TO_HOST_32(destination.ScopeId);
            }

            destination.Family = family;
            return SocketError.Success;
        }

        /// <summary>
        ///     Formats the <see cref="NativeSocketAddress" /> into the specified character buffer,
        ///     as a debug-friendly representation of the raw socket address bytes.
        /// </summary>
        /// <param name="destination">
        ///     A caller-provided character buffer that receives the formatted output.
        ///     The buffer is assumed to be large enough.
        ///     On return, this reference is reassigned to the
        ///     slice of the buffer that contains the formatted characters, i.e. it is trimmed
        ///     to the exact written length.
        /// </param>
        /// <param name="source">A reference to the socket address to format.</param>
        public static void FormatDebugView(ref Span<char> destination, in NativeSocketAddress source)
        {
            ReadOnlySpan<char> family = source.Family.ToString().AsSpan();

            family.CopyTo(destination);
            int length = family.Length;

            destination[length++] = ':';

            source.Size.TryFormat(destination.Slice(length), out int charsWritten, default, CultureInfo.InvariantCulture);

            length += charsWritten;

            destination[length++] = ':';
            destination[length++] = '{';

            ReadOnlySpan<byte> buffer = source.AsReadOnlySpan().Slice(0, source.Size);
            for (int i = 2; i < buffer.Length; ++i)
            {
                if (i > 2)
                    destination[length++] = ',';

                buffer[i].TryFormat(destination.Slice(length), out charsWritten, default, CultureInfo.InvariantCulture);

                length += charsWritten;
            }

            destination[length++] = '}';

            destination = destination.Slice(0, length);
        }

        /// <summary>
        ///     Formats the <see cref="NativeSocketAddress" /> into the specified character buffer,
        ///     as an <see cref="IPEndPoint" /> string.
        /// </summary>
        /// <param name="destination">
        ///     The buffer is assumed to be large enough.
        ///     On return, this reference is reassigned to the
        ///     slice of the buffer that contains the formatted characters, i.e. it is trimmed
        ///     to the exact written length.
        /// </param>
        /// <param name="source">A reference to the socket address to format.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError FormatAsIpEndPoint(ref Span<char> destination, in NativeSocketAddress source)
        {
            int length = 0;

            if (source.IsIpv6)
                destination[length++] = '[';

            Span<char> ip = destination.Slice(length);
            SocketError error = source.GetIp(ref ip);
            if (error != SocketError.Success)
                return error;

            length += ip.Length;

            int charsWritten;

            if (source.IsIpv6)
            {
                if (source.ScopeId != 0)
                {
                    destination[length++] = '%';
                    source.ScopeId.TryFormat(destination.Slice(length), out charsWritten, default, CultureInfo.InvariantCulture);
                    length += charsWritten;
                }

                destination[length++] = ']';
            }

            destination[length++] = ':';
            source.Port.TryFormat(destination.Slice(length), out charsWritten, default, CultureInfo.InvariantCulture);
            length += charsWritten;

            destination = destination.Slice(0, length);
            return SocketError.Success;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPEndPoint" />.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ipEndPoint">The <see cref="IPEndPoint" /> containing the ip and port.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="ipEndPoint" /> is null.</exception>
        public static SocketError SetFromIpEndPoint(ref NativeSocketAddress destination, IPEndPoint ipEndPoint) => SetFromIpAddress(ref destination, ipEndPoint.Address, (ushort)ipEndPoint.Port);

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPAddress" /> and port.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ipAddress">The <see cref="IPAddress" /> to copy from.</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="ipAddress" /> is null.</exception>
        public static SocketError SetFromIpAddress(ref NativeSocketAddress destination, IPAddress ipAddress, ushort port)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork || ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                destination = new NativeSocketAddress();
                destination.Family = ipAddress.AddressFamily;
                destination.Port = port;
                ipAddress.TryWriteBytes(destination.Ip, out _);

                if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    destination.ScopeId = (uint)ipAddress.ScopeId;

                return SocketError.Success;
            }

            return SocketError.AddressFamilyNotSupported;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="SocketAddress" />.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="socketAddress">The source <see cref="SocketAddress" /> to copy from.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="socketAddress" /> is null.</exception>
        public static SocketError SetFromSocketAddress(ref NativeSocketAddress destination, SocketAddress socketAddress)
        {
            if (socketAddress.Family == AddressFamily.InterNetwork || socketAddress.Family == AddressFamily.InterNetworkV6)
            {
                if ((socketAddress.Family == AddressFamily.InterNetwork && socketAddress.Size >= 16) || (socketAddress.Family == AddressFamily.InterNetworkV6 && socketAddress.Size >= 28))
                {
                    destination.Family = socketAddress.Family;

                    if (socketAddress.Family == AddressFamily.InterNetwork)
                    {
                        socketAddress.CopyToWithoutFamily(destination.AsSpan(), 8);
                        SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref destination), 8), 0, 20);
                    }
                    else
                    {
                        socketAddress.CopyToWithoutFamily(destination.AsSpan(), 28);
                    }

                    return SocketError.Success;
                }

                return SocketError.NoBufferSpaceAvailable;
            }

            return SocketError.AddressFamilyNotSupported;
        }

        /// <summary>
        ///     Sets the specified Ipv4 ip and port on a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetFromIpIpv4(ref NativeSocketAddress destination, ReadOnlySpan<char> ip, ushort port)
        {
            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            if (!IpAddressParser.TryParseIpv4(ip, MemoryMarshalHelpers.AsBytes(ref __socketAddress_native).Slice(4, 4)))
                return SocketError.InvalidArgument;

            SetFromIpv4(ref destination, ref __socketAddress_native, port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Sets the specified Ipv6 ip, port, and scope id on a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The scope id for the Ipv6 ip.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetFromIpIpv6(ref NativeSocketAddress destination, ReadOnlySpan<char> ip, ushort port, uint scopeId)
        {
            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            if (!IpAddressParser.TryParseIpv6(ip, MemoryMarshalHelpers.AsBytes(ref __socketAddress_native).Slice(8, 16)))
                return SocketError.InvalidArgument;

            SetFromIpv6(ref destination, ref __socketAddress_native, port, scopeId);
            return SocketError.Success;
        }

        /// <summary>
        ///     Copies an Ipv4 socket address into a <see cref="NativeSocketAddress" />,
        ///     converting the port to network byte order and zeroing the padding.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="__socketAddress_native">The source Ipv4 socket address.</param>
        /// <param name="port">The port number in host byte order.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetFromIpv4(ref NativeSocketAddress destination, ref sockaddr_in4 __socketAddress_native, ushort port)
        {
            __socketAddress_native.sin4_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
            __socketAddress_native.sin4_port = WinSock2.HOST_TO_NET_16(port);
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref destination), ref Unsafe.As<sockaddr_in4, byte>(ref __socketAddress_native), 8);
            SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref destination), 8), 0, 20);
        }

        /// <summary>
        ///     Copies an Ipv6 socket address into a <see cref="NativeSocketAddress" />,
        ///     converting the port to network byte order and setting the flow info and scope id.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="__socketAddress_native">The source Ipv6 socket address.</param>
        /// <param name="port">The port number in host byte order.</param>
        /// <param name="scopeId">The Ipv6 scope id.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetFromIpv6(ref NativeSocketAddress destination, ref sockaddr_in6 __socketAddress_native, ushort port, uint scopeId)
        {
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = WinSock2.HOST_TO_NET_16(port);
            __socketAddress_native.sin6_flowinfo = 0;
            __socketAddress_native.sin6_scope_id = scopeId;
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref destination), ref Unsafe.As<sockaddr_in6, byte>(ref __socketAddress_native), 28);
        }
    }
}