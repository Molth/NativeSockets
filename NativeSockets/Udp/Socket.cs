using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS1591
#pragma warning disable CS8632

// ReSharper disable ALL

namespace NativeSockets.Udp
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Socket : IDisposable, IEquatable<Socket>
    {
        [FieldOffset(0)] public nint Handle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Socket Create()
        {
            UdpPal.Initialize();
            Socket socket = UdpPal.Create();
            if (socket == -1)
                UdpPal.Cleanup();
            return socket;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            UdpPal.Close(ref this);
            UdpPal.Cleanup();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Bind(in SocketAddress address) => UdpPal.Bind(this, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Connect(in SocketAddress address) => UdpPal.Connect(this, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetOption(SocketOptionLevel level, SocketOptionName name, ref int value) => UdpPal.SetOption(this, level, name, ref value) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetOption(SocketOptionLevel level, SocketOptionName name, ref int value) => UdpPal.GetOption(this, level, name, ref value) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetNonBlocking(bool nonBlocking) => UdpPal.SetNonBlocking(this, nonBlocking) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Poll(int microseconds, SelectMode mode) => UdpPal.Poll(this, microseconds, mode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReceiveFrom(Span<byte> buffer, ref SocketAddress address) => UdpPal.ReceiveFrom(this, ref MemoryMarshal.GetReference(buffer), buffer.Length, ref address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Send(ReadOnlySpan<byte> buffer, in SocketAddress address) => UdpPal.SendTo(this, ref MemoryMarshal.GetReference(buffer), buffer.Length, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetAddress(ref SocketAddress address) => UdpPal.GetAddress(this, ref address) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetSendBufferSize(out int sendBufferSize)
        {
            sendBufferSize = 0;
            return UdpPal.GetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.SendBuffer, ref sendBufferSize) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetSendBufferSize(int sendBufferSize) => UdpPal.SetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.SendBuffer, ref sendBufferSize) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetReceiveBufferSize(out int receiveBufferSize)
        {
            receiveBufferSize = 0;
            return UdpPal.GetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ref receiveBufferSize) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetReceiveBufferSize(int receiveBufferSize) => UdpPal.SetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ref receiveBufferSize) == 0;

        public bool Equals(Socket other) => Handle == other.Handle;
        public override bool Equals(object? obj) => obj is Socket socket && Equals(socket);
        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => $"Socket[{Handle}]";

        public static bool operator ==(Socket left, Socket right) => left.Equals(right);
        public static bool operator !=(Socket left, Socket right) => !(left == right);

        public static implicit operator bool(Socket socket) => socket.Handle > 0;
        public static implicit operator nint(Socket socket) => socket.Handle;
        public static implicit operator Socket(nint handle) => Unsafe.As<nint, Socket>(ref handle);
    }
}