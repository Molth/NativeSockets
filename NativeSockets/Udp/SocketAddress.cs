using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if NET7_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

#pragma warning disable CS1591
#pragma warning disable CS8632

// ReSharper disable ALL

namespace NativeSockets.Udp
{
    [StructLayout(LayoutKind.Explicit, Size = 20)]
    public unsafe struct SocketAddress : IEquatable<SocketAddress>
    {
        public static ref SocketAddress NullRef => ref Unsafe.AsRef<SocketAddress>(null);

        public Span<byte> IPv6
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref Unsafe.As<SocketAddress, byte>(ref Unsafe.AsRef(in this)), 16);
        }

        public Span<byte> IPv4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref Unsafe.Add(ref Unsafe.As<SocketAddress, byte>(ref Unsafe.AsRef(in this)), 12), 4);
        }

        [FieldOffset(16)] public ushort Port;

        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref long reference = ref Unsafe.As<SocketAddress, long>(ref Unsafe.AsRef(in this));
                return reference != 0 || Unsafe.Add(ref reference, 1) != 0;
            }
        }

        public bool IsIPv4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref int reference = ref Unsafe.As<SocketAddress, int>(ref Unsafe.AsRef(in this));
                return Unsafe.Add(ref reference, 2) == -0x10000 && reference == 0 && Unsafe.Add(ref reference, 1) == 0;
            }
        }

        public bool IsIPv6
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !IsIPv4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateFromIP(ReadOnlySpan<char> ip, out SocketAddress address)
        {
            address = new SocketAddress();
            return UdpPal.SetIP(ref address, ip) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateFromHostName(ReadOnlySpan<char> name, out SocketAddress address)
        {
            address = new SocketAddress();
            return UdpPal.SetHostName(ref address, name) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIP(int bufferSize, Span<byte> ip, out int byteCount)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            SocketError status = UdpPal.GetIP(ref Unsafe.AsRef(in this), buffer);
            if (status == 0)
            {
                byteCount = buffer.IndexOf((byte)'\0');
                if (ip.Length < byteCount)
                    return false;
                Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(ip), ref MemoryMarshal.GetReference(buffer), (uint)byteCount);
                return true;
            }

            byteCount = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetHostName(int bufferSize, Span<byte> name, out int byteCount)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            SocketError status = UdpPal.GetHostName(ref Unsafe.AsRef(in this), buffer);
            if (status == 0)
            {
                byteCount = buffer.IndexOf((byte)'\0');
                if (name.Length < byteCount)
                    return false;
                Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(name), ref MemoryMarshal.GetReference(buffer), (uint)byteCount);
                return true;
            }

            byteCount = 0;
            return false;
        }

        public bool Equals(SocketAddress other)
        {
#if NET7_0_OR_GREATER
            if (Vector128.IsHardwareAccelerated)
                return Vector128.LoadUnsafe(ref Unsafe.As<SocketAddress, byte>(ref Unsafe.AsRef(in this))) == Vector128.LoadUnsafe(ref Unsafe.As<SocketAddress, byte>(ref other)) && Port == other.Port;
#endif
            ref int left = ref Unsafe.As<SocketAddress, int>(ref Unsafe.AsRef(in this));
            ref int right = ref Unsafe.As<SocketAddress, int>(ref other);
            return left == right && Unsafe.Add(ref left, 1) == Unsafe.Add(ref right, 1) && Unsafe.Add(ref left, 2) == Unsafe.Add(ref right, 2) && Unsafe.Add(ref left, 3) == Unsafe.Add(ref right, 3) && Unsafe.Add(ref left, 4) == Unsafe.Add(ref right, 4);
        }

        public override bool Equals(object? obj) => obj is SocketAddress socketAddress && Equals(socketAddress);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
#if NET6_0_OR_GREATER
            hashCode.AddBytes(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<SocketAddress, byte>(ref Unsafe.AsRef(in this)), 20));
#else
            ref int reference = ref Unsafe.As<SocketAddress, int>(ref Unsafe.AsRef(in this));
            for (int i = 0; i < 5; i++)
                hashCode.Add(Unsafe.Add(ref reference, i));
#endif
            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            Span<byte> buffer = stackalloc byte[64];
            return UdpPal.GetIP(ref Unsafe.AsRef(in this), buffer) == 0 ? Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]) + ":" + Port : "ERROR";
        }

        public string ToString(int bufferSize)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            return UdpPal.GetIP(ref Unsafe.AsRef(in this), buffer) == 0 ? Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]) + ":" + Port : "ERROR";
        }

        public static bool operator ==(SocketAddress left, SocketAddress right) => left.Equals(right);
        public static bool operator !=(SocketAddress left, SocketAddress right) => !(left == right);

        public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<SocketAddress, byte>(ref Unsafe.AsRef(in this)), 20);
        public ReadOnlySpan<byte> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<SocketAddress, byte>(ref Unsafe.AsRef(in this)), 20);
    }
}