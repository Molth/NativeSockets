using System;
using System.Runtime.InteropServices;
using static NativeSockets.WindowsNativeLibName;

#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time.

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides Windows-specific helpers for resolving network interface names to interface indices.
    /// </summary>
    internal static unsafe class WindowsInterfaceInfoPal
    {
        /// <summary>
        ///     Converts a network interface name (such as "Ethernet" or "Wi-Fi") into its locally unique id (LUID).
        /// </summary>
        /// <param name="__utf16_interfaceName_native">Pointer to a null-terminated UTF‑16 interface name string.</param>
        /// <param name="__interfaceLuid_native">Pointer to a buffer that receives the interface LUID.</param>
        /// <returns>0 on success; a non-zero Windows error code on failure.</returns>
        [DllImport(DLL_NAME_IPHLPAPI, EntryPoint = "ConvertInterfaceNameToLuidW", CallingConvention = CALLING_CONVENTION)]
        private static extern uint _ConvertInterfaceNameToLuidW(char* __utf16_interfaceName_native, ulong* __interfaceLuid_native);

        /// <summary>
        ///     Converts a network interface LUID into its interface index.
        /// </summary>
        /// <param name="__interfaceLuid_native">Pointer to the interface LUID.</param>
        /// <param name="__ifIndex_native">Pointer to a buffer that receives the interface index.</param>
        /// <returns>0 on success; a non-zero Windows error code on failure.</returns>
        [DllImport(DLL_NAME_IPHLPAPI, EntryPoint = "ConvertInterfaceLuidToIndex", CallingConvention = CALLING_CONVENTION)]
        private static extern uint _ConvertInterfaceLuidToIndex(ulong* __interfaceLuid_native, uint* __ifIndex_native);

        /// <summary>
        ///     Resolves a network interface name to its interface index, which is used as the Ipv6 scope id
        ///     for link‑local and site‑local addresses.
        /// </summary>
        /// <param name="interfaceName">The name of the network interface.</param>
        /// <returns>The interface index on success; otherwise 0.</returns>
        public static uint InterfaceNameToIndex(ReadOnlySpan<char> interfaceName)
        {
            int charCount = interfaceName.Length + 1;

            ulong __interfaceLuid_native = 0;

            using (NativeScopedArray<char> __buffers_native = new NativeScopedArray<char>(stackalloc char[256], charCount))
            {
                Span<char> buffer = __buffers_native.AsSpan();
                interfaceName.CopyTo(buffer);
                buffer[^1] = '\0';

                if (_ConvertInterfaceNameToLuidW(__buffers_native.Buffer, &__interfaceLuid_native) != 0)
                    return 0;
            }

            uint __interfaceIndex_native = 0;

            return _ConvertInterfaceLuidToIndex(&__interfaceLuid_native, &__interfaceIndex_native) == 0 ? __interfaceIndex_native : 0;
        }
    }
}