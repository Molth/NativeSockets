using System.Runtime.CompilerServices;
using rust;

// ReSharper disable ALL

namespace xxhash_rust
{
    /// <remarks>https://github.com/DoumanAsh/xxhash-rust</remarks>
    internal static partial class XxHash64
    {
        /// <summary>
        ///     The number of bytes processed per chunk in the main loop (4 × <see cref="ulong" />).
        /// </summary>
        private const int CHUNK_SIZE = sizeof(ulong) * 4;

        /// <summary>
        ///     XXH64 prime 1.
        /// </summary>
        private const ulong PRIME_1 = 0x9E3779B185EBCA87;

        /// <summary>
        ///     XXH64 prime 2.
        /// </summary>
        private const ulong PRIME_2 = 0xC2B2AE3D27D4EB4F;

        /// <summary>
        ///     XXH64 prime 3.
        /// </summary>
        private const ulong PRIME_3 = 0x165667B19E3779F9;

        /// <summary>
        ///     XXH64 prime 4.
        /// </summary>
        private const ulong PRIME_4 = 0x85EBCA77C2B2AE63;

        /// <summary>
        ///     XXH64 prime 5.
        /// </summary>
        private const ulong PRIME_5 = 0x27D4EB2F165667C5;

        /// <summary>
        ///     Accumulates a single lane of the hash for a 64-bit chunk.
        /// </summary>
        /// <param name="acc">The accumulator for this lane.</param>
        /// <param name="input">The 64-bit input value.</param>
        /// <returns>The updated accumulator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong round(ulong acc, ulong input) =>
            acc.wrapping_add(input.wrapping_mul(PRIME_2))
                .rotate_left(31)
                .wrapping_mul(PRIME_1);

        /// <summary>
        ///     Merges a lane accumulator into the final hash during the consolidation step.
        /// </summary>
        /// <param name="acc">The running hash accumulator.</param>
        /// <param name="val">The lane value to merge.</param>
        /// <returns>The updated accumulator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong merge_round(ulong acc, ulong val)
        {
            acc ^= round(0, val);
            return acc.wrapping_mul(PRIME_1).wrapping_add(PRIME_4);
        }

        /// <summary>
        ///     Final mixing step that spreads the bits of the hash across all output bits.
        /// </summary>
        /// <param name="input">The intermediate hash value.</param>
        /// <returns>The avalanched hash value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong avalanche(ulong input)
        {
            input ^= input >> 33;
            input = input.wrapping_mul(PRIME_2);
            input ^= input >> 29;
            input = input.wrapping_mul(PRIME_3);
            input ^= input >> 32;
            return input;
        }
    }
}