using System;
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
    public static unsafe class UdpPal
    {
        public static void Initialize() => SocketPal.Initialize();

        public static void Cleanup() => SocketPal.Cleanup();

        public static Socket Create() => SocketPal.Create();

        public static void Close(ref Socket socket)
        {
            SocketPal.Close(socket);
            socket = -1;
        }

        public static int Bind(Socket socket, ref SocketAddress socketAddress)
        {
            if (Unsafe.AsPointer(ref socketAddress) == null)
                return (int)SocketPal.Bind(socket, null);

            sockaddr_in6 __socketAddress_native;
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = socketAddress.Port;
            __socketAddress_native.sin6_flowinfo = 0;
            Unsafe.CopyBlockUnaligned(__socketAddress_native.sin6_addr, Unsafe.AsPointer(ref socketAddress), 16);
            __socketAddress_native.sin6_scope_id = 0;

            return (int)SocketPal.Bind(socket, &__socketAddress_native);
        }

        public static int Connect(Socket socket, ref SocketAddress socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = socketAddress.Port;
            __socketAddress_native.sin6_flowinfo = 0;
            Unsafe.CopyBlockUnaligned(__socketAddress_native.sin6_addr, Unsafe.AsPointer(ref socketAddress), 16);
            __socketAddress_native.sin6_scope_id = 0;

            return (int)SocketPal.Connect(socket, &__socketAddress_native);
        }

        public static SocketError SetOption(Socket socket, SocketOptionLevel level, SocketOptionName name, ref int value)
        {
            SocketError error;
            fixed (int* pinnedBuffer = &value)
            {
                error = SocketPal.SetOption(socket, level, name, pinnedBuffer);
            }

            return error;
        }

        public static SocketError GetOption(Socket socket, SocketOptionLevel level, SocketOptionName name, ref int value)
        {
            SocketError error;
            fixed (int* pinnedBuffer = &value)
            {
                error = SocketPal.GetOption(socket, level, name, pinnedBuffer);
            }

            return error;
        }

        public static SocketError SetNonBlocking(Socket socket, bool nonBlocking)
        {
            SocketError error = SocketPal.SetBlocking(socket, !nonBlocking);
            return error;
        }

        public static bool Poll(Socket socket, int microseconds, SelectMode mode)
        {
            SocketError error = SocketPal.Poll(socket, microseconds, mode, out bool status);
            return error == SocketError.Success && status;
        }

        public static int Send(Socket socket, ref byte buffer, int length)
        {
            int num;
            fixed (byte* pinnedBuffer = &buffer)
            {
                num = SocketPal.SendTo(socket, pinnedBuffer, length, null);
            }

            return num;
        }

        public static int Receive(Socket socket, ref byte buffer, int length)
        {
            int result;
            fixed (byte* pinnedBuffer = &buffer)
            {
                result = SocketPal.ReceiveFrom(socket, pinnedBuffer, length, null);
            }

            return result;
        }

        public static int SendTo(Socket socket, ref byte buffer, int length, ref SocketAddress socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            __socketAddress_native.sin6_family = SocketPal.ADDRESS_FAMILY_INTER_NETWORK_V6;
            __socketAddress_native.sin6_port = socketAddress.Port;
            __socketAddress_native.sin6_flowinfo = 0;
            Unsafe.CopyBlockUnaligned(__socketAddress_native.sin6_addr, Unsafe.AsPointer(ref socketAddress), 16);
            __socketAddress_native.sin6_scope_id = 0;

            int num;
            fixed (byte* pinnedBuffer = &buffer)
            {
                num = SocketPal.SendTo(socket, pinnedBuffer, length, &__socketAddress_native);
            }

            return num;
        }

        public static int ReceiveFrom(Socket socket, ref byte buffer, int length, ref SocketAddress socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            int result;
            fixed (byte* pinnedBuffer = &buffer)
            {
                result = SocketPal.ReceiveFrom(socket, pinnedBuffer, length, &__socketAddress_native);
            }

            if (result <= 0)
                return result;

            Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref socketAddress), __socketAddress_native.sin6_addr, 16);
            socketAddress.Port = __socketAddress_native.sin6_port;

            return result;
        }

        public static SocketError GetAddress(Socket socket, ref SocketAddress socketAddress)
        {
            sockaddr_in6 __socketAddress_native;
            SocketError result = SocketPal.GetName(socket, &__socketAddress_native);
            if (result == 0)
            {
                Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref socketAddress), __socketAddress_native.sin6_addr, 16);
                socketAddress.Port = __socketAddress_native.sin6_port;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP(ref SocketAddress socketAddress, ReadOnlySpan<char> ip) => SocketPal.SetIP(Unsafe.AsPointer(ref socketAddress), ip);

        public static SocketError GetIP(ref SocketAddress socketAddress, Span<byte> ip) => SocketPal.GetIP(Unsafe.AsPointer(ref socketAddress), ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetHostName(ref SocketAddress socketAddress, ReadOnlySpan<char> hostName) => (int)SocketPal.SetHostName(Unsafe.AsPointer(ref socketAddress), hostName);

        public static SocketError GetHostName(ref SocketAddress socketAddress, Span<byte> hostName)
        {
            sockaddr_in6 __socketAddress_native;
            SocketError result = SocketPal.GetHostName(&__socketAddress_native, hostName);
            if (result == 0)
            {
                Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref socketAddress), __socketAddress_native.sin6_addr, 16);
                socketAddress.Port = __socketAddress_native.sin6_port;
            }

            return result;
        }
    }
}