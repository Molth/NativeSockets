using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

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
        /// <param name="ipEndPoint">The <see cref="IPEndPoint" /> containing the ip address and port.</param>
        /// <returns>
        ///     <see cref="SocketError.Success" /> if successful;
        ///     <see cref="SocketError.AddressFamilyNotSupported" /> if the address family is not Ipv4 or Ipv6.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIp(ref this NativeSocketAddress socketAddress, IPEndPoint ipEndPoint) => socketAddress.SetIp(ipEndPoint.Address, (ushort)ipEndPoint.Port, ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6 ? (uint)ipEndPoint.Address.ScopeId : 0);

        /// <summary>
        ///     Populates a <see cref="NativeSocketAddress" /> from the specified <see cref="IPAddress" />, port, and scope id.
        /// </summary>
        /// <param name="socketAddress">The destination <see cref="NativeSocketAddress" /> to fill.</param>
        /// <param name="address">The <see cref="IPAddress" /> to set.</param>
        /// <param name="port">The port number.</param>
        /// <param name="scopeId">The Ipv6 scope identifier (ignored for Ipv4).</param>
        /// <returns>
        ///     <see cref="SocketError.Success" /> if successful;
        ///     <see cref="SocketError.AddressFamilyNotSupported" /> if the address family is not Ipv4 or Ipv6.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIp(ref this NativeSocketAddress socketAddress, IPAddress address, ushort port, uint scopeId = 0)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork || address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                socketAddress = new NativeSocketAddress();
                socketAddress.Family = address.AddressFamily;
                socketAddress.Port = port;
                address.TryWriteBytes(socketAddress.Address, out _);
            }

            switch (address.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return SocketError.Success;

                case AddressFamily.InterNetworkV6:
                    socketAddress.ScopeId = scopeId;
                    return SocketError.Success;

                default:
                    return SocketError.AddressFamilyNotSupported;
            }
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

            __socketAddress_native.sin4_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
            __socketAddress_native.sin4_port = WinSock2.HOST_TO_NET_16(port);
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), ref Unsafe.As<sockaddr_in4, byte>(ref __socketAddress_native), 8);
            SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 8), 0, 20);
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

            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = WinSock2.HOST_TO_NET_16(port);
            __socketAddress_native.sin6_flowinfo = 0;
            __socketAddress_native.sin6_scope_id = scopeId;
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), ref Unsafe.As<sockaddr_in6, byte>(ref __socketAddress_native), 28);
            return SocketError.Success;
        }

        /// <summary>
        ///     Retrieves the Ipv4 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIp(this NativeSocketAddress socketAddress, ref Span<byte> ip)
        {
            SocketError result = socketAddress.Family == AddressFamily.InterNetwork ? SocketPal.GetIpIpv4((sockaddr_in4*)&socketAddress, ip) : SocketPal.GetIpIpv6((sockaddr_in6*)&socketAddress, ip);
            if (result == SocketError.Success)
            {
                int index = ip.IndexOf((byte)'\0');
                if (index <= 0)
                    return SocketError.Fault;

                ip = ip.Slice(0, index);
            }

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

            __socketAddress_native.sin4_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V4;
            __socketAddress_native.sin4_port = WinSock2.HOST_TO_NET_16(port);
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), ref Unsafe.As<sockaddr_in4, byte>(ref __socketAddress_native), 8);
            SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 8), 0, 20);
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

            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = WinSock2.HOST_TO_NET_16(port);
            __socketAddress_native.sin6_flowinfo = 0;
            __socketAddress_native.sin6_scope_id = scopeId;
            SpanHelpers.Copy(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), ref Unsafe.As<sockaddr_in6, byte>(ref __socketAddress_native), 28);
            return SocketError.Success;
        }

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName(this NativeSocketAddress socketAddress, ref Span<byte> hostName)
        {
            SocketError result = socketAddress.Family == AddressFamily.InterNetwork ? SocketPal.GetHostNameIpv4((sockaddr_in4*)&socketAddress, hostName) : SocketPal.GetHostNameIpv6((sockaddr_in6*)&socketAddress, hostName);
            if (result == SocketError.Success)
            {
                int index = hostName.IndexOf((byte)'\0');
                if (index <= 0)
                    return SocketError.Fault;

                hostName = hostName.Slice(0, index);
            }

            return result;
        }
    }
}