using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides the native library name and calling convention for Unix.
    /// </summary>
    internal static class UnixNativeLibName
    {
        /// <summary>
        ///     The name of the native library.
        /// </summary>
        public const string DLL_NAME_LIBC = "libc";

        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        public const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;
    }
}