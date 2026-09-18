using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides native library functions for Unix-based systems.
    /// </summary>
    internal static class UnixNativeLib
    {
        /// <summary>
        ///     Get the last platform invoke error on the current thread.
        /// </summary>
        /// <returns>The last platform invoke error.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int _errno()
        {
#if NET6_0_OR_GREATER
            return Marshal.GetLastPInvokeError();
#else
            return Marshal.GetLastWin32Error();
#endif
        }
    }
}