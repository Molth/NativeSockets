using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides a collection of methods for interoperating with
    ///     <see cref="Memory{T}" />,
    ///     <see cref="ReadOnlyMemory{T}" />,
    ///     <see cref="Span{T}" />,
    ///     <see cref="ReadOnlySpan{T}" />.
    /// </summary>
    internal static class MemoryMarshalHelpers
    {
        /// <summary>
        ///     Casts to a Span of byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> AsBytes<T>(ref T value) => MemoryMarshal.CreateSpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>());

        /// <summary>
        ///     Casts to a ReadOnlySpan of byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> AsReadOnlyBytes<T>(ref T value) where T : unmanaged => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>());
    }
}