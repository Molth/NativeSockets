using System.Runtime.CompilerServices;
#if !NET7_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
#else
using System.Runtime.Intrinsics;
#endif

// ReSharper disable ALL

namespace NativeCollections
{
    /// <summary>
    ///     Span helpers
    /// </summary>
    internal static class SpanHelpers
    {
        /// <summary>
        ///     Determines whether two sequences are equal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(ref byte left, ref byte right, nuint byteCount)
        {
#if NET7_0_OR_GREATER
            if (byteCount >= (nuint)Unsafe.SizeOf<nuint>())
            {
                if (!Unsafe.AreSame(ref left, ref right))
                {
                    if (Vector128.IsHardwareAccelerated)
                    {
#if NET8_0_OR_GREATER
                        if (Vector512.IsHardwareAccelerated && byteCount >= (nuint)Vector512<byte>.Count)
                        {
                            nuint offset = 0;
                            nuint lengthToExamine = byteCount - (nuint)Vector512<byte>.Count;
                            if (lengthToExamine != 0)
                            {
                                do
                                {
                                    if (Vector512.LoadUnsafe(ref left, offset) != Vector512.LoadUnsafe(ref right, offset))
                                        return false;
                                    offset += (nuint)Vector512<byte>.Count;
                                } while (lengthToExamine > offset);
                            }

                            return Vector512.LoadUnsafe(ref left, lengthToExamine) == Vector512.LoadUnsafe(ref right, lengthToExamine);
                        }
#endif
                        if (Vector256.IsHardwareAccelerated && byteCount >= (nuint)Vector256<byte>.Count)
                        {
                            nuint offset = 0;
                            nuint lengthToExamine = byteCount - (nuint)Vector256<byte>.Count;
                            if (lengthToExamine != 0)
                            {
                                do
                                {
                                    if (Vector256.LoadUnsafe(ref left, offset) != Vector256.LoadUnsafe(ref right, offset))
                                        return false;
                                    offset += (nuint)Vector256<byte>.Count;
                                } while (lengthToExamine > offset);
                            }

                            return Vector256.LoadUnsafe(ref left, lengthToExamine) == Vector256.LoadUnsafe(ref right, lengthToExamine);
                        }

                        if (byteCount >= (nuint)Vector128<byte>.Count)
                        {
                            nuint offset = 0;
                            nuint lengthToExamine = byteCount - (nuint)Vector128<byte>.Count;
                            if (lengthToExamine != 0)
                            {
                                do
                                {
                                    if (Vector128.LoadUnsafe(ref left, offset) != Vector128.LoadUnsafe(ref right, offset))
                                        return false;
                                    offset += (nuint)Vector128<byte>.Count;
                                } while (lengthToExamine > offset);
                            }

                            return Vector128.LoadUnsafe(ref left, lengthToExamine) == Vector128.LoadUnsafe(ref right, lengthToExamine);
                        }
                    }

                    if (Unsafe.SizeOf<nint>() == 8 && Vector128.IsHardwareAccelerated)
                    {
                        nuint offset = byteCount - (nuint)Unsafe.SizeOf<nuint>();
                        nuint differentBits = Unsafe.ReadUnaligned<nuint>(ref left) - Unsafe.ReadUnaligned<nuint>(ref right);
                        differentBits |= Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref left, offset)) - Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref right, offset));
                        return differentBits == 0;
                    }
                    else
                    {
                        nuint offset = 0;
                        nuint lengthToExamine = byteCount - (nuint)Unsafe.SizeOf<nuint>();
                        if (lengthToExamine > 0)
                        {
                            do
                            {
                                if (Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref left, offset)) != Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref right, offset)))
                                    return false;
                                offset += (nuint)Unsafe.SizeOf<nuint>();
                            } while (lengthToExamine > offset);
                        }

                        return Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref left, lengthToExamine)) == Unsafe.ReadUnaligned<nuint>(ref Unsafe.AddByteOffset(ref right, lengthToExamine));
                    }
                }

                return true;
            }

            if (byteCount < sizeof(uint) || Unsafe.SizeOf<nint>() != 8)
            {
                uint differentBits = 0;
                nuint offset = byteCount & 2;
                if (offset != 0)
                {
                    differentBits = Unsafe.ReadUnaligned<ushort>(ref left);
                    differentBits -= Unsafe.ReadUnaligned<ushort>(ref right);
                }

                if ((byteCount & 1) != 0)
                    differentBits |= Unsafe.AddByteOffset(ref left, offset) - (uint)Unsafe.AddByteOffset(ref right, offset);
                return differentBits == 0;
            }
            else
            {
                nuint offset = byteCount - sizeof(uint);
                uint differentBits = Unsafe.ReadUnaligned<uint>(ref left) - Unsafe.ReadUnaligned<uint>(ref right);
                differentBits |= Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref left, offset)) - Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref right, offset));
                return differentBits == 0;
            }
#else
            nuint quotient = byteCount >> 30;
            nuint remainder = byteCount & 1073741823;
            for (nuint i = 0; i < quotient; ++i)
            {
                if (!MemoryMarshal.CreateReadOnlySpan(ref left, 1073741824).SequenceEqual(MemoryMarshal.CreateReadOnlySpan(ref right, 1073741824)))
                    return false;
                left = ref Unsafe.AddByteOffset(ref left, (nint)1073741824);
                right = ref Unsafe.AddByteOffset(ref right, (nint)1073741824);
            }

            return MemoryMarshal.CreateReadOnlySpan(ref left, (int)remainder).SequenceEqual(MemoryMarshal.CreateReadOnlySpan(ref right, (int)remainder));
#endif
        }
    }
}