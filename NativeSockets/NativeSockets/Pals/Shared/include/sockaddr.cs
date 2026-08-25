using System.Runtime.InteropServices;

#pragma warning disable CS8981

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a generic socket address structure used as a base
    ///     for address family-specific structures.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal unsafe struct sockaddr
    {
        /// <summary>
        ///     The address family (e.g., AF_INET, AF_INET6).
        /// </summary>
        [FieldOffset(0)] public ushort sa_family;

        /// <summary>
        ///     The address data (14 bytes). Actual interpretation depends on the address family.
        /// </summary>
        [FieldOffset(2)] public fixed byte sa_data[14];
    }
}