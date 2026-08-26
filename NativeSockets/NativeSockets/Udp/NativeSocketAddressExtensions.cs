using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides extension methods for <see cref="NativeSocketAddress" />.
    /// </summary>
    public static unsafe class NativeSocketAddressExtensions
    {
        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPEndPoint" />.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="source">The <see cref="IPEndPoint" /> containing the ip address and port.</param>
        /// <returns>
        ///     <see cref="SocketError.Success" /> if successful;
        ///     <see cref="SocketError.AddressFamilyNotSupported" /> if the address family is not Ipv4 or Ipv6.
        /// </returns>
        public static SocketError FromIpEndPoint(ref this NativeSocketAddress socketAddress, IPEndPoint source) => socketAddress.FromIpAddress(source.Address, (ushort)source.Port);

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPAddress" />, port,
        ///     and scope id (ignored for Ipv4).
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="source">The <see cref="IPAddress" /> to set.</param>
        /// <param name="port">The port number.</param>
        /// <returns>
        ///     <see cref="SocketError.Success" /> if successful;
        ///     <see cref="SocketError.AddressFamilyNotSupported" /> if the address family is not Ipv4 or Ipv6.
        /// </returns>
        public static SocketError FromIpAddress(ref this NativeSocketAddress socketAddress, IPAddress source, ushort port)
        {
            if (source.AddressFamily == AddressFamily.InterNetwork || source.AddressFamily == AddressFamily.InterNetworkV6)
            {
                socketAddress = new NativeSocketAddress();
                socketAddress.Family = source.AddressFamily;
                socketAddress.Port = port;
                source.TryWriteBytes(socketAddress.Address, out _);

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
        /// <returns>
        ///     <see cref="SocketError.Success" /> if the address is valid and copied successfully;
        ///     <see cref="SocketError.AddressFamilyNotSupported" /> if the address family is not Ipv4 or Ipv6,
        ///     or the buffer size is insufficient.
        /// </returns>
        public static SocketError FromSocketAddress(ref this NativeSocketAddress socketAddress, SocketAddress source)
        {
            if ((source.Family == AddressFamily.InterNetwork && source.Size >= 16) || (source.Family == AddressFamily.InterNetworkV6 && source.Size >= 28))
            {
                socketAddress.Family = source.Family;
                source.CopyTo(socketAddress.Buffer, source.Family == AddressFamily.InterNetwork ? 8 : 28);

                if (source.Family == AddressFamily.InterNetwork)
                    socketAddress.AsSpan().Slice(8).Clear();

                return SocketError.Success;
            }

            return SocketError.AddressFamilyNotSupported;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IPEndPoint" /> class with the specified address and port number.
        /// </summary>
        /// <param name="socketAddress">The socket address to convert.</param>
        /// <exception cref="ArgumentException">Address contains a bad ip address.</exception>
        /// <returns>A new instance of the <see cref="IPEndPoint" /> class.</returns>
        public static IPEndPoint ToIpEndPoint(this NativeSocketAddress socketAddress) => new(socketAddress.ToIpAddress(), socketAddress.Port);

        /// <summary>
        ///     Initializes a new instance of the <see cref="IPAddress" /> class with the specified address.
        /// </summary>
        /// <param name="socketAddress">The socket address to convert.</param>
        /// <exception cref="ArgumentException">Address contains a bad ip address.</exception>
        /// <returns>A new instance of the <see cref="IPAddress" /> class.</returns>
        public static IPAddress ToIpAddress(this NativeSocketAddress socketAddress) => socketAddress.IsIpv6 ? new IPAddress(socketAddress.Address, socketAddress.ScopeId) : new IPAddress(socketAddress.Address);

        /// <summary>
        ///     Initializes a new instance of the <see cref="SocketAddress" /> class with the specified address.
        /// </summary>
        /// <param name="socketAddress">The socket address to convert.</param>
        /// <exception cref="NotSupportedException">
        ///     Family != <see cref="AddressFamily.InterNetwork" />
        ///     or <see cref="AddressFamily.InterNetworkV6" />.
        /// </exception>
        /// <returns>A new instance of the <see cref="SocketAddress" /> class.</returns>
        public static SocketAddress ToSocketAddress(this NativeSocketAddress socketAddress)
        {
            if (socketAddress.Family != AddressFamily.InterNetwork && socketAddress.Family != AddressFamily.InterNetworkV6)
            {
                ThrowHelpers.ThrowNotSupportedException();
                return default;
            }

            ReadOnlySpan<byte> buffer = socketAddress.Buffer;
            SocketAddress result = new SocketAddress(socketAddress.Family);
            result.CopyFrom(buffer, socketAddress.Family == AddressFamily.InterNetwork ? 8 : 28);
            return result;
        }

        /// <summary>
        ///     Converts an Ipv4 address and port into a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv4(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> ip, ushort port)
        {
            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            SocketError error = SocketPal.SetIpIpv4(&__socketAddress_native, ip);
            if (error != SocketError.Success)
                return error;

            socketAddress.CopyFromIpv4(ref __socketAddress_native, port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Converts an Ipv6 address, port, and scope id into a <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="ip">The ip address as a span of characters.</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The scope id for the Ipv6 address.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIpIpv6(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> ip, ushort port, uint scopeId = 0)
        {
            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            SocketError error = SocketPal.SetIpIpv6(&__socketAddress_native, ip);
            if (error != SocketError.Success)
                return error;

            socketAddress.CopyFromIpv6(ref __socketAddress_native, port, scopeId);
            return SocketError.Success;
        }

        /// <summary>
        ///     Retrieves the address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the address structure.</param>
        /// <param name="ip">A span to receive the address chars.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIp(this NativeSocketAddress socketAddress, ref Span<char> ip)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError result = socketAddress.Family == AddressFamily.InterNetwork ? SocketPal.GetIpIpv4((sockaddr_in4*)&socketAddress, bytes) : SocketPal.GetIpIpv6((sockaddr_in6*)&socketAddress, bytes);
            if (result == SocketError.Success)
                return CopyAsciiBytesToChars(bytes, ref ip);

            return result;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> by resolving the specified host name to an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="hostName">The host name to resolve (e.g., "localhost", "example.com").</param>
        /// <param name="port">The port number.</param>
        /// <returns>
        ///     <see cref="SocketError.Success" /> if resolution succeeds and the address is filled successfully;
        ///     otherwise, an error code indicating the failure reason (e.g., host not found, invalid argument).
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv4(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> hostName, ushort port)
        {
            Unsafe.SkipInit(out sockaddr_in4 __socketAddress_native);
            SocketError error = SocketPal.SetHostNameIpv4(&__socketAddress_native, hostName);
            if (error != SocketError.Success)
                return error;

            socketAddress.CopyFromIpv4(ref __socketAddress_native, port);
            return SocketError.Success;
        }

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> by resolving the specified host name to an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="hostName">The host name to resolve (e.g., "localhost", "example.com").</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The Ipv6 scope identifier (used for link-local or site-local addresses).</param>
        /// <returns>
        ///     <see cref="SocketError.Success" /> if resolution succeeds and the address is filled successfully;
        ///     otherwise, an error code indicating the failure reason (e.g., host not found, invalid argument).
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostNameIpv6(ref this NativeSocketAddress socketAddress, ReadOnlySpan<char> hostName, ushort port, uint scopeId = 0)
        {
            Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
            SocketError error = SocketPal.SetHostNameIpv6(&__socketAddress_native, hostName);
            if (error != SocketError.Success)
                return error;

            socketAddress.CopyFromIpv6(ref __socketAddress_native, port, scopeId);
            return SocketError.Success;
        }

        /// <summary>
        ///     Gets the host name (reverse DNS) from an address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the address structure.</param>
        /// <param name="hostName">A span to receive the host name chars.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName(this NativeSocketAddress socketAddress, ref Span<char> hostName)
        {
            Span<byte> bytes = stackalloc byte[WinSock2.NI_MAXHOST];
            SocketError result = socketAddress.Family == AddressFamily.InterNetwork ? SocketPal.GetHostNameIpv4((sockaddr_in4*)&socketAddress, bytes) : SocketPal.GetHostNameIpv6((sockaddr_in6*)&socketAddress, bytes);
            if (result == SocketError.Success)
                return CopyAsciiBytesToChars(bytes, ref hostName);

            return result;
        }

        /// <summary>
        ///     Copies an Ipv4 socket address structure into a <see cref="NativeSocketAddress" />,
        ///     converting the port to network byte order and zeroing the padding.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="__socketAddress_native">The source Ipv4 address structure.</param>
        /// <param name="port">The port number in host byte order.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyFromIpv4(ref this NativeSocketAddress socketAddress, ref sockaddr_in4 __socketAddress_native, ushort port)
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
        private static void CopyFromIpv6(ref this NativeSocketAddress socketAddress, ref sockaddr_in6 __socketAddress_native, ushort port, uint scopeId)
        {
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = WinSock2.HOST_TO_NET_16(port);
            __socketAddress_native.sin6_flowinfo = 0;
            __socketAddress_native.sin6_scope_id = scopeId;
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), ref Unsafe.As<sockaddr_in6, byte>(ref __socketAddress_native), 28);
        }

        /// <summary>
        ///     Extracts the ASCII string from a null-terminated byte
        ///     span and copies it into a character span.
        /// </summary>
        /// <param name="source">The null-terminated ASCII byte span (typically from native APIs).</param>
        /// <param name="destination">
        ///     The character span to receive the decoded string.
        ///     On success, it is resized to the actual character count.
        /// </param>
        /// <returns>
        ///     <see cref="SocketError.Success" /> if the extraction and conversion succeed;
        ///     <see cref="SocketError.Fault" /> if the source does not contain a valid null terminator or is empty;
        ///     <see cref="SocketError.NoBufferSpaceAvailable" /> if the destination span is too small.
        /// </returns>
        private static SocketError CopyAsciiBytesToChars(ReadOnlySpan<byte> source, ref Span<char> destination)
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
    }
}