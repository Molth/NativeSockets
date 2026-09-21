using System;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides an implementation of the XxHash64 hash algorithm for generating a 64-bit hash.
    /// </summary>
    internal static class XxHash64
    {
        /// <summary>
        ///     Computes the hash of the provided data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="seed">The seed value for this hash computation. The default is zero.</param>
        /// <returns>The computed hash.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong HashToUInt64(ReadOnlySpan<byte> source, ulong seed = default) => xxhash_rust.XxHash64.xxh64(source, seed);
    }
}