using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents a native socket handle with its associated address family.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct NativeSocket : IIsCreated, IDisposable, IEquatable<NativeSocket>
    {
        /// <summary>
        ///     The native socket handle.
        /// </summary>
        private readonly nint _handle;

        /// <summary>
        ///     The address family of the socket.
        /// </summary>
        private readonly AddressFamily _addressFamily;

        /// <summary>
        ///     Gets a value that indicates whether this has been allocated or initialized.
        /// </summary>
        public bool IsCreated => Handle != 0;

        /// <summary>
        ///     Gets the native socket handle.
        /// </summary>
        public nint Handle => _handle;

        /// <summary>
        ///     Gets the address family of the socket.
        /// </summary>
        public AddressFamily Family => _addressFamily;

        /// <summary>
        ///     Gets a value indicating whether the socket uses Ipv4.
        /// </summary>
        public bool IsIpv4 => _addressFamily == AddressFamily.InterNetwork;

        /// <summary>
        ///     Gets a value indicating whether the socket uses Ipv6.
        /// </summary>
        public bool IsIpv6 => _addressFamily == AddressFamily.InterNetworkV6;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NativeSocket" /> structure.
        /// </summary>
        /// <param name="handle">The native socket handle.</param>
        /// <param name="addressFamily">The address family of the socket.</param>
        public NativeSocket(nint handle, AddressFamily addressFamily)
        {
            _handle = handle;
            _addressFamily = addressFamily;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NativeSocket" /> structure.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSocket(Socket socket) : this(socket.Handle, socket.AddressFamily)
        {
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing,
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => NativeSocketPal.Close(this);

        /// <summary>
        ///     Implicitly converts a <see cref="NativeSocket" /> to its native handle.
        /// </summary>
        /// <param name="socket">The socket to convert.</param>
        /// <returns>The native socket handle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint(NativeSocket socket) => socket.Handle;

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public bool Equals(NativeSocket other) => SpanHelpers.Equals(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) => obj is NativeSocket other && other.Equals(this);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => NativeHashCode.GetHashCode(this);

        /// <summary>
        ///     Returns information about the socket.
        /// </summary>
        /// <returns>A string that contains information about this.</returns>
        public override string ToString() => $"NativeSocket[{Handle}]";

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public static bool operator ==(NativeSocket left, NativeSocket right) => left.Equals(right);

        /// <summary>
        ///     Indicates whether the current object is not equal to another object.
        /// </summary>
        public static bool operator !=(NativeSocket left, NativeSocket right) => !left.Equals(right);
    }
}