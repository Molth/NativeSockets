using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides helper methods for validating arguments and
    ///     throwing standard exceptions with consistent messaging.
    /// </summary>
    internal static class ThrowHelpers
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException" /> if <paramref name="value" /> is greater than or equal
        ///     <paramref name="other" />.
        /// </summary>
        /// <param name="value">The argument to validate as less than <paramref name="other" />.</param>
        /// <param name="other">The value to compare with <paramref name="value" />.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value" /> corresponds.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, ExceptionArgument paramName) where T : unmanaged, IComparable<T>
        {
            if (value.CompareTo(other) >= 0)
                throw new ArgumentOutOfRangeException(GetArgumentName(paramName), value, SR.ArgumentOutOfRange_MustBeLess);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException" /> if <paramref name="value" /> is negative.
        /// </summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value" /> corresponds.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative<T>(T value, ExceptionArgument paramName) where T : unmanaged,
#if NET7_0_OR_GREATER
            ISignedNumber<T>
#else
            IComparable<T>
#endif
        {
#if NET7_0_OR_GREATER
            if (T.IsNegative(value))
#else
            if (value.CompareTo(default) < 0)
#endif
                throw new ArgumentOutOfRangeException(GetArgumentName(paramName), value, SR.ArgumentOutOfRange_MustBeNonNegative);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentException" /> if <paramref name="value" /> is not a power of two.
        /// </summary>
        /// <param name="value">The alignment value to validate.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value" /> corresponds.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfAlignmentNotBePow2(uint value, ExceptionArgument paramName)
        {
            if (!BitOperationsHelpers.IsPow2(value))
                throw new ArgumentException(SR.Argument_AlignmentMustBePow2, GetArgumentName(paramName));
        }

        /// <summary>
        ///     Throws a <see cref="InvalidOperationException" />.
        /// </summary>
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException() => throw new InvalidOperationException();

        /// <summary>
        ///     Throws a <see cref="NotSupportedException" />.
        /// </summary>
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNotSupportedException() => throw new NotSupportedException();

        /// <summary>
        ///     Returns the argument name string associated with the specified <see cref="ExceptionArgument" /> value.
        /// </summary>
        /// <param name="argument">The <see cref="ExceptionArgument" /> value to convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string? GetArgumentName(ExceptionArgument argument) => argument switch
        {
            ExceptionArgument.alignment => "alignment",
            ExceptionArgument.length => "length",
            ExceptionArgument.offset => "offset",
            _ => null
        };
    }
}