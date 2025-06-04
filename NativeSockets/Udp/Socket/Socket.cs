using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS1591
#pragma warning disable CS8632

// ReSharper disable ALL

namespace NativeSockets.Udp
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct Socket
    {
        [FieldOffset(0)] public int Handle;
        public bool IsIPv4 => !IsIPv6;
        [FieldOffset(4)] public bool IsIPv6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Socket Create(bool ipv6)
        {
            UdpPal.Initialize();
            Socket socket = UdpPal.Create(ipv6);
            if (socket.Handle == -1)
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
        public SocketError SetDualMode(bool dualMode) => UdpPal.SetDualMode(this, dualMode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetDualMode(out bool dualMode)
        {
            int optionValue = 0;
            bool error = GetOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, ref optionValue);
            dualMode = optionValue == 0;
            return error;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketError Bind(in SocketAddress address) => UdpPal.Bind(this, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketError Connect(in SocketAddress address) => UdpPal.Connect(this, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetOption(SocketOptionLevel level, SocketOptionName name, ref int value) => UdpPal.SetOption(this, level, name, ref value) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetOption(SocketOptionLevel level, SocketOptionName name, ref int value) => UdpPal.GetOption(this, level, name, ref value) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetNonBlocking(bool nonBlocking) => UdpPal.SetNonBlocking(this, nonBlocking) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Poll(int microseconds, SelectMode mode) => UdpPal.Poll(this, microseconds, mode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Send(ReadOnlySpan<byte> buffer) => UdpPal.Send(this, ref MemoryMarshal.GetReference(buffer), buffer.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Receive(Span<byte> buffer) => UdpPal.Receive(this, ref MemoryMarshal.GetReference(buffer), buffer.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SendTo(ReadOnlySpan<byte> buffer, in SocketAddress address) => UdpPal.SendTo(this, ref MemoryMarshal.GetReference(buffer), buffer.Length, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReceiveFrom(Span<byte> buffer, ref SocketAddress address) => UdpPal.ReceiveFrom(this, ref MemoryMarshal.GetReference(buffer), buffer.Length, ref address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetAddress(ref SocketAddress address) => UdpPal.GetAddress(this, ref address) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetSendBufferSize(out int sendBufferSize)
        {
            sendBufferSize = 0;
            return UdpPal.GetOption(this, SocketOptionLevel.Socket, SocketOptionName.SendBuffer, ref sendBufferSize) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetSendBufferSize(int sendBufferSize) => UdpPal.SetOption(this, SocketOptionLevel.Socket, SocketOptionName.SendBuffer, ref sendBufferSize) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetReceiveBufferSize(out int receiveBufferSize)
        {
            receiveBufferSize = 0;
            return UdpPal.GetOption(this, SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ref receiveBufferSize) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetReceiveBufferSize(int receiveBufferSize) => UdpPal.SetOption(this, SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ref receiveBufferSize) == 0;

        public bool Equals(Socket other) => Handle == other.Handle;
        public override bool Equals(object? obj) => obj is Socket socket && Equals(socket);
        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => $"Socket[{Handle}]";

        public static bool operator ==(Socket left, Socket right) => left.Equals(right);
        public static bool operator !=(Socket left, Socket right) => !(left == right);

        public static implicit operator bool(Socket socket) => socket.Handle > 0;
        public static implicit operator nint(Socket socket) => socket.Handle;
    }
}