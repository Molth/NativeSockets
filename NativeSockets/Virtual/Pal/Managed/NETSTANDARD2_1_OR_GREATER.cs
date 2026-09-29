#if !NET6_0_OR_GREATER
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

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
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult SendTo(Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            if (socket.AddressFamily != socketAddress.Family)
                return new IoResult(-1, SocketError.AddressFamilyNotSupported);

            SocketError error = socketAddress.ToIpEndPoint(out IPEndPoint? ipEndPoint);
            if (error != SocketError.Success)
                return new IoResult(-1, error);

            byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                buffer.CopyTo(array);
                int num = socket.SendTo(array, 0, buffer.Length, WindowsSocketFlags.ToNativeSocketFlags(socketFlags), ipEndPoint!);

                return num >= 0 ? new IoResult(num, SocketError.Success) : new IoResult(-1, SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
            }
            finally
            {
                if (array is { Length: > 0 })
                    ArrayPool<byte>.Shared.Return(array);
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
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult ReceiveFrom(Socket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress)
        {
            EndPoint ipEndPoint = GetReceiveFromIpEndPoint(socket);

            byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                int num = socket.ReceiveFrom(array, 0, buffer.Length, WindowsSocketFlags.ToNativeSocketFlags(socketFlags), ref ipEndPoint);

                if (num >= 0)
                {
                    array.AsSpan(0, num).CopyTo(buffer);

                    NativeSocketAddress.FromIpEndPoint((IPEndPoint)ipEndPoint, out socketAddress);

                    return new IoResult(num, SocketError.Success);
                }

                return new IoResult(-1, SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
            }
            finally
            {
                if (array is { Length: > 0 })
                    ArrayPool<byte>.Shared.Return(array);
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
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
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
                    Build(buffers, out array, out int bufferLength);
                    buffer = array.AsSpan(0, bufferLength);
                    break;
            }

            try
            {
                int num = socket.Send(buffer, WindowsSocketFlags.ToNativeSocketFlags(socketFlags), out SocketError socketError);

                return new IoResult(num >= 0 ? num : -1, socketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
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
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult SendToVectored(Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress)
        {
            if (socket.AddressFamily != socketAddress.Family)
                return new IoResult(-1, SocketError.AddressFamilyNotSupported);

            SocketError error = socketAddress.ToIpEndPoint(out IPEndPoint? ipEndPoint);
            if (error != SocketError.Success)
                return new IoResult(-1, error);

            byte[] array;
            int bufferLength;

            switch (buffers.Length)
            {
                case 0:
                    array = Array.Empty<byte>();
                    bufferLength = 0;
                    break;

                default:
                    Build(buffers, out array, out bufferLength);
                    break;
            }

            try
            {
                int num = socket.SendTo(array, 0, bufferLength, WindowsSocketFlags.ToNativeSocketFlags(socketFlags), ipEndPoint!);

                return num >= 0 ? new IoResult(num, SocketError.Success) : new IoResult(-1, SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
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
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult ReceiveVectored(Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags inOutFlags)
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
                    Build(buffers, out array, out int bufferLength);
                    buffer = array.AsSpan(0, bufferLength);
                    break;
            }

            try
            {
                int num = socket.Receive(buffer, Unsafe.IsNullRef(ref inOutFlags) ? 0 : WindowsSocketFlags.ToNativeSocketFlags(inOutFlags), out SocketError socketError);

                if (num >= 0)
                {
                    CopyReceived(array, buffer, num, buffers);

                    if (!Unsafe.IsNullRef(ref inOutFlags))
                        inOutFlags = 0;

                    return new IoResult(num, socketError);
                }

                return new IoResult(-1, socketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
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
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static IoResult ReceiveFromVectored(Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags inOutFlags, ref NativeSocketAddress socketAddress)
        {
            EndPoint ipEndPoint = GetReceiveFromIpEndPoint(socket);

            byte[] array;
            int bufferLength;

            switch (buffers.Length)
            {
                case 0:
                    array = Array.Empty<byte>();
                    bufferLength = 0;
                    break;

                default:
                    Build(buffers, out array, out bufferLength);
                    break;
            }

            try
            {
                int num = socket.ReceiveFrom(array, 0, bufferLength, Unsafe.IsNullRef(ref inOutFlags) ? 0 : WindowsSocketFlags.ToNativeSocketFlags(inOutFlags), ref ipEndPoint);

                if (num >= 0)
                {
                    CopyReceived(array, array.AsSpan(0, bufferLength), num, buffers);

                    if (!Unsafe.IsNullRef(ref inOutFlags))
                        inOutFlags = 0;

                    NativeSocketAddress.FromIpEndPoint((IPEndPoint)ipEndPoint, out socketAddress);

                    return new IoResult(num, SocketError.Success);
                }

                return new IoResult(-1, SocketError.SocketError);
            }
            catch (SocketException ex)
            {
                return new IoResult(-1, ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return new IoResult(-1, SocketError.NotSocket);
            }
            catch
            {
                return new IoResult(-1, SocketError.SocketError);
            }
            finally
            {
                if (array is { Length: > 0 })
                    ArrayPool<byte>.Shared.Return(array);
            }
        }

        /// <summary>
        ///     Copies the buffers into a single pooled array.
        /// </summary>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="array">When this method returns, contains the pooled array holding the combined buffers.</param>
        /// <param name="bufferLength">When this method returns, contains the total length of the combined buffers.</param>
        private static void Build(ReadOnlySpan<NativeIoSlice> buffers, out byte[] array, out int bufferLength)
        {
            bufferLength = 0;
            for (int i = 0; i < buffers.Length; ++i)
                bufferLength += buffers[i].Length;

            if (bufferLength == 0)
            {
                array = Array.Empty<byte>();
                return;
            }

            array = ArrayPool<byte>.Shared.Rent(bufferLength);
            Span<byte> span = array.AsSpan(0, bufferLength);

            int offset = 0;
            for (int i = 0; i < buffers.Length; ++i)
            {
                buffers[i].AsReadOnlySpan().CopyTo(span.Slice(offset));
                offset += buffers[i].Length;
            }
        }

        /// <summary>
        ///     Copies the received bytes from the pooled array back into the buffers.
        /// </summary>
        /// <param name="array">The pooled array holding the received bytes, or null when a single buffer was used.</param>
        /// <param name="buffer">The received bytes.</param>
        /// <param name="received">The number of bytes received.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        private static void CopyReceived(byte[]? array, Span<byte> buffer, int received, Span<NativeIoSlice> buffers)
        {
            if (array == null)
                return;

            int offset = 0;
            int remaining = received;
            for (int i = 0; i < buffers.Length && remaining > 0; ++i)
            {
                int length = Math.Min(buffers[i].Length, remaining);
                buffer.Slice(offset, length).CopyTo(buffers[i].AsSpan());
                offset += length;
                remaining -= length;
            }
        }
    }
}
#endif