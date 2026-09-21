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
        ///     Maps this socket address to an Ipv6 socket address.
        /// </summary>
        /// <param name="socketAddress">The socket address to map.</param>
        /// <param name="scopeId">The Ipv6 scope id.</param>
        /// <param name="result">When this method returns, contains the mapped socket address.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <para>
        ///                 If the family is already Ipv6, the socket address is returned as-is.
        ///             </para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 If the family is Ipv4, the socket address is converted to an Ipv4-mapped Ipv6
        ///                 (<c>::ffff:a.b.c.d</c>), and <paramref name="scopeId" /> is set.
        ///             </para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 If the family is neither Ipv4 nor Ipv6, returns
        ///                 <see cref="SocketError.AddressFamilyNotSupported" />.
        ///             </para>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static SocketError MapToIpv6(this NativeSocketAddress socketAddress, uint scopeId, out NativeSocketAddress result)
        {
            if (socketAddress.IsIpv6)
            {
                result = socketAddress;
                return SocketError.Success;
            }

            if (!socketAddress.IsIpv4)
            {
                Unsafe.SkipInit(out result);
                return SocketError.AddressFamilyNotSupported;
            }

            result = new NativeSocketAddress();
            result.Family = AddressFamily.InterNetworkV6;
            result.Port = socketAddress.Port;

            WinSock2.MapIpv4ToIpv6(result.Ip, socketAddress.Ip);
            result.ScopeId = scopeId;

            return SocketError.Success;
        }

        /// <summary>
        ///     Maps this socket address to an Ipv4 socket address.
        /// </summary>
        /// <param name="socketAddress">The socket address to map.</param>
        /// <param name="result">When this method returns, contains the mapped socket address.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <para>
        ///                 If the family is already Ipv4, the socket address is returned as-is.
        ///             </para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 If the family is Ipv6 and the socket address is Ipv4-mapped,
        ///                 the embedded Ipv4 is extracted. <br />
        ///                 The Ipv6-specific fields (scope id, flow info) are discarded.
        ///             </para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 If the family is Ipv6 but the socket address is not Ipv4-mapped, returns
        ///                 <see cref="SocketError.InvalidArgument" />.
        ///             </para>
        ///         </item>
        ///         <item>
        ///             <para>
        ///                 If the family is neither Ipv4 nor Ipv6, returns
        ///                 <see cref="SocketError.AddressFamilyNotSupported" />.
        ///             </para>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static SocketError MapToIpv4(this NativeSocketAddress socketAddress, out NativeSocketAddress result)
        {
            if (socketAddress.IsIpv4)
            {
                result = socketAddress;
                return SocketError.Success;
            }

            if (!socketAddress.IsIpv4MappedToIpv6)
            {
                Unsafe.SkipInit(out result);
                return socketAddress.IsIpv6 ? SocketError.InvalidArgument : SocketError.AddressFamilyNotSupported;
            }

            result = new NativeSocketAddress();
            result.Family = AddressFamily.InterNetwork;
            result.Port = socketAddress.Port;

            WinSock2.MapIpv4MappedIpv6ToIpv4(result.Ip, socketAddress.Ip);

            return SocketError.Success;
        }

        /// <summary>
        ///     Serializes a <see cref="NativeSocketAddress" /> into the specified byte span.
        /// </summary>
        /// <param name="socketAddress">The socket address to serialize.</param>
        /// <param name="destination">
        ///     The byte span to receive the serialized socket address. On return, it is sliced
        ///     to the number of bytes actually written.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 An Ipv4 socket address is serialized as <c>8</c> bytes.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 An Ipv6 socket address is serialized as <c>28</c> bytes.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Serialize(in this NativeSocketAddress socketAddress, ref Span<byte> destination) => NativeSocketAddressPal.Serialize(ref destination, socketAddress);

        /// <summary>
        ///     Tries to format a <see cref="NativeSocketAddress" /> as an <see cref="IPEndPoint" />,
        ///     into the provided span of characters.
        /// </summary>
        /// <param name="socketAddress">The socket address to format.</param>
        /// <param name="destination">
        ///     The character span to receive the <see cref="IPEndPoint" /> string;
        ///     resized to the actual length on success.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError TryFormat(in this NativeSocketAddress socketAddress, ref Span<char> destination)
        {
            Span<char> chars = stackalloc char[NativeSocketAddressPal.FORMAT_MAX_CHARS];
            SocketError error = NativeSocketAddressPal.FormatAsIpEndPoint(ref chars, socketAddress);
            if (error != SocketError.Success)
                return error;

            if (chars.TryCopyTo(destination))
            {
                destination = destination.Slice(0, chars.Length);
                return SocketError.Success;
            }

            return SocketError.NoBufferSpaceAvailable;
        }

        /// <summary>
        ///     Retrieves the ip from a <see cref="NativeSocketAddress" /> as text.
        /// </summary>
        /// <param name="socketAddress">The <see cref="NativeSocketAddress" /> to read the ip from.</param>
        /// <param name="destination">The character span to receive the ip; resized to the actual length on success.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIp(this NativeSocketAddress socketAddress, ref Span<char> destination)
        {
            if (!socketAddress.IsIpv4 && !socketAddress.IsIpv6)
                return SocketError.AddressFamilyNotSupported;

            bool result = socketAddress.IsIpv4 ? IpAddressParser.TryFormatIpv4(socketAddress.Ip, ref destination) : IpAddressParser.TryFormatIpv6(socketAddress.Ip, ref destination);
            return result ? SocketError.Success : SocketError.NoBufferSpaceAvailable;
        }

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
            SocketError error = socketAddress.ToIpAddress(out IPAddress? ipAddress);
            if (error != SocketError.Success)
            {
                result = default;
                return error;
            }

            result = new IPEndPoint(ipAddress!, socketAddress.Port);
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
    }
}