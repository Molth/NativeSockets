using System;
using System.Net;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides extension methods for <see cref="SocketAddress" />.
    /// </summary>
    internal static class SocketAddressExtensions
    {
        /// <summary>
        ///     Copies raw address data from a byte span into a <see cref="SocketAddress" />.
        ///     The first two bytes of the source are assumed to be the address family and are skipped.
        /// </summary>
        /// <param name="destination">The destination <see cref="SocketAddress" /> to populate.</param>
        /// <param name="source">The source byte span containing the raw address data.</param>
        /// <param name="size">
        ///     The total number of bytes to copy, including the address family prefix.
        ///     Must match the size of the socket address.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="size" /> is larger than the capacity of the destination buffer,
        ///     or when <paramref name="source" /> is shorter than <paramref name="size" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyFromWithoutFamily(this SocketAddress destination, ReadOnlySpan<byte> source, int size)
        {
#if NET8_0_OR_GREATER
            source.Slice(2, size - 2).CopyTo(destination.Buffer.Span.Slice(2, size - 2));
#else
            for (int i = 2; i < size; ++i)
                destination[i] = source[i];
#endif
        }

        /// <summary>
        ///     Copies raw address data from a <see cref="SocketAddress" /> into a byte span.
        ///     The first two bytes of the source are assumed to be the address family and are skipped.
        /// </summary>
        /// <param name="source">The source <see cref="SocketAddress" /> containing the address data.</param>
        /// <param name="destination">The destination byte span to receive the raw address data.</param>
        /// <param name="size">
        ///     The total number of bytes to copy, including the address family prefix.
        ///     Must match the size of the socket address.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="size" /> is larger than the capacity of the destination buffer,
        ///     or when <paramref name="source" /> is shorter than <paramref name="size" />.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToWithoutFamily(this SocketAddress source, Span<byte> destination, int size)
        {
#if NET8_0_OR_GREATER
            source.Buffer.Span.Slice(2, size - 2).CopyTo(destination.Slice(2, size - 2));
#else
            for (int i = 2; i < size; ++i)
                destination[i] = source[i];
#endif
        }
    }
}