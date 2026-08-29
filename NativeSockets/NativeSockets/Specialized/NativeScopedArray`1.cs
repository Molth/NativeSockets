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
    internal readonly unsafe struct NativeScopedArray<T> : IIsCreated, IDisposable, IEquatable<NativeScopedArray<T>> where T : unmanaged
    {
        /// <summary>
        ///     Represents a contiguous region of arbitrary memory.
        /// </summary>
        private readonly T* _buffer;

        /// <summary>
        ///     Gets the total numbers of elements the internal data structure can hold.
        /// </summary>
        private readonly int _length;

        /// <summary>
        ///     Indicates whether the memory buffer was allocated by this instance.
        ///     <see langword="true" /> if the instance allocated its own memory;
        ///     <see langword="false" /> if the buffer was provided by the caller.
        /// </summary>
        private readonly bool _allocated;

        /// <summary>
        ///     Initializes a new instance of this class
        ///     with the specified number of elements,
        ///     using the natural alignment of <typeparamref name="T" />
        ///     and without zero-initializing the allocated memory.
        /// </summary>
        /// <param name="buffer">
        ///     An optional pre-allocated span that can be used as storage.
        ///     If the span is shorter than <paramref name="length" />, the instance will allocate its own memory.
        ///     If the span is long enough, it will be used directly without additional allocation.
        /// </param>
        /// <param name="length">The number of elements to allocate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length" /> is negative.</exception>
        [MustBePinned(nameof(buffer))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeScopedArray([MustBePinned] Span<T> buffer, int length)
        {
            ThrowHelpers.ThrowIfNegative(length, ExceptionArgument.length);
            bool shouldAllocate = buffer.Length < length;
            _buffer = !shouldAllocate ? (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(buffer)) : NativeMemoryAllocator.AlignedAlloc<T>((uint)length);
            _length = length;
            _allocated = shouldAllocate;
        }

        /// <summary>
        ///     Gets a value that indicates whether this has been allocated or initialized.
        /// </summary>
        public bool IsCreated => _buffer != null;

        /// <summary>
        ///     Represents a contiguous region of arbitrary memory.
        /// </summary>
        public T* Buffer => _buffer;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing,
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (!_allocated)
                return;
            NativeMemoryAllocator.AlignedFree(_buffer);
        }

        /// <summary>
        ///     Creates a new span over a portion of a regular managed object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(_buffer), _length);

        /// <summary>
        ///     Creates a new read-only span over a portion of a regular managed object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>(_buffer), _length);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public bool Equals(NativeScopedArray<T> other) => SpanHelpers.Equals(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) => obj is NativeScopedArray<T> other && other.Equals(this);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => NativeHashCode.GetHashCode(this);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public static bool operator ==(NativeScopedArray<T> left, NativeScopedArray<T> right) => left.Equals(right);

        /// <summary>
        ///     Indicates whether the current object is not equal to another object.
        /// </summary>
        public static bool operator !=(NativeScopedArray<T> left, NativeScopedArray<T> right) => !left.Equals(right);
    }
}