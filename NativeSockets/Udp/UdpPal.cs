using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS1591
#pragma warning disable SYSLIB1054
#pragma warning disable CA1401
#pragma warning disable CA2101
#pragma warning disable CS9081

// ReSharper disable ALL

namespace NativeSockets.Udp
{
    public static unsafe class UdpPal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(Socket socket, ReadOnlySpan<byte> buffer, IPEndPoint ipEndPoint)
        {
            ushort port = WinSock2.HOST_TO_NET_16((ushort)ipEndPoint.Port);

            if (socket.AddressFamily == AddressFamily.InterNetworkV6 && ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Unsafe.SkipInit(out sockaddr_in6 __socketAddress_native);
                ref byte reference = ref Unsafe.As<sockaddr_in6, byte>(ref __socketAddress_native);
                Unsafe.WriteUnaligned(ref reference, (sa_family_t)SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6);
                Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)2), port);
                Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)4), 0U);
                ipEndPoint.Address.TryWriteBytes(MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref reference, (nint)8), 16), out _);
                Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)24), (uint)ipEndPoint.Address.ScopeId);

                fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
                {
                    return SocketPal.SendTo6(socket.Handle, pinnedBuffer, buffer.Length, &__socketAddress_native);
                }
            }

            if (socket.AddressFamily == AddressFamily.InterNetwork && ipEndPoint.AddressFamily == AddressFamily.InterNetwork)
            {
                Unsafe.SkipInit(out sockaddr_in __socketAddress_native);
                ref byte reference = ref Unsafe.As<sockaddr_in, byte>(ref __socketAddress_native);
                Unsafe.WriteUnaligned(ref reference, (sa_family_t)(ushort)AddressFamily.InterNetwork);
                Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)2), port);
                ipEndPoint.Address.TryWriteBytes(MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref reference, (nint)4), 4), out _);
                Unsafe.InitBlockUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)8), 0, 8);

                fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
                {
                    return SocketPal.SendTo4(socket.Handle, pinnedBuffer, buffer.Length, &__socketAddress_native);
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(Socket socket, ReadOnlySpan<byte> buffer, ReadOnlySpan<byte> socketAddress)
        {
            if (socket.AddressFamily == AddressFamily.InterNetworkV6)
            {
                if (socketAddress.Length < 28)
                    return -1;

                fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
                {
                    fixed (byte* __socketAddress_native = &MemoryMarshal.GetReference(socketAddress))
                    {
                        return SocketPal.SendTo6(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in6*)__socketAddress_native);
                    }
                }
            }

            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                if (socketAddress.Length < 16)
                    return -1;

                fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
                {
                    fixed (byte* __socketAddress_native = &MemoryMarshal.GetReference(socketAddress))
                    {
                        return SocketPal.SendTo4(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in*)__socketAddress_native);
                    }
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom(Socket socket, Span<byte> buffer, ref Span<byte> socketAddress)
        {
            if (socket.AddressFamily == AddressFamily.InterNetworkV6)
            {
                if (socketAddress.Length < 28)
                    return -1;

                socketAddress = socketAddress.Slice(0, 28);

                fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
                {
                    fixed (byte* __socketAddress_native = &MemoryMarshal.GetReference(socketAddress))
                    {
                        return SocketPal.ReceiveFrom6(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in6*)__socketAddress_native);
                    }
                }
            }

            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                if (socketAddress.Length < 16)
                    return -1;

                socketAddress = socketAddress.Slice(0, 16);

                fixed (byte* pinnedBuffer = &MemoryMarshal.GetReference(buffer))
                {
                    fixed (byte* __socketAddress_native = &MemoryMarshal.GetReference(socketAddress))
                    {
                        return SocketPal.ReceiveFrom4(socket.Handle, pinnedBuffer, buffer.Length, (sockaddr_in*)__socketAddress_native);
                    }
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateIPEndPoint(ReadOnlySpan<byte> socketAddress, out IPEndPoint? ipEndPoint)
        {
            ipEndPoint = null;

            if (socketAddress.Length < 16)
                return false;

            ref byte reference = ref MemoryMarshal.GetReference(socketAddress);

            sa_family_t family = Unsafe.ReadUnaligned<sa_family_t>(ref reference);

            if (family.IsIPv4)
            {
                sockaddr_in sockaddrIn = Unsafe.ReadUnaligned<sockaddr_in>(ref reference);
                ipEndPoint = new IPEndPoint(Unsafe.ReadUnaligned<uint>(&sockaddrIn.sin_addr), WinSock2.NET_TO_HOST_16(sockaddrIn.sin_port));

                return true;
            }

            if (family.IsIPv6)
            {
                if (socketAddress.Length < 28)
                    return false;

                sockaddr_in6 sockaddrIn6 = Unsafe.ReadUnaligned<sockaddr_in6>(ref reference);
                ipEndPoint = new IPEndPoint(new IPAddress(MemoryMarshal.CreateReadOnlySpan(ref *sockaddrIn6.sin6_addr, 16), sockaddrIn6.sin6_scope_id), WinSock2.NET_TO_HOST_16(sockaddrIn6.sin6_port));

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateSocketAddress(ReadOnlySpan<byte> socketAddress, out SocketAddress? address)
        {
            address = null;

            if (socketAddress.Length < 16)
                return false;

            ref byte reference = ref MemoryMarshal.GetReference(socketAddress);

            sa_family_t family = Unsafe.ReadUnaligned<sa_family_t>(ref reference);

            if (family.IsIPv4)
            {
                address = new SocketAddress(AddressFamily.InterNetwork);

#if NET8_0_OR_GREATER
                socketAddress.CopyTo(address.Buffer.Span);
#else
                for (int i = 0; i < 16; ++i)
                    address[i] = socketAddress[i];
#endif

                return true;
            }

            if (family.IsIPv6)
            {
                if (socketAddress.Length < 28)
                    return false;

                address = new SocketAddress(AddressFamily.InterNetworkV6);

#if NET8_0_OR_GREATER
                socketAddress.CopyTo(address.Buffer.Span);
#else
                for (int i = 0; i < 28; ++i)
                    address[i] = socketAddress[i];
#endif

                return true;
            }

            return false;
        }
    }
}