using System;
using System.Runtime.CompilerServices;
using NativeSockets;
using rust;

// ReSharper disable ALL

namespace xxhash_rust
{
    internal static partial class XxHash32
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
        ///     Processes the remaining tail bytes after the main chunk loop and applies the avalanche.
        /// </summary>
        /// <param name="input">The running hash value.</param>
        /// <param name="data">The full input span.</param>
        /// <param name="cursor">The byte offset where the tail begins.</param>
        /// <returns>The finalized hash value.</returns>
        private static uint finalize(uint input, ReadOnlySpan<byte> data, int cursor)
        {
            int len = data.Length - cursor;

            while (len >= 4)
            {
                input = input.wrapping_add(
                    read_u32(data, cursor).wrapping_mul(PRIME_3)
                );
                cursor += sizeof(uint);
                len -= sizeof(uint);
                input = input.rotate_left(17).wrapping_mul(PRIME_4);
            }

            while (len > 0)
            {
                input = input.wrapping_add(((uint)data[cursor]).wrapping_mul(PRIME_5));
                cursor += sizeof(byte);
                len -= sizeof(byte);
                input = input.rotate_left(11).wrapping_mul(PRIME_1);
            }

            return avalanche(input);
        }

        /// <summary>
        ///     Computes the XXH32 hash of the given input.
        /// </summary>
        /// <param name="input">The data to hash.</param>
        /// <param name="seed">The seed value.</param>
        /// <returns>The 32-bit hash value.</returns>
        public static uint xxh32(ReadOnlySpan<byte> input, uint seed)
        {
            uint result = (uint)input.Length;
            int cursor = 0;

            if (input.Length >= CHUNK_SIZE)
            {
                uint v1 = seed.wrapping_add(PRIME_1).wrapping_add(PRIME_2);
                uint v2 = seed.wrapping_add(PRIME_2);
                uint v3 = seed;
                uint v4 = seed.wrapping_sub(PRIME_1);

                while (true)
                {
                    v1 = round(v1, read_u32(input, cursor));
                    cursor += sizeof(uint);
                    v2 = round(v2, read_u32(input, cursor));
                    cursor += sizeof(uint);
                    v3 = round(v3, read_u32(input, cursor));
                    cursor += sizeof(uint);
                    v4 = round(v4, read_u32(input, cursor));
                    cursor += sizeof(uint);

                    if (input.Length - cursor < CHUNK_SIZE)
                    {
                        break;
                    }
                }

                result = result.wrapping_add(
                    v1.rotate_left(1).wrapping_add(
                        v2.rotate_left(7).wrapping_add(
                            v3.rotate_left(12).wrapping_add(
                                v4.rotate_left(18)
                            )
                        )
                    )
                );
            }
            else
            {
                result = result.wrapping_add(seed.wrapping_add(PRIME_5));
            }

            return finalize(result, input, cursor);
        }
    }
}