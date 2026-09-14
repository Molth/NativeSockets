using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     A dummy structure used for WSAStartup data.
    ///     The actual content is not required for this implementation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 408)]
    internal struct WSAData
    {
        /// <summary>
        ///     Alignment padding to ensure the structure is properly aligned in memory.
        /// </summary>
        private nint __ss_align;
    }
}