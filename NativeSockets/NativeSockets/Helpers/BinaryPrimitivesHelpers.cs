using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Reads bytes as primitives with specific endianness.
    /// </summary>
    internal static class BinaryPrimitivesHelpers
    {
        /// <summary>
        ///     Write a <see cref="T:System.UInt16" /> into a span of bytes,
        ///     as big endian.
        /// </summary>
        /// <param name="destination">The span of bytes where the value is to be written.</param>
        /// <param name="value">The value to write into the span of bytes.</param>
        /// <remarks>Writes exactly 2 bytes to the beginning of the span.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16BigEndian(Span<byte> destination, ushort value)
        {
            if (BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        /// <summary>
        ///     Write a <see cref="T:System.UInt32" /> into a span of bytes,
        ///     as big endian.
        /// </summary>
        /// <param name="destination">The span of bytes where the value is to be written.</param>
        /// <param name="value">The value to write into the span of bytes.</param>
        /// <remarks>Writes exactly 4 bytes to the beginning of the span.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32BigEndian(Span<byte> destination, uint value)
        {
            if (BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        /// <summary>
        ///     Reads a <see cref="T:System.UInt16" /> from the given location,
        ///     as big endian.
        /// </summary>
        /// <param name="source">The read-only span to read.</param>
        /// <returns>The big endian value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> source)
        {
            ushort result = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(source));
            if (BitConverter.IsLittleEndian)
                result = BinaryPrimitives.ReverseEndianness(result);
            return result;
        }

        /// <summary>
        ///     Reads a <see cref="T:System.Int32" /> from the given location,
        ///     as big endian.
        /// </summary>
        /// <param name="source">The read-only span to read.</param>
        /// <returns>The big endian value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32BigEndian(ReadOnlySpan<byte> source)
        {
            int result = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(source));
            if (BitConverter.IsLittleEndian)
                result = BinaryPrimitives.ReverseEndianness(result);
            return result;
        }

        /// <summary>
        ///     Reads a <see cref="T:System.UInt32" /> from the given location,
        ///     as little endian.
        /// </summary>
        /// <param name="source">The read-only span to read.</param>
        /// <returns>The little endian value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32LittleEndian(ReadOnlySpan<byte> source)
        {
            uint result = Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(source));
            if (!BitConverter.IsLittleEndian)
                result = BinaryPrimitives.ReverseEndianness(result);
            return result;
        }

        /// <summary>
        ///     Reads a <see cref="T:System.UInt64" /> from the given location,
        ///     as little endian.
        /// </summary>
        /// <param name="source">The read-only span to read.</param>
        /// <returns>The little endian value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64LittleEndian(ReadOnlySpan<byte> source)
        {
            ulong result = Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(source));
            if (!BitConverter.IsLittleEndian)
                result = BinaryPrimitives.ReverseEndianness(result);
            return result;
        }
    }
}