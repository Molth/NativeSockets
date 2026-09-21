using System;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides an implementation of the XxHash32 hash algorithm for generating a 32-bit hash.
    /// </summary>
    internal static class XxHash32
    {
        /// <summary>
        ///     Computes the hash of the provided data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="seed">The seed value for this hash computation. The default is zero.</param>
        /// <returns>The computed hash.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint HashToUInt32(ReadOnlySpan<byte> source, uint seed = default) => xxhash_rust.XxHash32.xxh32(source, seed);
    }
}