using System;
#if !NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides cross-platform helpers for resolving network interface names to interface indices.
    /// </summary>
    internal static unsafe class InterfaceInfoPal
    {
        /// <summary>
        ///     Resolves a network interface name to its interface index, which is used as the Ipv6 scope identifier
        ///     for link‑local and site‑local addresses.
        /// </summary>
        private static delegate* managed<ReadOnlySpan<char>, uint> _InterfaceNameToIndex;

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        static InterfaceInfoPal()
        {
            _InterfaceNameToIndex = IsWindows() ? &WindowsInterfaceInfoPal.InterfaceNameToIndex : &UnixInterfaceInfoPal.InterfaceNameToIndex;

            return;

            static bool IsWindows() =>
#if NET5_0_OR_GREATER
                OperatingSystem.IsWindows()
#else
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#endif
            ;
        }

        /// <summary>
        ///     Resolves a network interface name to its interface index, which is used as the Ipv6 scope identifier
        ///     for link‑local and site‑local addresses.
        /// </summary>
        /// <param name="interfaceName">The name of the network interface.</param>
        /// <returns>The interface index on success; otherwise 0.</returns>
        public static uint InterfaceNameToIndex(ReadOnlySpan<char> interfaceName) => _InterfaceNameToIndex(interfaceName);
    }
}