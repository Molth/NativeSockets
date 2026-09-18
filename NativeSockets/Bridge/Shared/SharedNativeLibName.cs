using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides the native library name and calling convention for Shared.
    /// </summary>
    internal static class SharedNativeLibName
    {
        /// <summary>
        ///     The name of the native library containing the socket functions.
        /// </summary>
        public const string DLL_NAME_NATIVESOCKETPAL = "nativesocketpal";

        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        public const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;
    }
}