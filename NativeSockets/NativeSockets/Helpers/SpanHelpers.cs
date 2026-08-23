using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides low-level memory manipulation utilities for spans.
    /// </summary>
    internal static unsafe class SpanHelpers
    {
        /// <summary>
        ///     Copies bytes from the source address to the destination address
        ///     without assuming architecture dependent alignment of the addresses.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(void* destination, void* source, uint byteCount) => Copy(ref Unsafe.AsRef<byte>(destination), ref Unsafe.AsRef<byte>(source), byteCount);

        /// <summary>
        ///     Copies bytes from the source address to the destination address
        ///     without assuming architecture dependent alignment of the addresses.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(ref byte destination, ref byte source, uint byteCount) => Unsafe.CopyBlockUnaligned(ref destination, ref source, byteCount);

        /// <summary>
        ///     Initializes a block of memory at the given location with a given initial value
        ///     without assuming architecture dependent alignment of the address.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref byte startAddress, byte value, uint byteCount) => Unsafe.InitBlockUnaligned(ref startAddress, value, byteCount);

        /// <summary>
        ///     Determines whether two sequences are equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ref byte left, ref byte right, uint byteCount)
        {
            for (uint count; byteCount > 0; byteCount -= count, left = ref Unsafe.AddByteOffset(ref left, (nint)count), right = ref Unsafe.AddByteOffset(ref right, (nint)count))
            {
                count = byteCount > int.MaxValue ? int.MaxValue : byteCount;
                if (!MemoryMarshal.CreateReadOnlySpan(ref left, (int)count).SequenceEqual(MemoryMarshal.CreateReadOnlySpan(ref right, (int)count)))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Determines the relative order of the sequences.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(ref byte left, ref byte right, uint byteCount)
        {
            int comparison = 0;
            for (uint count; byteCount > 0 && comparison == 0; byteCount -= count, left = ref Unsafe.AddByteOffset(ref left, (nint)count), right = ref Unsafe.AddByteOffset(ref right, (nint)count))
            {
                count = byteCount > int.MaxValue ? int.MaxValue : byteCount;
                comparison = MemoryMarshal.CreateReadOnlySpan(ref left, (int)count).SequenceCompareTo(MemoryMarshal.CreateReadOnlySpan(ref right, (int)count));
            }

            return comparison;
        }

        /// <summary>
        ///     Determines whether two values are equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<T>(ref T left, ref T right) where T : unmanaged => Equals(ref Unsafe.As<T, byte>(ref left), ref Unsafe.As<T, byte>(ref right), (uint)Unsafe.SizeOf<T>());

        /// <summary>
        ///     Determines the relative order of the values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare<T>(ref T left, ref T right) where T : unmanaged => Compare(ref Unsafe.As<T, byte>(ref left), ref Unsafe.As<T, byte>(ref right), (uint)Unsafe.SizeOf<T>());
    }
}