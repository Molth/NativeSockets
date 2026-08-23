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
        ///     Receives data from a remote endpoint and fills
        ///     the provided <see cref="NativeSocketAddress" /> with the sender's address.
        /// </summary>
        /// <param name="socket">The socket instance.</param>
        /// <param name="buffer">The buffer to store the received data.</param>
        /// <param name="socketAddress">
        ///     A reference to a <see cref="NativeSocketAddress" /> that will receive the sender's
        ///     address.
        /// </param>
        /// <returns>The number of bytes received.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the socket address family is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromNonAlloc(this Socket socket, Span<byte> buffer, ref NativeSocketAddress socketAddress) => new NativeSocket(socket).ReceiveFrom(buffer, ref socketAddress);
    }
}