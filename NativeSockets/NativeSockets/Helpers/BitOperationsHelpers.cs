using System.Runtime.CompilerServices;
#if NET5_0_OR_GREATER
using System.Numerics;
#endif

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Utility methods for intrinsic bit-twiddling operations.
    ///     The methods use hardware intrinsics when available on the underlying platform,
    ///     otherwise they use optimized software fallbacks.
    /// </summary>
    internal static class BitOperationsHelpers
    {
        /// <summary>
        ///     Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(uint value)
        {
#if NET6_0_OR_GREATER
            return BitOperations.IsPow2(value);
#else
            return (value & (value - 1)) == 0 && value != 0;
#endif
        }

        /// <summary>
        ///     Rotates the specified value left by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">
        ///     The number of bits to rotate by.
        ///     Any value outside the range [0..63] is treated as congruent mod 64.
        /// </param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int offset)
        {
#if NET5_0_OR_GREATER
            return BitOperations.RotateLeft(value, offset);
#else
            return (value << offset) | (value >> (64 - offset));
#endif
        }

        /// <summary>
        ///     Rotates the specified value left by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">
        ///     The number of bits to rotate by.
        ///     Any value outside the range [0..31] is treated as congruent mod 32.
        /// </param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int offset)
        {
#if NET5_0_OR_GREATER
            return BitOperations.RotateLeft(value, offset);
#else
            return (value << offset) | (value >> (32 - offset));
#endif
        }
    }
}