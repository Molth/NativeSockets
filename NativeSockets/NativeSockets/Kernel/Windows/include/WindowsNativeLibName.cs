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

        /// <summary>
        ///     IOC_IN flag indicating that the control code transfers data into the driver.
        /// </summary>
        private const uint IOC_IN = 0x80000000;

        /// <summary>
        ///     IOC_VENDOR flag indicating a vendor‑defined control code.
        /// </summary>
        private const uint IOC_VENDOR = 0x18000000;

        /// <summary>
        ///     Winsock IOCTL code used with <c>WSAIoctl</c> to control whether a UDP socket returns
        ///     <c>WSAECONNRESET</c> when an ICMP port unreachable message is received.
        ///     By default, Windows UDP sockets report connection reset errors; this code is used to disable
        ///     that behavior by setting the associated Boolean option to <c>FALSE</c>.
        /// </summary>
        public const uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
    }
}