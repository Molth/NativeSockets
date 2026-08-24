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
        ///     Sends data to a specified endpoint using
        ///     a strongly‑typed <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffer">The buffer containing the data to send.</param>
        /// <param name="socketAddress">The destination address.</param>
        /// <returns>The total number of bytes sent.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToNonAlloc(this Socket socket, ReadOnlySpan<byte> buffer, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendTo(buffer, socketAddress);

        /// <summary>
        ///     Sends data to a specified endpoint using
        ///     a strongly‑typed <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffer">The buffer containing the data to send.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination address.</param>
        /// <returns>The total number of bytes sent.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToNonAlloc(this Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendTo(buffer, socketFlags, socketAddress);

        /// <summary>
        ///     Receives data from a remote endpoint and fills the provided <see cref="NativeSocketAddress" /> with the sender's
        ///     address.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffer">The buffer to store the received data.</param>
        /// <param name="socketAddress">A reference to a <see cref="NativeSocketAddress" /> that will receive the sender's address.</param>
        /// <returns>The number of bytes received.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromNonAlloc(this Socket socket, Span<byte> buffer, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveFrom(buffer, ref socketAddress);

        /// <summary>
        ///     Receives data from a remote endpoint and fills
        ///     the provided <see cref="NativeSocketAddress" /> with the sender's address.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffer">The buffer to store the received data.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">A reference to a <see cref="NativeSocketAddress" /> that will receive the sender's address.</param>
        /// <returns>The number of bytes received.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromNonAlloc(this Socket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveFrom(buffer, socketFlags, ref socketAddress);

        /// <summary>
        ///     Sends a message (scatter/gather) on a connected socket without allocating.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffers">The buffers containing the data to send.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The total number of bytes sent.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageNonAlloc(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags = SocketFlags.None) => new NativeSocket(socket).SendMessage(buffers, socketFlags);

        /// <summary>
        ///     Sends a message to a specified endpoint using a strongly‑typed <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffers">The buffers containing the data to send.</param>
        /// <param name="socketAddress">The destination address.</param>
        /// <returns>The total number of bytes sent.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToNonAlloc(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendMessageTo(buffers, socketAddress);

        /// <summary>
        ///     Sends a message to a specified endpoint
        ///     using a strongly‑typed <see cref="NativeSocketAddress" />.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffers">The buffers containing the data to send.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination address.</param>
        /// <returns>The total number of bytes sent.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendMessageToNonAlloc(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress) => new NativeSocket(socket).SendMessageTo(buffers, socketFlags, socketAddress);

        /// <summary>
        ///     Receives a message on a connected socket without allocating.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffers">The buffers to store the received data.</param>
        /// <returns>The number of bytes received.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageNonAlloc(this Socket socket, Span<NativeIoSlice> buffers) => new NativeSocket(socket).ReceiveMessage(buffers);

        /// <summary>
        ///     Receives a message on a connected socket without allocating.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffers">The buffers to store the received data.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>The number of bytes received.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageNonAlloc(this Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags socketFlags) => new NativeSocket(socket).ReceiveMessage(buffers, ref socketFlags);

        /// <summary>
        ///     Receives a message from a remote endpoint and fills the provided <see cref="NativeSocketAddress" /> with the
        ///     sender's address.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffers">The buffers to store the received data.</param>
        /// <param name="socketAddress">A reference to a <see cref="NativeSocketAddress" /> that will receive the sender's address.</param>
        /// <returns>The number of bytes received.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromNonAlloc(this Socket socket, Span<NativeIoSlice> buffers, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveMessageFrom(buffers, ref socketAddress);

        /// <summary>
        ///     Receives a message from a remote endpoint and fills
        ///     the provided <see cref="NativeSocketAddress" /> with the sender's address.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffers">The buffers to store the received data.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">A reference to a <see cref="NativeSocketAddress" /> that will receive the sender's address.</param>
        /// <returns>The number of bytes received.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveMessageFromNonAlloc(this Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags socketFlags, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveMessageFrom(buffers, ref socketFlags, ref socketAddress);
    }
}