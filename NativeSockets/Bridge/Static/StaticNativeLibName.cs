using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides the native library name and calling convention for Static.
    /// </summary>
    internal static class StaticNativeLibName
    {
        /// <summary>
        ///     The name of the native library.
        /// </summary>
        public const string DLL_NAME_INTERNAL = "__Internal";

        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        public const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;
    }
}