using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides extension methods for <see cref="Socket" /> that perform non‑allocating send and receive operations.
    /// </summary>
    public static class SocketExtensions
    {
        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(this Socket socket, int microseconds, SelectModeFlags mode, out SelectModeFlags status) => new NativeSocket(socket).PollFlags(microseconds, mode, out status);

        /// <summary>
        ///     Sends data to an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="socketAddress">Pointer to the destination socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToNonAlloc(this Socket socket, ReadOnlySpan<byte> buffer, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendTo(buffer, socketAddress);

        /// <summary>
        ///     Sends data to an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToNonAlloc(this Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendTo(buffer, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data from an endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="socketAddress">Pointer to the sender's address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromNonAlloc(this Socket socket, Span<byte> buffer, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveFrom(buffer, ref socketAddress);

        /// <summary>
        ///     Receives data from an endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromNonAlloc(this Socket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveFrom(buffer, socketFlags, ref socketAddress);

        /// <summary>
        ///     Sends a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageNonAlloc(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags = SocketFlags.None) => new NativeSocket(socket).SendMessage(buffers, socketFlags);

        /// <summary>
        ///     Sends a message to an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketAddress">Pointer to the destination socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToNonAlloc(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendMessageTo(buffers, socketAddress);

        /// <summary>
        ///     Sends a message to an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToNonAlloc(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendMessageTo(buffers, socketFlags, socketAddress);

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageNonAlloc(this Socket socket, Span<NativeIoSlice> buffers) => new NativeSocket(socket).ReceiveMessage(buffers);

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageNonAlloc(this Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags socketFlags) => new NativeSocket(socket).ReceiveMessage(buffers, ref socketFlags);

        /// <summary>
        ///     Receives a message from an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketAddress">Pointer to the sender's socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromNonAlloc(this Socket socket, Span<NativeIoSlice> buffers, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveMessageFrom(buffers, ref socketAddress);

        /// <summary>
        ///     Receives a message from an endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromNonAlloc(this Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveMessageFrom(buffers, ref socketFlags, ref socketAddress);
    }
}