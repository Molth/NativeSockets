#if !NET8_0_OR_GREATER && NET6_0_OR_GREATER
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides managed virtual socket send and receive operations backed by <see cref="Socket" />.
    /// </summary>
    internal static partial class ManagedVirtualSocketPalImpl
    {
        /// <summary>
        ///     The thread-local IP endpoint used to receive Ipv4 socket addresses.
        /// </summary>
        [ThreadStatic] private static IPEndPoint? _receiveIpv4;

        /// <summary>
        ///     The thread-local IP endpoint used to receive Ipv6 socket addresses.
        /// </summary>
        [ThreadStatic] private static IPEndPoint? _receiveIpv6;

        /// <summary>
        ///     Gets the reusable IP endpoint used to receive the sender's socket address.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <returns>The endpoint used to receive the sender's socket address.</returns>
        private static EndPoint GetReceiveFromIpEndPoint(Socket socket) => socket.AddressFamily == AddressFamily.InterNetwork ? _receiveIpv4 ??= new IPEndPoint(IPAddress.Any, 0) : _receiveIpv6 ??= new IPEndPoint(IPAddress.IPv6Any, 0);

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult SendTo(Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            if (socket.AddressFamily != socketAddress.Family)
                return IoResult.Err(SocketError.AddressFamilyNotSupported);

            SocketError error = socketAddress.ToIpEndPoint(out IPEndPoint? ipEndPoint);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            try
            {
                int num = socket.SendTo(buffer, WindowsSocketFlags.ToNativeSendSocketFlags(socketFlags), ipEndPoint!);

                return num >= 0 ? IoResult.Ok(num) : IoResult.Err(SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return IoResult.Err(ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return IoResult.Err(SocketError.NotSocket);
            }
            catch
            {
                return IoResult.Err(SocketError.SocketError);
            }
        }

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult ReceiveFrom(Socket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress)
        {
            EndPoint ipEndPoint = GetReceiveFromIpEndPoint(socket);

            try
            {
                int num = socket.ReceiveFrom(buffer, WindowsSocketFlags.ToNativeReceiveSocketFlags(socketFlags), ref ipEndPoint);

                if (num >= 0)
                {
                    NativeSocketAddress.FromIpEndPoint((IPEndPoint)ipEndPoint, out socketAddress);

                    return IoResult.Ok(num);
                }

                return IoResult.Err(SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return IoResult.Err(ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return IoResult.Err(SocketError.NotSocket);
            }
            catch
            {
                return IoResult.Err(SocketError.SocketError);
            }
        }

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult SendVectored(Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags)
        {
            byte[]? array;
            ReadOnlySpan<byte> buffer;

            switch (buffers.Length)
            {
                case 0:
                    array = null;
                    buffer = Array.Empty<byte>();
                    break;

                case 1:
                    array = null;
                    buffer = buffers[0].AsReadOnlySpan();
                    break;

                default:
                    BuildSendBuffer(buffers, out array, out int bufferLength);
                    buffer = array.AsSpan(0, bufferLength);
                    break;
            }

            try
            {
                int num = socket.Send(buffer, WindowsSocketFlags.ToNativeSendSocketFlags(socketFlags), out SocketError socketError);

                return socketError == SocketError.Success ? IoResult.Ok(num) : IoResult.Err(socketError);
            }
            catch (SocketException ex)
            {
                return IoResult.Err(ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return IoResult.Err(SocketError.NotSocket);
            }
            catch
            {
                return IoResult.Err(SocketError.SocketError);
            }
            finally
            {
                if (array is { Length: > 0 })
                    ArrayPool<byte>.Shared.Return(array);
            }
        }

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult SendToVectored(Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            if (socket.AddressFamily != socketAddress.Family)
                return IoResult.Err(SocketError.AddressFamilyNotSupported);

            SocketError error = socketAddress.ToIpEndPoint(out IPEndPoint? ipEndPoint);
            if (error != SocketError.Success)
                return IoResult.Err(error);

            byte[]? array;
            ReadOnlySpan<byte> buffer;

            switch (buffers.Length)
            {
                case 0:
                    array = null;
                    buffer = Array.Empty<byte>();
                    break;

                case 1:
                    array = null;
                    buffer = buffers[0].AsReadOnlySpan();
                    break;

                default:
                    BuildSendBuffer(buffers, out array, out int bufferLength);
                    buffer = array.AsSpan(0, bufferLength);
                    break;
            }

            try
            {
                int num = socket.SendTo(buffer, WindowsSocketFlags.ToNativeSendSocketFlags(socketFlags), ipEndPoint!);

                return num >= 0 ? IoResult.Ok(num) : IoResult.Err(SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return IoResult.Err(ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return IoResult.Err(SocketError.NotSocket);
            }
            catch
            {
                return IoResult.Err(SocketError.SocketError);
            }
            finally
            {
                if (array is { Length: > 0 })
                    ArrayPool<byte>.Shared.Return(array);
            }
        }

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult ReceiveVectored(Socket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags)
        {
            byte[]? array;
            Span<byte> buffer;

            switch (buffers.Length)
            {
                case 0:
                    array = null;
                    buffer = Array.Empty<byte>();
                    break;

                case 1:
                    array = null;
                    buffer = buffers[0].AsSpan();
                    break;

                default:
                    RentBuffer(buffers, out array, out int bufferLength);
                    buffer = array.AsSpan(0, bufferLength);
                    break;
            }

            try
            {
                int num = socket.Receive(buffer, WindowsSocketFlags.ToNativeReceiveSocketFlags(socketFlags), out SocketError socketError);

                if (socketError == SocketError.Success)
                {
                    CopyReceived(array, buffers, buffer.Slice(0, num));

                    return IoResult.Ok(num);
                }

                return IoResult.Err(socketError);
            }
            catch (SocketException ex)
            {
                return IoResult.Err(ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return IoResult.Err(SocketError.NotSocket);
            }
            catch
            {
                return IoResult.Err(SocketError.SocketError);
            }
            finally
            {
                if (array is { Length: > 0 })
                    ArrayPool<byte>.Shared.Return(array);
            }
        }

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" /> and the socket is
        ///                 non-blocking, consider polling with <c>Poll</c> or <c>PollFlags</c> before sending or
        ///                 receiving, because frequent <see cref="SocketError.WouldBlock" /> exceptions have a severe
        ///                 impact on performance.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static IoResult ReceiveFromVectored(Socket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, ref NativeSocketAddress socketAddress)
        {
            EndPoint ipEndPoint = GetReceiveFromIpEndPoint(socket);

            byte[]? array;
            Span<byte> buffer;

            switch (buffers.Length)
            {
                case 0:
                    array = null;
                    buffer = Array.Empty<byte>();
                    break;

                case 1:
                    array = null;
                    buffer = buffers[0].AsSpan();
                    break;

                default:
                    RentBuffer(buffers, out array, out int bufferLength);
                    buffer = array.AsSpan(0, bufferLength);
                    break;
            }

            try
            {
                int num = socket.ReceiveFrom(buffer, WindowsSocketFlags.ToNativeReceiveSocketFlags(socketFlags), ref ipEndPoint);

                if (num >= 0)
                {
                    CopyReceived(array, buffers, buffer.Slice(0, num));

                    NativeSocketAddress.FromIpEndPoint((IPEndPoint)ipEndPoint, out socketAddress);

                    return IoResult.Ok(num);
                }

                return IoResult.Err(SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return IoResult.Err(ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return IoResult.Err(SocketError.NotSocket);
            }
            catch
            {
                return IoResult.Err(SocketError.SocketError);
            }
            finally
            {
                if (array is { Length: > 0 })
                    ArrayPool<byte>.Shared.Return(array);
            }
        }
    }
}
#endif