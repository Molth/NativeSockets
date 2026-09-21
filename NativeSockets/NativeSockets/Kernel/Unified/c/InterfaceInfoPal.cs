using System;
using System.Globalization;
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
        ///     Resolves a network interface name to its interface index, which is used as the Ipv6 scope id
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
                OperatingSystem.IsWindows();
#else
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
        }

        /// <summary>
        ///     Attempts to parse an Ipv6 scope id from the specified text,
        ///     which may be either a numeric value
        ///     or a network interface name.
        /// </summary>
        /// <param name="scopeIdText">The text to parse.</param>
        /// <param name="scopeId">
        ///     When this method returns, contains the parsed scope id on success;
        ///     otherwise, 0.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the scope id was successfully parsed;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryParseScopeId(ReadOnlySpan<char> scopeIdText, out uint scopeId)
        {
            if (uint.TryParse(scopeIdText, NumberStyles.None, CultureInfo.InvariantCulture, out scopeId))
                return true;

            uint interfaceIndex = InterfaceNameToIndex(scopeIdText);
            if (interfaceIndex == 0)
                return false;

            scopeId = interfaceIndex;
            return true;
        }

        /// <summary>
        ///     Resolves a network interface name to its interface index, which is used as the Ipv6 scope id
        ///     for link‑local and site‑local addresses.
        /// </summary>
        /// <param name="interfaceName">The name of the network interface.</param>
        /// <returns>The interface index on success; otherwise 0.</returns>
        private static uint InterfaceNameToIndex(ReadOnlySpan<char> interfaceName) => _InterfaceNameToIndex(interfaceName);
    }
}