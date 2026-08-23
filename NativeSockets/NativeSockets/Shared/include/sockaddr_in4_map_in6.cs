using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents an Ipv4‑mapped Ipv6 address structure (12 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 12)]
    internal readonly struct sockaddr_in4_map_in6 : IEquatable<sockaddr_in4_map_in6>
    {
        /// <summary>
        ///     Creates a new instance from the given byte span.
        /// </summary>
        /// <param name="sin6_addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
        /// <returns>A new <see cref="sockaddr_in4_map_in6" /> instance.</returns>
        public static sockaddr_in4_map_in6 Create(ReadOnlySpan<byte> sin6_addr)
        {
            sockaddr_in4_map_in6 value = new sockaddr_in4_map_in6();
            sin6_addr.CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.As<sockaddr_in4_map_in6, byte>(ref value), Unsafe.SizeOf<sockaddr_in4_map_in6>()));
            return value;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public readonly bool Equals(sockaddr_in4_map_in6 other) => SpanHelpers.Equals(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public readonly override bool Equals(object? obj) => obj is sockaddr_in4_map_in6 other && other.Equals(this);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        public readonly override int GetHashCode() => NativeHashCode.GetHashCode(this);
    }
}