using System.Runtime.CompilerServices;
#if !NATIVE_SOCKETS_USE_BRIDGE
using System.Runtime.InteropServices;
#endif

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides native library functions for Unix-based systems.
    /// </summary>
    internal static class UnixNativeLib
    {
#if !NATIVE_SOCKETS_USE_BRIDGE
        /// <summary>
        ///     Get the last platform invoke error on the current thread.
        /// </summary>
        /// <returns>The last platform invoke error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int _errno() => Marshal.GetLastWin32Error();
#endif

        /// <summary>
        ///     Converts a time duration in milliseconds to a <see cref="UnixTimeValue" /> structure.
        /// </summary>
        /// <param name="milliseconds">The duration in milliseconds.</param>
        /// <param name="socketTime">The <see cref="UnixTimeValue" /> structure to fill.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MillisecondsToTimeValue(int milliseconds, ref UnixTimeValue socketTime)
        {
            const int millicnv = 1000;
            int quotient = milliseconds / millicnv;
            int remainder = milliseconds - quotient * millicnv;
            socketTime.Seconds = quotient;
            socketTime.Microseconds = remainder * 1000;
        }
    }
}