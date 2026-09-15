using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope.

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides extension methods for <see cref="NativeSocketAddress" />.
    /// </summary>
    public static unsafe class NativeSocketAddressExtensions
    {
        /// <summary>
        ///     Converts a <see cref="NativeSocketAddress" /> into an <see cref="IPEndPoint" />.
        /// </summary>
        /// <param name="socketAddress">The socket address to convert.</param>
        /// <param name="result">
        ///     When this method returns, contains the converted <see cref="IPEndPoint" />,
        ///     or null if the address family is not supported.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError ToIpEndPoint(this NativeSocketAddress socketAddress, out IPEndPoint? result)
        {
            SocketError error = socketAddress.ToIpAddress(out IPAddress? address);
            if (error != SocketError.Success)
            {
                result = default;
                return error;
            }

            result = new IPEndPoint(address!, socketAddress.Port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Converts a <see cref="NativeSocketAddress" /> into an <see cref="IPAddress" />.
        /// </summary>
        /// <param name="socketAddress">The socket address to convert.</param>
        /// <param name="result">
        ///     When this method returns, contains the converted <see cref="IPAddress" />,
        ///     or null if the address family is not supported.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError ToIpAddress(this NativeSocketAddress socketAddress, out IPAddress? result)
        {
            if (!socketAddress.IsIpv4 && !socketAddress.IsIpv6)
            {
                result = default;
                return SocketError.AddressFamilyNotSupported;
            }

            result = socketAddress.IsIpv6 ? new IPAddress(socketAddress.Ip, socketAddress.ScopeId) : new IPAddress(socketAddress.Ip);
            return SocketError.Success;
        }

        /// <summary>
        ///     Converts a <see cref="NativeSocketAddress" /> into a <see cref="SocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The socket address to convert.</param>
        /// <param name="result">
        ///     When this method returns, contains the converted <see cref="SocketAddress" />,
        ///     or null if the address family is not supported.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError ToSocketAddress(this NativeSocketAddress socketAddress, out SocketAddress? result)
        {
            if (!socketAddress.IsIpv4 && !socketAddress.IsIpv6)
            {
                result = default;
                return SocketError.AddressFamilyNotSupported;
            }

            result = new SocketAddress(socketAddress.Family);
            result.CopyFromWithoutFamily(socketAddress.AsReadOnlySpan(), socketAddress.IsIpv4 ? 8 : 28);
            return SocketError.Success;
        }

        /// <summary>
        ///     Retrieves the ip address from a <see cref="NativeSocketAddress" /> as text.
        /// </summary>
        /// <param name="socketAddress">The <see cref="NativeSocketAddress" /> to read the ip address from.</param>
        /// <param name="ip">The character span to receive the ip address; resized to the actual length on success.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIp(this NativeSocketAddress socketAddress, ref Span<char> ip)
        {
            if (!socketAddress.IsIpv4 && !socketAddress.IsIpv6)
                return SocketError.AddressFamilyNotSupported;

            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError result = socketAddress.IsIpv4 ? SocketPal.GetIpIpv4((sockaddr_in4*)&socketAddress, bytes) : SocketPal.GetIpIpv6((sockaddr_in6*)&socketAddress, bytes);
            if (result == SocketError.Success)
                return GetAsciiCharsFromBytes(ref ip, bytes);

            return result;
        }

        /// <summary>
        ///     Gets the host name (reverse DNS) from a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The <see cref="NativeSocketAddress" /> to resolve the host name for.</param>
        /// <param name="hostName">The character span to receive the host name; resized to the actual length on success.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName(this NativeSocketAddress socketAddress, ref Span<char> hostName)
        {
            if (!socketAddress.IsIpv4 && !socketAddress.IsIpv6)
                return SocketError.AddressFamilyNotSupported;

            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError result = socketAddress.IsIpv4 ? SocketPal.GetHostNameIpv4((sockaddr_in4*)&socketAddress, bytes) : SocketPal.GetHostNameIpv6((sockaddr_in6*)&socketAddress, bytes);
            if (result == SocketError.Success)
                return GetAsciiCharsFromBytes(ref hostName, bytes);

            return result;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPEndPoint" />.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="source">The <see cref="IPEndPoint" /> containing the ip address and port.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="source" /> is null.</exception>
        internal static SocketError FromIpEndPoint(ref this NativeSocketAddress socketAddress, IPEndPoint source) => socketAddress.FromIpAddress(source.Address, (ushort)source.Port);

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPAddress" /> and port.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="source">The <see cref="IPAddress" /> to copy from.</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="source" /> is null.</exception>
        internal static SocketError FromIpAddress(ref this NativeSocketAddress socketAddress, IPAddress source, ushort port)
        {
            if (source.AddressFamily == AddressFamily.InterNetwork || source.AddressFamily == AddressFamily.InterNetworkV6)
            {
                socketAddress = new NativeSocketAddress();
                socketAddress.Family = source.AddressFamily;
                socketAddress.Port = port;
                source.TryWriteBytes(socketAddress.Ip, out _);

                if (source.AddressFamily == AddressFamily.InterNetworkV6)
                    socketAddress.ScopeId = (uint)source.ScopeId;

                return SocketError.Success;
            }

            return SocketError.AddressFamilyNotSupported;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="SocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="source">The source <see cref="SocketAddress" /> to copy from.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="source" /> is null.</exception>
        internal static SocketError FromSocketAddress(ref this NativeSocketAddress socketAddress, SocketAddress source)
        {
            if (source.Family == AddressFamily.InterNetwork || source.Family == AddressFamily.InterNetworkV6)
            {
                if ((source.Family == AddressFamily.InterNetwork && source.Size >= 16) || (source.Family == AddressFamily.InterNetworkV6 && source.Size >= 28))
                {
                    socketAddress.Family = source.Family;
                    source.CopyToWithoutFamily(socketAddress.AsSpan(), source.Family == AddressFamily.InterNetwork ? 8 : 28);

                    if (source.Family == AddressFamily.InterNetwork)
                        socketAddress.AsSpan().Slice(8).Clear();

                    return SocketError.Success;
                }

                return SocketError.NoBufferSpaceAvailable;
            }

            return SocketError.AddressFamilyNotSupported;
        }

        /// <summary>
        ///     Sets the specified Ipv4 address and port on a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SocketError FromIpIpv4(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> ip, ushort port)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, ip);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            error = SocketPal.SetIpIpv4(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            socketAddress.SetFromIpv4(ref __socketAddress_native, port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Sets the specified Ipv6 address, port, and scope id on a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The scope id for the Ipv6 address.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SocketError FromIpIpv6(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> ip, ushort port, uint scopeId)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, ip);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            error = SocketPal.SetIpIpv6(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            socketAddress.SetFromIpv6(ref __socketAddress_native, port, scopeId);
            return SocketError.Success;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> by resolving the specified host name to an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="hostName">The host name to resolve (e.g., "localhost", "example.com").</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SocketError FromHostNameIpv4(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> hostName, ushort port)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, hostName);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            error = SocketPal.SetHostNameIpv4(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            socketAddress.SetFromIpv4(ref __socketAddress_native, port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> by resolving the specified host name to an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="hostName">The host name to resolve (e.g., "localhost", "example.com").</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The Ipv6 scope identifier (used for link-local or site-local addresses).</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SocketError FromHostNameIpv6(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> hostName, ushort port, uint scopeId)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError error = GetAsciiBytesFromChars(ref bytes, hostName);
            if (error != SocketError.Success)
                return error;

            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            error = SocketPal.SetHostNameIpv6(&__socketAddress_native, bytes);
            if (error != SocketError.Success)
                return error;

            socketAddress.SetFromIpv6(ref __socketAddress_native, port, scopeId);
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
        private static SocketError GetAsciiCharsFromBytes(ref Span<char> destination, ReadOnlySpan<byte> source)
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
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="__socketAddress_native">The source Ipv4 address structure.</param>
        /// <param name="port">The port number in host byte order.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetFromIpv4(ref this NativeSocketAddress socketAddress, ref sockaddr_in4 __socketAddress_native, ushort port)
        {
            __socketAddress_native.sin4_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
            __socketAddress_native.sin4_port = WinSock2.HOST_TO_NET_16(port);
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), ref Unsafe.As<sockaddr_in4, byte>(ref __socketAddress_native), 8);
            SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 8), 0, 20);
        }

        /// <summary>
        ///     Copies an Ipv6 socket address structure into a <see cref="NativeSocketAddress" />,
        ///     converting the port to network byte order and setting the flow info and scope id.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="__socketAddress_native">The source Ipv6 address structure.</param>
        /// <param name="port">The port number in host byte order.</param>
        /// <param name="scopeId">The Ipv6 scope identifier.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetFromIpv6(ref this NativeSocketAddress socketAddress, ref sockaddr_in6 __socketAddress_native, ushort port, uint scopeId)
        {
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = WinSock2.HOST_TO_NET_16(port);
            __socketAddress_native.sin6_flowinfo = 0;
            __socketAddress_native.sin6_scope_id = scopeId;
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), ref Unsafe.As<sockaddr_in6, byte>(ref __socketAddress_native), 28);
        }
    }
}