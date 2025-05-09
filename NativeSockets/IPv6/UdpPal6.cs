﻿using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security;
using winsock;

#pragma warning disable CS1591
#pragma warning disable SYSLIB1054
#pragma warning disable CA1401
#pragma warning disable CA2101

// ReSharper disable ALL

namespace NativeSockets.Udp
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class UdpPal6
    {
        public static void Initialize() => SocketPal.Initialize();

        public static void Cleanup() => SocketPal.Cleanup();

        public static Socket6 Create() => SocketPal.Create(true);

        public static void Close(ref Socket6 socket)
        {
            SocketPal.Close(socket);
            socket = -1;
        }

        public static SocketError Bind(Socket6 socket, ref SocketAddress6 socketAddress)
        {
            if (Unsafe.AsPointer(ref socketAddress) == null)
                return SocketPal.Bind6(socket, (sockaddr_in6*)null);

            sockaddr_in6 __socketAddress_native;
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = socketAddress.Port;
            __socketAddress_native.sin6_flowinfo = 0;
            Unsafe.CopyBlockUnaligned(ref *__socketAddress_native.sin6_addr, ref socketAddress.GetPinnableReference(), 16);
            __socketAddress_native.sin6_scope_id = 0;

            return SocketPal.Bind6(socket, &__socketAddress_native);
        }

        public static SocketError Connect(Socket6 socket, ref SocketAddress6 socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = socketAddress.Port;
            __socketAddress_native.sin6_flowinfo = 0;
            Unsafe.CopyBlockUnaligned(ref *__socketAddress_native.sin6_addr, ref socketAddress.GetPinnableReference(), 16);
            __socketAddress_native.sin6_scope_id = 0;

            return SocketPal.Connect6(socket, &__socketAddress_native);
        }

        public static SocketError SetOption(Socket6 socket, SocketOptionLevel level, SocketOptionName name, ref int value)
        {
            SocketError error;
            fixed (int* pinnedBuffer = &value)
            {
                error = SocketPal.SetOption(socket, level, name, pinnedBuffer);
            }

            return error;
        }

        public static SocketError GetOption(Socket6 socket, SocketOptionLevel level, SocketOptionName name, ref int value)
        {
            SocketError error;
            fixed (int* pinnedBuffer = &value)
            {
                error = SocketPal.GetOption(socket, level, name, pinnedBuffer);
            }

            return error;
        }

        public static SocketError SetNonBlocking(Socket6 socket, bool nonBlocking)
        {
            SocketError error = SocketPal.SetBlocking(socket, !nonBlocking);
            return error;
        }

        public static bool Poll(Socket6 socket, int microseconds, SelectMode mode)
        {
            SocketError error = SocketPal.Poll(socket, microseconds, mode, out bool status);
            return error == SocketError.Success && status;
        }

        public static int Send(Socket6 socket, ref byte buffer, int length)
        {
            int num;
            fixed (byte* pinnedBuffer = &buffer)
            {
                num = SocketPal.SendTo6(socket, pinnedBuffer, length, (sockaddr_in6*)null);
            }

            return num;
        }

        public static int Receive(Socket6 socket, ref byte buffer, int length)
        {
            int result;
            fixed (byte* pinnedBuffer = &buffer)
            {
                result = SocketPal.ReceiveFrom6(socket, pinnedBuffer, length, (sockaddr_in6*)null);
            }

            return result;
        }

        public static int SendTo(Socket6 socket, ref byte buffer, int length, ref SocketAddress6 socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = socketAddress.Port;
            __socketAddress_native.sin6_flowinfo = 0;
            Unsafe.CopyBlockUnaligned(ref *__socketAddress_native.sin6_addr, ref socketAddress.GetPinnableReference(), 16);
            __socketAddress_native.sin6_scope_id = 0;

            int num;
            fixed (byte* pinnedBuffer = &buffer)
            {
                num = SocketPal.SendTo6(socket, pinnedBuffer, length, &__socketAddress_native);
            }

            return num;
        }

        public static int ReceiveFrom(Socket6 socket, ref byte buffer, int length, ref SocketAddress6 socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            int result;
            fixed (byte* pinnedBuffer = &buffer)
            {
                result = SocketPal.ReceiveFrom6(socket, pinnedBuffer, length, &__socketAddress_native);
            }

            if (result <= 0)
                return result;

            Unsafe.CopyBlockUnaligned(ref socketAddress.GetPinnableReference(), ref *__socketAddress_native.sin6_addr, 16);
            socketAddress.Port = __socketAddress_native.sin6_port;

            return result;
        }

        public static SocketError GetAddress(Socket6 socket, ref SocketAddress6 socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            SocketError error = SocketPal.GetName6(socket, &__socketAddress_native);
            if (error == 0)
            {
                Unsafe.CopyBlockUnaligned(ref socketAddress.GetPinnableReference(), ref *__socketAddress_native.sin6_addr, 16);
                socketAddress.Port = __socketAddress_native.sin6_port;
            }

            return error;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP(ref SocketAddress6 socketAddress, ReadOnlySpan<char> ip)
        {
            sockaddr_in6 __socketAddress_native;
            SocketError error = SocketPal.SetIP6(&__socketAddress_native, ip);
            if (error == 0)
                Unsafe.CopyBlockUnaligned(ref socketAddress.GetPinnableReference(), ref *__socketAddress_native.sin6_addr, 16);

            return error;
        }

        public static SocketError GetIP(ref SocketAddress6 socketAddress, Span<byte> ip)
        {
            sockaddr_in6 __socketAddress_native;
            Unsafe.CopyBlockUnaligned(ref *__socketAddress_native.sin6_addr, ref socketAddress.GetPinnableReference(), 16);

            SocketError error = SocketPal.GetIP6(&__socketAddress_native, ip);

            return error;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostName(ref SocketAddress6 socketAddress, ReadOnlySpan<char> hostName)
        {
            sockaddr_in6 __socketAddress_native;
            SocketError error = SocketPal.SetHostName6(&__socketAddress_native, hostName);
            if (error == 0)
                Unsafe.CopyBlockUnaligned(ref socketAddress.GetPinnableReference(), ref *__socketAddress_native.sin6_addr, 16);

            return error;
        }

        public static SocketError GetHostName(ref SocketAddress6 socketAddress, Span<byte> hostName)
        {
            ref byte reference = ref socketAddress.GetPinnableReference();

            sockaddr_in6 __socketAddress_native;

            __socketAddress_native.sin6_family = (ushort)SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = socketAddress.Port;
            __socketAddress_native.sin6_flowinfo = 0;
            Unsafe.CopyBlockUnaligned(ref *__socketAddress_native.sin6_addr, ref reference, 16);
            __socketAddress_native.sin6_scope_id = 0;

            SocketError error = SocketPal.GetHostName6(&__socketAddress_native, hostName);
            if (error == 0)
            {
                Unsafe.CopyBlockUnaligned(ref reference, ref *__socketAddress_native.sin6_addr, 16);
                socketAddress.Port = __socketAddress_native.sin6_port;
            }

            return error;
        }
    }
}