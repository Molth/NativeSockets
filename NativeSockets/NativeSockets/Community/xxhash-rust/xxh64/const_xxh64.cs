using System;
using System.Runtime.CompilerServices;
using NativeSockets;
using rust;

// ReSharper disable ALL

namespace xxhash_rust
{
    internal static partial class XxHash64
    {
        /// <summary>
        ///     Reads a 32-bit little-endian value from the input span at the given cursor.
        /// </summary>
        /// <param name="input">The input span.</param>
        /// <param name="cursor">The byte offset within the span.</param>
        /// <returns>The 32-bit value read from the span.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint read_u32(ReadOnlySpan<byte> input, int cursor) => BinaryPrimitivesHelpers.ReadUInt32LittleEndian(input.Slice(cursor));

        /// <summary>
        ///     Reads a 64-bit little-endian value from the input span at the given cursor.
        /// </summary>
        /// <param name="input">The input span.</param>
        /// <param name="cursor">The byte offset within the span.</param>
        /// <returns>The 64-bit value read from the span.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong read_u64(ReadOnlySpan<byte> input, int cursor) => BinaryPrimitivesHelpers.ReadUInt64LittleEndian(input.Slice(cursor));

        /// <summary>
        ///     Processes the remaining tail bytes after the main chunk loop and applies the avalanche.
        /// </summary>
        /// <param name="input">The running hash value.</param>
        /// <param name="data">The full input span.</param>
        /// <param name="cursor">The byte offset where the tail begins.</param>
        /// <returns>The finalized hash value.</returns>
        private static ulong finalize(ulong input, ReadOnlySpan<byte> data, int cursor)
        {
            int len = data.Length - cursor;

            while (len >= 8)
            {
                input ^= round(0, read_u64(data, cursor));
                cursor += sizeof(ulong);
                len -= sizeof(ulong);
                input = input.rotate_left(27).wrapping_mul(PRIME_1).wrapping_add(PRIME_4);
            }

            if (len >= 4)
            {
                input ^= ((ulong)read_u32(data, cursor)).wrapping_mul(PRIME_1);
                cursor += sizeof(uint);
                len -= sizeof(uint);
                input = input.rotate_left(23).wrapping_mul(PRIME_2).wrapping_add(PRIME_3);
            }

            while (len > 0)
            {
                input ^= ((ulong)data[cursor]).wrapping_mul(PRIME_5);
                cursor += sizeof(byte);
                len -= sizeof(byte);
                input = input.rotate_left(11).wrapping_mul(PRIME_1);
            }

            return avalanche(input);
        }

        /// <summary>
        ///     Computes the XXH64 hash of the given input.
        /// </summary>
        /// <param name="input">The data to hash.</param>
        /// <param name="seed">The seed value.</param>
        /// <returns>The 64-bit hash value.</returns>
        public static ulong xxh64(ReadOnlySpan<byte> input, ulong seed)
        {
            ulong input_len = (ulong)input.Length;
            int cursor = 0;
            ulong result;

            if (input.Length >= CHUNK_SIZE)
            {
                ulong v1 = seed.wrapping_add(PRIME_1).wrapping_add(PRIME_2);
                ulong v2 = seed.wrapping_add(PRIME_2);
                ulong v3 = seed;
                ulong v4 = seed.wrapping_sub(PRIME_1);

                while (true)
                {
                    v1 = round(v1, read_u64(input, cursor));
                    cursor += sizeof(ulong);
                    v2 = round(v2, read_u64(input, cursor));
                    cursor += sizeof(ulong);
                    v3 = round(v3, read_u64(input, cursor));
                    cursor += sizeof(ulong);
                    v4 = round(v4, read_u64(input, cursor));
                    cursor += sizeof(ulong);

                    if (input.Length - cursor < CHUNK_SIZE)
                    {
                        break;
                    }
                }

                result = v1.rotate_left(1).wrapping_add(v2.rotate_left(7))
                    .wrapping_add(v3.rotate_left(12))
                    .wrapping_add(v4.rotate_left(18));

                result = merge_round(result, v1);
                result = merge_round(result, v2);
                result = merge_round(result, v3);
                result = merge_round(result, v4);
            }
            else
            {
                result = seed.wrapping_add(PRIME_5);
            }

            result = result.wrapping_add(input_len);

            return finalize(result, input, cursor);
        }
    }
}