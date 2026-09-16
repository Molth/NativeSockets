using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides methods for <see cref="NativeSocketAddress" />.
    /// </summary>
    internal static unsafe class NativeSocketAddressPal
    {
        /// <summary>
        ///     Size of the stack-allocated character buffer used when formatting an address as
        ///     an endpoint string. The longest possible output (an expanded Ipv6 address with a
        ///     scope id and port) is well under this size.
        /// </summary>
        public const int FORMAT_MAX_CHARS = 256;

        /// <summary>
        ///     Serializes the address into the specified byte span.
        /// </summary>
        /// <param name="source">The socket address to serialize.</param>
        /// <param name="destination">
        ///     The byte span to receive the serialized address. On return, it is sliced
        ///     to the number of bytes actually written.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     An Ipv4 address is serialized as 8 bytes (family, port, address),
        ///     an Ipv6 address as 28 bytes (the full socket address structure).
        ///     The family field is stored as the managed <see cref="AddressFamily" /> value
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
        ///     Deserializes an address from the specified byte span.
        /// </summary>
        /// <param name="source">
        ///     An Ipv4 address requires at least 8 bytes; an Ipv6 address requires 28 bytes.
        /// </param>
        /// <param name="destination">When this method returns, contains the deserialized address.</param>
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
        ///     Formats the socket address into the specified character buffer.
        ///     The format is: <c>Family:Size:{byte1,byte2,...}</c>,
        ///     where each byte is expressed as a decimal number.
        /// </summary>
        /// <param name="destination">
        ///     A caller-provided character buffer that receives the formatted output.
        ///     The buffer is assumed to be large enough.
        ///     On return, this reference is reassigned to the
        ///     slice of the buffer that contains the formatted characters, i.e. it is trimmed
        ///     to the exact written length.
        /// </param>
        /// <param name="source">A reference to the socket address to format.</param>
        public static void Format(ref Span<char> destination, in NativeSocketAddress source)
        {
            ReadOnlySpan<char> family = source.Family.ToString().AsSpan();

            family.CopyTo(destination);
            int length = family.Length;

            destination[length++] = ':';

            source.Size.TryFormat(destination.Slice(length), out int charsWritten);

            length += charsWritten;

            destination[length++] = ':';
            destination[length++] = '{';

            ReadOnlySpan<byte> buffer = source.AsReadOnlySpan().Slice(0, source.Size);
            for (int i = 2; i < buffer.Length; ++i)
            {
                if (i > 2)
                    destination[length++] = ',';

                buffer[i].TryFormat(destination.Slice(length), out charsWritten);

                length += charsWritten;
            }

            destination[length++] = '}';

            destination = destination.Slice(0, length);
        }

        /// <summary>
        ///     Formats the socket address into the specified character buffer,
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
            int position = 0;

            if (source.IsIpv6)
                destination[position++] = '[';

            Span<char> ip = destination.Slice(position);
            SocketError error = source.GetIp(ref ip);
            if (error != SocketError.Success)
                return error;

            position += ip.Length;

            int charsWritten;

            if (source.IsIpv6)
            {
                if (source.ScopeId != 0)
                {
                    destination[position++] = '%';
                    source.ScopeId.TryFormat(destination.Slice(position), out charsWritten);
                    position += charsWritten;
                }

                destination[position++] = ']';
            }

            destination[position++] = ':';
            source.Port.TryFormat(destination.Slice(position), out charsWritten);
            position += charsWritten;

            destination = destination.Slice(0, position);
            return SocketError.Success;
        }

        /// <summary>
        ///     Extracts the ASCII string from a null-terminated byte
        ///     span and copies it into a character span.
        /// </summary>
        /// <param name="destination">
        ///     The character span to receive the decoded string.
        ///     On success, it is resized to the actual character count.
        /// </param>
        /// <param name="source">The null-terminated ASCII byte span (typically from native APIs).</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        public static SocketError GetAsciiCharsFromBytes(ref Span<char> destination, ReadOnlySpan<byte> source)
        {
            int index = source.IndexOf((byte)'\0');
            if (index <= 0)
                return SocketError.Fault;

            source = source.Slice(0, index);
            int charCount = Encoding.ASCII.GetCharCount(source);
            if (destination.Length < charCount)
                return SocketError.NoBufferSpaceAvailable;

            destination = destination.Slice(0, charCount);
            Encoding.ASCII.GetChars(source, destination);
            return SocketError.Success;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPEndPoint" />.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ipEndPoint">The <see cref="IPEndPoint" /> containing the ip address and port.</param>
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
        ///     Sets the specified Ipv4 address and port on a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetFromIpIpv4(ref NativeSocketAddress destination, ReadOnlySpan<char> ip, ushort port)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, ip);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            error = SocketPal.SetIpIpv4(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            SetFromIpv4(ref destination, ref __socketAddress_native, port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Sets the specified Ipv6 address, port, and scope id on a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The scope id for the Ipv6 address.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetFromIpIpv6(ref NativeSocketAddress destination, ReadOnlySpan<char> ip, ushort port, uint scopeId)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, ip);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            error = SocketPal.SetIpIpv6(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            SetFromIpv6(ref destination, ref __socketAddress_native, port, scopeId);
            return SocketError.Success;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> by resolving the specified host name to an Ipv4 address.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="hostName">The host name to resolve (e.g., "localhost", "example.com").</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetFromHostNameIpv4(ref NativeSocketAddress destination, ReadOnlySpan<char> hostName, ushort port)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, hostName);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            error = SocketPal.SetHostNameIpv4(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            SetFromIpv4(ref destination, ref __socketAddress_native, port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> by resolving the specified host name to an Ipv6 address.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="hostName">The host name to resolve (e.g., "localhost", "example.com").</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The Ipv6 scope identifier (used for link-local or site-local addresses).</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetFromHostNameIpv6(ref NativeSocketAddress destination, ReadOnlySpan<char> hostName, ushort port, uint scopeId)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, hostName);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            error = SocketPal.SetHostNameIpv6(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            SetFromIpv6(ref destination, ref __socketAddress_native, port, scopeId);
            return SocketError.Success;
        }

        /// <summary>
        ///     Converts the specified text to null-terminated ASCII bytes and writes them into the provided span,
        ///     suitable for use with native APIs that expect null-terminated strings (e.g., <c>inet_pton</c>, <c>getaddrinfo</c>).
        /// </summary>
        /// <param name="destination">
        ///     The span used to receive the null-terminated ASCII bytes.
        ///     On success, it is resized to the actual written length (ASCII byte count + 1).
        /// </param>
        /// <param name="source">The text to convert to ASCII.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        private static SocketError GetAsciiBytesFromChars(ref Span<byte> destination, ReadOnlySpan<char> source)
        {
            int byteCount = Encoding.ASCII.GetByteCount(source);
            if ((uint)(byteCount + 1) > (uint)destination.Length)
                return SocketError.InvalidArgument;

            destination = destination.Slice(0, byteCount + 1);
            Encoding.ASCII.GetBytes(source, destination);
            destination[byteCount] = (byte)'\0';

            return SocketError.Success;
        }

        /// <summary>
        ///     Copies an Ipv4 socket address structure into a <see cref="NativeSocketAddress" />,
        ///     converting the port to network byte order and zeroing the padding.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="__socketAddress_native">The source Ipv4 address structure.</param>
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
        ///     Copies an Ipv6 socket address structure into a <see cref="NativeSocketAddress" />,
        ///     converting the port to network byte order and setting the flow info and scope id.
        /// </summary>
        /// <param name="destination">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="__socketAddress_native">The source Ipv6 address structure.</param>
        /// <param name="port">The port number in host byte order.</param>
        /// <param name="scopeId">The Ipv6 scope identifier.</param>
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