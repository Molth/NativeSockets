using System;
using System.Runtime.InteropServices;
using System.Text;
using static NativeSockets.UnixNativeLibName;

#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time.

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides Unix-specific helpers for resolving network interface names to interface indices.
    /// </summary>
    internal static unsafe class UnixInterfaceInfoPal
    {
        /// <summary>
        ///     Returns the interface index of the network interface whose name is given by
        ///     <paramref name="__utf8_interfaceName_native" /> (see <c>if_nametoindex(3)</c>).
        ///     The interface index is commonly used as the Ipv6 scope identifier for link‑local addresses.
        /// </summary>
        /// <param name="__utf8_interfaceName_native">Pointer to a null-terminated UTF‑8 interface name string.</param>
        /// <returns>The interface index on success; 0 on failure, with the error code available via errno.</returns>
        [DllImport(DLL_NAME_LIBC, EntryPoint = "if_nametoindex", CallingConvention = CALLING_CONVENTION, SetLastError = true)]
        private static extern uint _if_nametoindex(byte* __utf8_interfaceName_native);

        /// <summary>
        ///     Resolves a network interface name to its interface index, which is used as the Ipv6 scope identifier
        ///     for link‑local and site‑local addresses.
        /// </summary>
        /// <param name="interfaceName">The name of the network interface.</param>
        /// <returns>The interface index on success; otherwise 0.</returns>
        public static uint InterfaceNameToIndex(ReadOnlySpan<char> interfaceName)
        {
            int byteCount = Encoding.UTF8.GetByteCount(interfaceName) + 1;

            uint __interfaceIndex_native;

            using (NativeScopedArray<byte> __buffers_native = new NativeScopedArray<byte>(stackalloc byte[512], byteCount))
            {
                Span<byte> buffer = __buffers_native.AsSpan();
                Encoding.UTF8.GetBytes(interfaceName, buffer);
                buffer[^1] = (byte)'\0';

                __interfaceIndex_native = _if_nametoindex(__buffers_native.Buffer);
            }

            return __interfaceIndex_native;
        }
    }
}