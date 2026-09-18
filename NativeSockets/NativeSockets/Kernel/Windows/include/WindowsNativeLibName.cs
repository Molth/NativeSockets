using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides the native library calling convention for Windows.
    /// </summary>
    internal static class WindowsNativeLibName
    {
#if !NATIVE_SOCKETS_USE_BRIDGE
        /// <summary>
        ///     The name of the native library containing the socket functions.
        /// </summary>
        public const string DLL_NAME_WS2_32 = "ws2_32.dll";
#endif

        /// <summary>
        ///     The name of the native library containing the IP Helper API functions.
        /// </summary>
        public const string DLL_NAME_IPHLPAPI = "iphlpapi.dll";

        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        public const CallingConvention CALLING_CONVENTION = CallingConvention.StdCall;
    }
}