﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS1591
#pragma warning disable CS8632
#pragma warning disable CS9084

// ReSharper disable ALL

namespace NativeSockets.Udp
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public unsafe struct SocketAddress4 : IEquatable<SocketAddress4>
    {
        public static ref SocketAddress4 NullRef => ref Unsafe.NullRef<SocketAddress4>();

        [FieldOffset(0)] public uint Address;
        [FieldOffset(4)] public ushort Port;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetPinnableReference() => ref Unsafe.As<SocketAddress4, byte>(ref Unsafe.AsRef(in this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateFromIP(ReadOnlySpan<char> ip, out SocketAddress4 address)
        {
            address = new SocketAddress4();
            return UdpPal4.SetIP(ref address, ip) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateFromHostName(ReadOnlySpan<char> name, out SocketAddress4 address)
        {
            address = new SocketAddress4();
            return UdpPal4.SetHostName(ref address, name) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IPAddress CreateIPAddress() => new IPAddress(Address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IPEndPoint CreateIPEndPoint() => new IPEndPoint(CreateIPAddress(), Port);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAddress CreateSocketAddress()
        {
            SocketAddress socketAddress = new SocketAddress(AddressFamily.InterNetwork);
            ReadOnlySpan<byte> buffer = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<uint, byte>(ref Address), 4);
            ushort port = WinSock2.HOST_TO_NET_16(Port);
#if NET8_0_OR_GREATER
            buffer.CopyTo(socketAddress.Buffer.Span.Slice(4));
            Unsafe.WriteUnaligned(ref socketAddress.Buffer.Span[2], port);
#else
            for (int i = 4; i < 8; ++i)
                socketAddress[i] = buffer[i - 4];
            buffer = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<ushort, byte>(ref port), 2);
            for (int i = 2; i < 4; ++i)
                socketAddress[i] = buffer[i - 2];
#endif
            return socketAddress;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIP(out string? ip)
        {
            Span<byte> buffer = stackalloc byte[1024];
            SocketError error = UdpPal4.GetIP(ref Unsafe.AsRef(in this), buffer);
            if (error == 0)
            {
                ip = Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]);
                return true;
            }

            ip = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIP(Span<byte> ip, out int byteCount)
        {
            Span<byte> buffer = stackalloc byte[1024];
            SocketError error = UdpPal4.GetIP(ref Unsafe.AsRef(in this), buffer);
            if (error == 0)
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
        public bool GetHostName(out string? ip)
        {
            Span<byte> buffer = stackalloc byte[1024];
            SocketError error = UdpPal4.GetHostName(ref Unsafe.AsRef(in this), buffer);
            if (error == 0)
            {
                ip = Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]);
                return true;
            }

            ip = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetHostName(Span<byte> name, out int byteCount)
        {
            Span<byte> buffer = stackalloc byte[1024];
            SocketError error = UdpPal4.GetHostName(ref Unsafe.AsRef(in this), buffer);
            if (error == 0)
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

        public bool Equals(SocketAddress4 other)
        {
            ref byte local1 = ref Unsafe.As<SocketAddress4, byte>(ref Unsafe.AsRef(in this));
            ref byte local2 = ref Unsafe.As<SocketAddress4, byte>(ref other);
            return SpanHelpers.Compare(ref local1, ref local2, (nuint)sizeof(SocketAddress4));
        }

        public override bool Equals(object? obj) => obj is SocketAddress4 socketAddress && Equals(socketAddress);

        public override int GetHashCode() => XxHash.Hash32(this);

        public override string ToString()
        {
            Span<byte> buffer = stackalloc byte[128];

            SocketError error = UdpPal4.GetIP(ref Unsafe.AsRef(in this), buffer);
            if (error != 0)
                return "ERROR";

            Span<char> destination = stackalloc char[256];

            int chars = 0;
            int charsWritten;

            chars += Encoding.ASCII.GetChars(buffer.Slice(0, buffer.IndexOf((byte)'\0')), destination.Slice(chars));

            destination[chars] = ':';
            ++chars;

            Port.TryFormat(destination.Slice(chars), out charsWritten);
            chars += charsWritten;

            destination[chars] = '\0';
            ++chars;

            return destination.Slice(0, chars).ToString();
        }

        public static bool operator ==(SocketAddress4 left, SocketAddress4 right) => left.Equals(right);
        public static bool operator !=(SocketAddress4 left, SocketAddress4 right) => !(left == right);

        public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<SocketAddress4, byte>(ref Unsafe.AsRef(in this)), 8);
        public ReadOnlySpan<byte> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<SocketAddress4, byte>(ref Unsafe.AsRef(in this)), 8);

        public static implicit operator MnSocketAddress(SocketAddress4 socketAddress)
        {
            MnSocketAddress address = new MnSocketAddress();

            ref byte reference = ref Unsafe.As<MnSocketAddress, byte>(ref address);
            Unsafe.InitBlockUnaligned(ref reference, 0, 8);
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)8), WinSock2.ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6);

            address.Address = socketAddress.Address;
            address.Port = socketAddress.Port;

            return address;
        }

        public static implicit operator SocketAddress6(SocketAddress4 socketAddress)
        {
            SocketAddress6 address = new SocketAddress6();

            ref byte reference = ref Unsafe.As<SocketAddress6, byte>(ref address);
            Unsafe.InitBlockUnaligned(ref reference, 0, 8);
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)8), WinSock2.ADDRESS_FAMILY_INTER_NETWORK_V4_MAPPED_V6);

            Unsafe.As<byte, uint>(ref Unsafe.AddByteOffset(ref Unsafe.As<SocketAddress6, byte>(ref address), (nint)12)) = socketAddress.Address;
            address.Port = socketAddress.Port;

            return address;
        }
    }
}