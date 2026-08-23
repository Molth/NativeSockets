using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     This class contains methods that are mainly used to manage native memory.
    /// </summary>
    internal static unsafe class NativeMemoryAllocator
    {
        /// <summary>
        ///     Allocates an aligned block of memory of the specified size and alignment, in bytes.
        /// </summary>
        /// <param name="elementCount">The count, in elements, of the block to allocate.</param>
        /// <returns>A pointer to the allocated aligned block of memory.</returns>
        /// <exception cref="OutOfMemoryException">Allocating memory failed.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* AlignedAlloc<T>(uint elementCount) where T : unmanaged
        {
            uint byteCount = checked(elementCount * (uint)Unsafe.SizeOf<T>());
            uint alignment = AlignOf<T>();
            return (T*)AlignedAlloc(byteCount, alignment);
        }

        /// <summary>
        ///     Allocates and zeroes an aligned block of memory of the specified size and alignment, in bytes.
        /// </summary>
        /// <param name="elementCount">The count, in elements, of the block to allocate.</param>
        /// <returns>A pointer to the allocated and zeroed aligned block of memory.</returns>
        /// <exception cref="OutOfMemoryException">Allocating memory failed.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* AlignedAllocZeroed<T>(uint elementCount) where T : unmanaged
        {
            uint byteCount = checked(elementCount * (uint)Unsafe.SizeOf<T>());
            uint alignment = AlignOf<T>();
            return (T*)AlignedAllocZeroed(byteCount, alignment);
        }

        /// <summary>
        ///     Allocates an aligned block of memory of the specified size and alignment, in bytes.
        /// </summary>
        /// <param name="byteCount">The size, in bytes, of the block to allocate.</param>
        /// <param name="alignment">The alignment, in bytes, of the block to allocate. This must be a power of <c>2</c>.</param>
        /// <returns>A pointer to the allocated aligned block of memory.</returns>
        /// <exception cref="ArgumentException"><paramref name="alignment" /> is not a power of <c>2</c>.</exception>
        /// <exception cref="OutOfMemoryException">Allocating memory failed.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AlignedAlloc(uint byteCount, uint alignment)
        {
#if NET6_0_OR_GREATER
            return NativeMemory.AlignedAlloc(byteCount, alignment);
#else
            ThrowHelpers.ThrowIfAlignmentNotBePow2(alignment, ExceptionArgument.alignment);
            uint byteOffset = alignment - 1 + (uint)Unsafe.SizeOf<nint>();
            void* ptr = (void*)Marshal.AllocHGlobal((nint)(byteCount + byteOffset));
            void* result = (void*)(((nint)ptr + (nint)byteOffset) & ~((nint)alignment - 1));
            Unsafe.Subtract(ref Unsafe.AsRef<nint>(result), 1) = (nint)ptr;
            return result;
#endif
        }

        /// <summary>
        ///     Allocates and zeroes an aligned block of memory of the specified size and alignment, in bytes.
        /// </summary>
        /// <param name="byteCount">The size, in bytes, of the block to allocate.</param>
        /// <param name="alignment">The alignment, in bytes, of the block to allocate. This must be a power of <c>2</c>.</param>
        /// <returns>A pointer to the allocated and zeroed aligned block of memory.</returns>
        /// <exception cref="ArgumentException"><paramref name="alignment" /> is not a power of <c>2</c>.</exception>
        /// <exception cref="OutOfMemoryException">Allocating memory failed.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AlignedAllocZeroed(uint byteCount, uint alignment)
        {
#if NET6_0_OR_GREATER
            void* ptr = NativeMemory.AlignedAlloc(byteCount, alignment);
            SpanHelpers.Set(ref Unsafe.AsRef<byte>(ptr), 0, byteCount);
            return ptr;
#else
            ThrowHelpers.ThrowIfAlignmentNotBePow2(alignment, ExceptionArgument.alignment);
            uint byteOffset = alignment - 1 + (uint)Unsafe.SizeOf<nint>();
            void* ptr = (void*)Marshal.AllocHGlobal((nint)(byteCount + byteOffset));
            void* result = (void*)(((nint)ptr + (nint)byteOffset) & ~((nint)alignment - 1));
            Unsafe.Subtract(ref Unsafe.AsRef<nint>(result), 1) = (nint)ptr;
            SpanHelpers.Set(ref Unsafe.AsRef<byte>(result), 0, byteCount);
            return result;
#endif
        }

        /// <summary>
        ///     Frees an aligned block of memory.
        /// </summary>
        /// <param name="ptr">A pointer to the aligned block of memory that should be freed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AlignedFree(void* ptr)
        {
#if NET6_0_OR_GREATER
            NativeMemory.AlignedFree(ptr);
#else
            if (ptr == null)
                return;
            Marshal.FreeHGlobal(Unsafe.Subtract(ref Unsafe.AsRef<nint>(ptr), 1));
#endif
        }

        /// <summary>
        ///     Gets the alignment, in bytes, of the specified unmanaged type.
        /// </summary>
        /// <typeparam name="T">The unmanaged type whose alignment is to be determined.</typeparam>
        /// <returns>The alignment, in bytes, of type <typeparamref name="T" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AlignOf<T>() where T : unmanaged => (uint)(Unsafe.SizeOf<AlignOfHelper<T>>() - Unsafe.SizeOf<T>());

        /// <summary>
        ///     Helper structure for calculating type alignment.
        /// </summary>
        /// <typeparam name="T">The unmanaged type being measured.</typeparam>
        [StructLayout(LayoutKind.Sequential)]
        private readonly struct AlignOfHelper<T> where T : unmanaged
        {
            /// <summary>
            ///     Padding byte used for alignment calculation.
            /// </summary>
            private readonly byte _dummy;

            /// <summary>
            ///     The typed data used for alignment measurement.
            /// </summary>
            private readonly T _data;
        }
    }
}