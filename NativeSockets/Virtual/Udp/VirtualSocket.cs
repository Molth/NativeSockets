using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents a virtual socket that is backed by either
    ///     a <see cref="NativeSocket" /> or a <see cref="Socket" />.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 No manual <see cref="NativeSocketPal.Startup" /> or <see cref="NativeSocketPal.Cleanup" /> of the
    ///                 underlying socket subsystem is required;
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 When <see cref="NativeSocketPal.IsSupported" /> is <see langword="false" />,
    ///                 the socket falls back to <see cref="Socket" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct VirtualSocket : IIsCreated, IDisposable, IEquatable<VirtualSocket>, IComparable<VirtualSocket>
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
        public bool IsCreated => VirtualSocketPal.IsCreated(this);

        /// <summary>
        ///     Gets the native socket handle.
        /// </summary>
        internal nint Handle => _handle;

        /// <summary>
        ///     Gets the address family of the socket.
        /// </summary>
        public AddressFamily Family => _addressFamily;

        /// <summary>
        ///     Gets a value indicating whether the socket uses Ipv4.
        /// </summary>
        public bool IsIpv4 => Family == AddressFamily.InterNetwork;

        /// <summary>
        ///     Gets a value indicating whether the socket uses Ipv6.
        /// </summary>
        public bool IsIpv6 => Family == AddressFamily.InterNetworkV6;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualSocket" /> structure.
        /// </summary>
        /// <param name="handle">The socket handle.</param>
        /// <param name="addressFamily">The address family of the socket.</param>
        internal VirtualSocket(nint handle, AddressFamily addressFamily)
        {
            _handle = handle;
            _addressFamily = addressFamily;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualSocket" /> structure.
        /// </summary>
        /// <param name="socket">The managed socket.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal VirtualSocket(Socket socket) : this(socket.Handle, socket.AddressFamily)
        {
        }

        /// <summary>
        ///     Creates a virtual socket for the specified address family (Ipv4 or Ipv6).
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <param name="socket">When this method returns, contains the created virtual socket.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Create(bool ipv6, out VirtualSocket socket) => VirtualSocketPal.Create(ipv6, out socket);

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(VirtualSocket socket) => VirtualSocketPal.Close(socket);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing,
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Close(this);

        /// <summary>
        ///     Converts this virtual socket to a native socket.
        /// </summary>
        /// <returns>The native socket wrapping the same handle and address family.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal NativeSocket AsNativeSocket() => new(_handle, _addressFamily);

        /// <summary>
        ///     Tries to get the managed <see cref="Socket" /> backing this virtual socket.
        /// </summary>
        /// <param name="socket">When this method returns, contains the managed socket, or null if it could not be retrieved.</param>
        /// <returns><see cref="SocketError.Success" /> if the managed socket was retrieved; otherwise an error code.</returns>
        internal SocketError TryGetSocket(out Socket? socket)
        {
            if (_handle == 0)
            {
                socket = default;
                return SocketError.NotSocket;
            }

            GCHandle gcHandle = GCHandle.FromIntPtr(_handle);
            if (!gcHandle.IsAllocated)
            {
                socket = default;
                return SocketError.NotSocket;
            }

            try
            {
                socket = (Socket?)gcHandle.Target;
                return socket != null ? SocketError.Success : SocketError.NotSocket;
            }
            catch
            {
                socket = default;
                return SocketError.NotSocket;
            }
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public bool Equals(VirtualSocket other) => SpanHelpers.Equals(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates
        ///     whether the current instance precedes, follows, or occurs in the same position in the sort order as the other
        ///     object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     <list type="table">
        ///         <listheader>
        ///             <term> Value</term><description> Meaning</description>
        ///         </listheader>
        ///         <item>
        ///             <term> Less than zero</term>
        ///             <description> This instance precedes <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///         <item>
        ///             <term> Zero</term>
        ///             <description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description>
        ///         </item>
        ///         <item>
        ///             <term> Greater than zero</term>
        ///             <description> This instance follows <paramref name="other" /> in the sort order.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public int CompareTo(VirtualSocket other) => SpanHelpers.Compare(ref Unsafe.AsRef(in this), ref other);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) => obj is VirtualSocket other && other.Equals(this);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() => NativeHashCode.GetHashCode(this);

        /// <summary>
        ///     Indicates whether the current object is equal to another object.
        /// </summary>
        public static bool operator ==(VirtualSocket left, VirtualSocket right) => left.Equals(right);

        /// <summary>
        ///     Indicates whether the current object is not equal to another object.
        /// </summary>
        public static bool operator !=(VirtualSocket left, VirtualSocket right) => !left.Equals(right);
    }
}