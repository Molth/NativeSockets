using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides extension methods for <see cref="NativeSocket" />.
    /// </summary>
    public static unsafe class NativeSocketExtensions
    {
        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetDualMode(this NativeSocket socket, bool dualMode) => SocketPal.SetDualModeIpv6(socket, dualMode);

        /// <summary>
        ///     Binds a socket to an address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind(this NativeSocket socket, NativeSocketAddress socketAddress) => socket.IsIpv4 ? SocketPal.BindIpv4(socket, (sockaddr_in4*)&socketAddress) : SocketPal.BindIpv6(socket, (sockaddr_in6*)&socketAddress);

        /// <summary>
        ///     Connects a socket to an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(this NativeSocket socket, NativeSocketAddress socketAddress) => socket.IsIpv4 ? SocketPal.ConnectIpv4(socket, (sockaddr_in4*)&socketAddress) : SocketPal.ConnectIpv6(socket, (sockaddr_in6*)&socketAddress);

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to the option value.</param>
        /// <param name="length">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(this NativeSocket socket, SocketOptionLevel level, SocketOptionName name, ref int value, int length = sizeof(int))
        {
            fixed (int* pValue = &value)
            {
                return SocketPal.SetOption(socket, level, name, pValue, length);
            }
        }

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <param name="length">Pointer to the length of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(this NativeSocket socket, SocketOptionLevel level, SocketOptionName name, ref int value, ref int length)
        {
            fixed (int* pValue = &value)
            {
                fixed (int* pLength = &length)
                {
                    return SocketPal.GetOption(socket, level, name, pValue, pLength);
                }
            }
        }

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(this NativeSocket socket, SocketOptionLevel level, SocketOptionName name, ref int value)
        {
            fixed (int* pValue = &value)
            {
                return SocketPal.GetOption(socket, level, name, pValue);
            }
        }

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(this NativeSocket socket, bool blocking) => SocketPal.SetBlocking(socket, blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(this NativeSocket socket, int microseconds, SelectMode mode, out bool status) => SocketPal.Poll(socket, microseconds, mode, out status);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Send(this NativeSocket socket, ReadOnlySpan<byte> buffer)
        {
            fixed (void* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
            {
                return SocketPal.Send(socket, pinnedBuffer, buffer.Length);
            }
        }

        /// <summary>
        ///     Sends data to an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="socketAddress">Pointer to the destination socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(this NativeSocket socket, ReadOnlySpan<byte> buffer, in NativeSocketAddress socketAddress)
        {
            fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
            {
                fixed (void* __socketAddress_native = &socketAddress)
                {
                    return socket.IsIpv4 ? SocketPal.SendToIpv4(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in4*)__socketAddress_native) : SocketPal.SendToIpv6(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in6*)__socketAddress_native);
                }
            }
        }

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Receive(this NativeSocket socket, Span<byte> buffer)
        {
            fixed (void* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
            {
                return SocketPal.Receive(socket, pinnedBuffer, buffer.Length);
            }
        }

        /// <summary>
        ///     Receives data from an endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="socketAddress">Pointer to the sender's address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom(this NativeSocket socket, Span<byte> buffer, ref NativeSocketAddress socketAddress)
        {
            int result;
            fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
            {
                fixed (void* __socketAddress_native = &socketAddress)
                {
                    result = socket.IsIpv4 ? SocketPal.ReceiveFromIpv4(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in4*)__socketAddress_native) : SocketPal.ReceiveFromIpv6(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in6*)__socketAddress_native);
                }
            }

            if (socket.IsIpv4 && result >= 0)
                SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 16), 0, 12);

            return result;
        }

        /// <summary>
        ///     Sends a message (scatter/gather) on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessage(this NativeSocket socket, ReadOnlySpan<NativeIoSlice> buffers)
        {
            fixed (NativeIoSlice* pinnedBuffer = &MemoryMarshal.GetReference(buffers))
            {
                return SocketPal.SendMessage(socket.Handle, pinnedBuffer, buffers.Length);
            }
        }

        /// <summary>
        ///     Sends a message to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageTo(this NativeSocket socket, ReadOnlySpan<NativeIoSlice> buffers, in NativeSocketAddress socketAddress)
        {
            fixed (NativeIoSlice* pinnedBuffer = &MemoryMarshal.GetReference(buffers))
            {
                fixed (void* __socketAddress_native = &socketAddress)
                {
                    return socket.IsIpv4 ? SocketPal.SendMessageToIpv4(socket.Handle, pinnedBuffer, buffers.Length, (sockaddr_in4*)__socketAddress_native) : SocketPal.SendMessageToIpv6(socket.Handle, pinnedBuffer, buffers.Length, (sockaddr_in6*)__socketAddress_native);
                }
            }
        }

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessage(this NativeSocket socket, Span<NativeIoSlice> buffers)
        {
            fixed (NativeIoSlice* pinnedBuffer = &MemoryMarshal.GetReference(buffers))
            {
                return SocketPal.ReceiveMessage(socket.Handle, pinnedBuffer, buffers.Length);
            }
        }

        /// <summary>
        ///     Receives a message from an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFrom(this NativeSocket socket, Span<NativeIoSlice> buffers, ref NativeSocketAddress socketAddress)
        {
            int result;
            fixed (NativeIoSlice* pinnedBuffer = &MemoryMarshal.GetReference(buffers))
            {
                fixed (void* __socketAddress_native = &socketAddress)
                {
                    result = socket.IsIpv4 ? SocketPal.ReceiveMessageFromIpv4(socket.Handle, pinnedBuffer, buffers.Length, (sockaddr_in4*)__socketAddress_native) : SocketPal.ReceiveMessageFromIpv6(socket.Handle, pinnedBuffer, buffers.Length, (sockaddr_in6*)__socketAddress_native);
                }
            }

            if (socket.IsIpv4 && result >= 0)
                SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 16), 0, 12);

            return result;
        }

        /// <summary>
        ///     Gets the local name (address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(this NativeSocket socket, ref NativeSocketAddress socketAddress)
        {
            SocketError result;
            fixed (void* __socketAddress_native = &socketAddress)
            {
                result = socket.IsIpv4 ? SocketPal.GetNameIpv4(socket.Handle, (sockaddr_in4*)__socketAddress_native) : SocketPal.GetNameIpv6(socket.Handle, (sockaddr_in6*)__socketAddress_native);
            }

            if (result == SocketError.Success && socket.IsIpv4)
                SpanHelpers.Set(ref Unsafe.Add(ref Unsafe.As<NativeSocketAddress, byte>(ref socketAddress), 16), 0, 12);

            return result;
        }
    }
}