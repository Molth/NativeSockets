using System.Runtime.CompilerServices;
using rust;

// ReSharper disable ALL

namespace xxhash_rust
{
    /// <remarks>https://github.com/DoumanAsh/xxhash-rust</remarks>
    internal static partial class XxHash32
    {
        /// <summary>
        ///     The number of bytes processed per chunk in the main loop (4 × <see cref="uint" />).
        /// </summary>
        private const int CHUNK_SIZE = sizeof(uint) * 4;

        /// <summary>
        ///     XXH32 prime 1.
        /// </summary>
        private const uint PRIME_1 = 0x9E3779B1;

        /// <summary>
        ///     XXH32 prime 2.
        /// </summary>
        private const uint PRIME_2 = 0x85EBCA77;

        /// <summary>
        ///     XXH32 prime 3.
        /// </summary>
        private const uint PRIME_3 = 0xC2B2AE3D;

        /// <summary>
        ///     XXH32 prime 4.
        /// </summary>
        private const uint PRIME_4 = 0x27D4EB2F;

        /// <summary>
        ///     XXH32 prime 5.
        /// </summary>
        private const uint PRIME_5 = 0x165667B1;

        /// <summary>
        ///     Accumulates a single lane of the hash for a 32-bit chunk.
        /// </summary>
        /// <param name="acc">The accumulator for this lane.</param>
        /// <param name="input">The 32-bit input value.</param>
        /// <returns>The updated accumulator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint round(uint acc, uint input) =>
            acc.wrapping_add(input.wrapping_mul(PRIME_2))
                .rotate_left(13)
                .wrapping_mul(PRIME_1);

        /// <summary>
        ///     Final mixing step that spreads the bits of the hash across all output bits.
        /// </summary>
        /// <param name="input">The intermediate hash value.</param>
        /// <returns>The avalanched hash value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint avalanche(uint input)
        {
            input ^= input >> 15;
            input = input.wrapping_mul(PRIME_2);
            input ^= input >> 13;
            input = input.wrapping_mul(PRIME_3);
            input ^= input >> 16;
            return input;
        }
    }
}