using System.Runtime.CompilerServices;
using NativeSockets;

// ReSharper disable ALL

namespace rust
{
    /// <summary>
    ///     Provides Rust-style wrapping arithmetic extensions for <see cref="uint" />.
    /// </summary>
    internal static class U32Extensions
    {
        /// <summary>
        ///     Adds two <see cref="uint" /> values, wrapping on overflow.
        /// </summary>
        /// <param name="value">The first value.</param>
        /// <param name="other">The second value.</param>
        /// <returns>The wrapped sum.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint wrapping_add(this uint value, uint other) => unchecked(value + other);

        /// <summary>
        ///     Subtracts two <see cref="uint" /> values, wrapping on underflow.
        /// </summary>
        /// <param name="value">The minuend.</param>
        /// <param name="other">The subtrahend.</param>
        /// <returns>The wrapped difference.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint wrapping_sub(this uint value, uint other) => unchecked(value - other);

        /// <summary>
        ///     Multiplies two <see cref="uint" /> values, wrapping on overflow.
        /// </summary>
        /// <param name="value">The first value.</param>
        /// <param name="other">The second value.</param>
        /// <returns>The wrapped product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint wrapping_mul(this uint value, uint other) => unchecked(value * other);

        /// <summary>
        ///     Rotates the bits of a <see cref="uint" /> value left by the specified offset.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint rotate_left(this uint value, int offset) => BitOperationsHelpers.RotateLeft(value, offset);
    }
}