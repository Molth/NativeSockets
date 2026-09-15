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
        ///     Serializes the address into the specified byte span.
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
        public static SocketError Serialize(in this NativeSocketAddress socketAddress, ref Span<byte> destination) => NativeSocketAddressPal.Serialize(ref destination, socketAddress);

        /// <summary>
        ///     Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="socketAddress">The socket address to format.</param>
        /// <param name="destination">When this method returns, this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">
        ///     When this method returns, the number of characters that were written in
        ///     <paramref name="destination" />.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the formatting was successful;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryFormat(in this NativeSocketAddress socketAddress, Span<char> destination, out int charsWritten)
        {
            Span<char> chars = stackalloc char[NativeSocketAddressPal.FORMAT_MAX_CHARS];
            NativeSocketAddressPal.Format(ref chars, socketAddress);

            if (chars.TryCopyTo(destination))
            {
                charsWritten = chars.Length;
                return true;
            }

            charsWritten = 0;
            return false;
        }

        /// <summary>
        ///     Tries to format the value of the current instance as UTF-8 into the provided span of bytes.
        /// </summary>
        /// <param name="socketAddress">The socket address to format.</param>
        /// <param name="utf8Destination">The span in which to write this instance's value formatted as a span of bytes.</param>
        /// <param name="bytesWritten">
        ///     When this method returns, contains the number of bytes that were written in
        ///     <paramref name="utf8Destination" />.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the formatting was successful;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryFormat(in this NativeSocketAddress socketAddress, Span<byte> utf8Destination, out int bytesWritten)
        {
            Span<char> chars = stackalloc char[NativeSocketAddressPal.FORMAT_MAX_CHARS];
            NativeSocketAddressPal.Format(ref chars, socketAddress);

            int byteCount = Encoding.UTF8.GetByteCount(chars);
            if (byteCount <= utf8Destination.Length)
            {
                Encoding.UTF8.GetBytes(chars, utf8Destination);
                bytesWritten = byteCount;
                return true;
            }

            bytesWritten = 0;
            return false;
        }

        /// <summary>
        ///     Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="socketAddress">The socket address to format.</param>
        /// <param name="destination">When this method returns, this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">
        ///     When this method returns, the number of characters that were written in
        ///     <paramref name="destination" />.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError TryFormatAsIpEndPoint(in this NativeSocketAddress socketAddress, Span<char> destination, out int charsWritten)
        {
            if (!socketAddress.IsIpv4 && !socketAddress.IsIpv6)
            {
                charsWritten = 0;
                return SocketError.AddressFamilyNotSupported;
            }

            Span<char> chars = stackalloc char[NativeSocketAddressPal.FORMAT_MAX_CHARS];
            SocketError error = NativeSocketAddressPal.FormatAsIpEndPoint(ref chars, socketAddress);
            if (error != SocketError.Success)
            {
                charsWritten = 0;
                return error;
            }

            if (chars.TryCopyTo(destination))
            {
                charsWritten = chars.Length;
                return SocketError.Success;
            }

            charsWritten = 0;
            return SocketError.NoBufferSpaceAvailable;
        }

        /// <summary>
        ///     Tries to format the value of the current instance as UTF-8 into the provided span of bytes.
        /// </summary>
        /// <param name="socketAddress">The socket address to format.</param>
        /// <param name="utf8Destination">The span in which to write this instance's value formatted as a span of bytes.</param>
        /// <param name="bytesWritten">
        ///     When this method returns, contains the number of bytes that were written in
        ///     <paramref name="utf8Destination" />.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public static SocketError TryFormatAsIpEndPoint(in this NativeSocketAddress socketAddress, Span<byte> utf8Destination, out int bytesWritten)
        {
            if (!socketAddress.IsIpv4 && !socketAddress.IsIpv6)
            {
                bytesWritten = 0;
                return SocketError.AddressFamilyNotSupported;
            }

            Span<char> chars = stackalloc char[NativeSocketAddressPal.FORMAT_MAX_CHARS];
            SocketError error = NativeSocketAddressPal.FormatAsIpEndPoint(ref chars, socketAddress);
            if (error != SocketError.Success)
            {
                bytesWritten = 0;
                return error;
            }

            int byteCount = Encoding.UTF8.GetByteCount(chars);
            if (byteCount <= utf8Destination.Length)
            {
                Encoding.UTF8.GetBytes(chars, utf8Destination);
                bytesWritten = byteCount;
                return SocketError.Success;
            }

            bytesWritten = 0;
            return SocketError.NoBufferSpaceAvailable;
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
                return NativeSocketAddressPal.GetAsciiCharsFromBytes(ref ip, bytes);

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
                return NativeSocketAddressPal.GetAsciiCharsFromBytes(ref hostName, bytes);

            return result;
        }
    }
}