using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides the native library calling convention for Windows.
    /// </summary>
    internal static class WindowsNativeLibName
    {
        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        public const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;
    }
}