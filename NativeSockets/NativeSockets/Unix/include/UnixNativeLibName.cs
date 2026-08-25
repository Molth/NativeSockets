using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides Unix-specific socket operations using libc functions.
    /// </summary>
    internal static class UnixNativeLibName
    {
        /// <summary>
        ///     The name of the native library containing the socket functions.
        /// </summary>
        public const string NATIVE_LIBRARY = "libc";

        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        public const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;
    }
}