using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

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
        ///     Serializes a <see cref="NativeSocketAddress" /> into the specified byte span.
        /// </summary>
        /// <param name="socketAddress">The socket address to serialize.</param>
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
        public static SocketError Serialize(in this NativeSocketAddress socketAddress, ref Span<byte> destination) => NativeSocketAddressPal.Serialize(ref destination, socketAddress);

        /// <summary>
        ///     Tries to format the value of the current instance as an <see cref="IPEndPoint" />,
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
        ///     Retrieves the ip address from a <see cref="NativeSocketAddress" /> as text.
        /// </summary>
        /// <param name="socketAddress">The <see cref="NativeSocketAddress" /> to read the ip address from.</param>
        /// <param name="destination">The character span to receive the ip address; resized to the actual length on success.</param>
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
    }
}