using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Combines the hash code for multiple values into a single hash code.
    /// </summary>
    internal static unsafe class NativeHashCode
    {
        /// <summary>
        ///     Default seed value used for hash code calculation.
        /// </summary>
        private static readonly ulong DefaultSeed;

        /// <summary>
        ///     Performs initialization of the object.
        /// </summary>
        static NativeHashCode()
        {
            Unsafe.SkipInit(out ulong result);
            RandomNumberGenerator.Fill(MemoryMarshalHelpers.AsBytes(ref result));
            DefaultSeed = result;
        }

        /// <summary>
        ///     Diffuses the hash code returned by the specified bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T>(in T obj) where T : unmanaged => GetHashCode(MemoryMarshalHelpers.AsReadOnlyBytes(ref Unsafe.AsRef(in obj)));

        /// <summary>
        ///     Diffuses the hash code returned by the specified bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHashCode(ReadOnlySpan<byte> buffer) => Environment.Is64BitProcess ? XxHash64.HashToUInt64(buffer, DefaultSeed).GetHashCode() : (int)XxHash32.HashToUInt32(buffer, (uint)DefaultSeed.GetHashCode());
    }
}