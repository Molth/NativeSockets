using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents a contiguous region of arbitrary native memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct NativeIoSlice : IIsCreated, IDisposable
    {
        /// <summary>
        ///     Represents a contiguous region of arbitrary memory.
        /// </summary>
        private readonly void* _buffer;

        /// <summary>
        ///     Gets the total numbers of elements the internal data structure can hold.
        /// </summary>
        private readonly int _length;

        /// <summary>
        ///     Initializes a new instance of this class
        ///     with the specified number of elements,
        ///     using the natural alignment of byte
        ///     and without zero-initializing the allocated memory.
        /// </summary>
        /// <param name="length">The number of elements to allocate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length" /> is negative.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeIoSlice(int length)
        {
            ThrowHelpers.ThrowIfNegative(length, ExceptionArgument.length);
            _buffer = NativeMemoryAllocator.AlignedAlloc<byte>((uint)length);
            _length = length;
        }

        /// <summary>
        ///     Initializes a new instance of this class
        ///     with the specified number of elements,
        ///     using the natural alignment of byte and optionally zero-initializing the memory.
        /// </summary>
        /// <param name="length">The number of elements to allocate.</param>
        /// <param name="zeroed">
        ///     <see langword="true" /> to zero-initialize the allocated memory;
        ///     otherwise, the memory content is undefined.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length" /> is negative.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeIoSlice(int length, bool zeroed)
        {
            ThrowHelpers.ThrowIfNegative(length, ExceptionArgument.length);
            _buffer = zeroed ? NativeMemoryAllocator.AlignedAllocZeroed<byte>((uint)length) : NativeMemoryAllocator.AlignedAlloc<byte>((uint)length);
            _length = length;
        }

        /// <summary>
        ///     Initializes a new instance of this class
        ///     that wraps an existing native memory buffer.
        /// </summary>
        /// <param name="buffer">A pointer to the existing native memory buffer.</param>
        /// <param name="length">The number of elements in the buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length" /> is negative.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeIoSlice(void* buffer, int length)
        {
            ThrowHelpers.ThrowIfNegative(length, ExceptionArgument.length);
            _buffer = buffer;
            _length = length;
        }

        /// <summary>
        ///     Gets a value that indicates whether this has been allocated or initialized.
        /// </summary>
        public bool IsCreated => _buffer != null;

        /// <summary>
        ///     Gets a value that indicates whether this is empty.
        /// </summary>
        public bool IsEmpty => _length == 0;

        /// <summary>
        ///     Represents a contiguous region of arbitrary memory.
        /// </summary>
        public void* Buffer => _buffer;

        /// <summary>
        ///     Gets the total number of elements in all the dimensions of the instance.
        /// </summary>
        public int Length => _length;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing,
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => NativeMemoryAllocator.AlignedFree(_buffer);

        /// <summary>
        ///     Creates a new span over a portion of a regular managed object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.AsRef<byte>(_buffer), _length);

        /// <summary>
        ///     Creates a new read-only span over a portion of a regular managed object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>(_buffer), _length);
    }
}