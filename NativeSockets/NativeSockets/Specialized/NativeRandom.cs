using System.Runtime.CompilerServices;
using System.Security.Cryptography;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents a pseudo-random number generator, which is an algorithm that produces a sequence of numbers
    ///     that meet certain statistical requirements for randomness.
    /// </summary>
    internal static class NativeRandom
    {
        /// <summary>
        ///     Generates a random value of blittable type.
        /// </summary>
        /// <typeparam name="T">The blittable type.</typeparam>
        /// <returns>The randomly generated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Next<T>() where T : unmanaged
        {
            Unsafe.SkipInit(out T result);
            RandomNumberGenerator.Fill(MemoryMarshalHelpers.AsBytes(ref result));
            return result;
        }
    }
}