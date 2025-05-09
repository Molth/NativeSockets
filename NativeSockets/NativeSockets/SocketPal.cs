using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
#if !NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using unixsock;
using winsock;

#pragma warning disable CS1591

// ReSharper disable ALL

namespace NativeSockets
{
    public static unsafe class SocketPal
    {
        public static readonly ushort ADDRESS_FAMILY_INTER_NETWORK_V6;
        private static readonly delegate* managed<SocketError> _GetLastSocketError;
        private static readonly delegate* managed<SocketError> _Initialize;
        private static readonly delegate* managed<SocketError> _Cleanup;
        private static readonly delegate* managed<bool, nint> _Create;
        private static readonly delegate* managed<nint, SocketError> _Close;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _Bind_IPv4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _Bind_IPv6;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _Connect_IPv4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _Connect_IPv6;
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, int*, int, SocketError> _SetOption;
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, int*, int*, SocketError> _GetOption;
        private static readonly delegate* managed<nint, bool, SocketError> _SetBlocking;
        private static readonly delegate* managed<nint, int, SelectMode, out bool, SocketError> _Poll;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in*, int> _SendTo_IPv4;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in6*, int> _SendTo_IPv6;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in*, int> _ReceiveFrom_IPv4;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in6*, int> _ReceiveFrom_IPv6;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _GetName_IPv4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _GetName_IPv6;
        private static readonly delegate* managed<sockaddr_in*, ReadOnlySpan<char>, SocketError> _SetIP_IPv4;
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<char>, SocketError> _SetIP_IPv6;
        private static readonly delegate* managed<sockaddr_in*, Span<byte>, SocketError> _GetIP_IPv4;
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetIP_IPv6;
        private static readonly delegate* managed<sockaddr_in*, ReadOnlySpan<char>, SocketError> _SetHostName_IPv4;
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<char>, SocketError> _SetHostName_IPv6;
        private static readonly delegate* managed<sockaddr_in*, Span<byte>, SocketError> _GetHostName_IPv4;
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetHostName_IPv6;

        static SocketPal()
        {
            bool isWindows =
#if NET5_0_OR_GREATER
                OperatingSystem.IsWindows();
#else
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
            if (isWindows)
            {
                ADDRESS_FAMILY_INTER_NETWORK_V6 = WinSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
                _GetLastSocketError = &WinSock.GetLastSocketError;
                _Initialize = &WinSock.Initialize;
                _Cleanup = &WinSock.Cleanup;
                _Create = &WinSock.Create;
                _Close = &WinSock.Close;
                _Bind_IPv4 = &WinSock.Bind;
                _Bind_IPv6 = &WinSock.Bind;
                _Connect_IPv4 = &WinSock.Connect;
                _Connect_IPv6 = &WinSock.Connect;
                _SetOption = &WinSock.SetOption;
                _GetOption = &WinSock.GetOption;
                _SetBlocking = &WinSock.SetBlocking;
                _Poll = &WinSock.Poll;
                _SendTo_IPv4 = &WinSock.SendTo;
                _SendTo_IPv6 = &WinSock.SendTo;
                _ReceiveFrom_IPv4 = &WinSock.ReceiveFrom;
                _ReceiveFrom_IPv6 = &WinSock.ReceiveFrom;
                _GetName_IPv4 = &WinSock.GetName;
                _GetName_IPv6 = &WinSock.GetName;
                _SetIP_IPv4 = &WinSock.SetIP;
                _SetIP_IPv6 = &WinSock.SetIP;
                _GetIP_IPv4 = &WinSock.GetIP;
                _GetIP_IPv6 = &WinSock.GetIP;
                _SetHostName_IPv4 = &WinSock.SetHostName;
                _SetHostName_IPv6 = &WinSock.SetHostName;
                _GetHostName_IPv4 = &WinSock.GetHostName;
                _GetHostName_IPv6 = &WinSock.GetHostName;
            }
            else
            {
                ADDRESS_FAMILY_INTER_NETWORK_V6 = UnixSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
                _GetLastSocketError = &UnixSock.GetLastSocketError;
                _Initialize = &UnixSock.Initialize;
                _Cleanup = &UnixSock.Cleanup;
                _Create = &UnixSock.Create;
                _Close = &UnixSock.Close;
                _Bind_IPv4 = &UnixSock.Bind;
                _Bind_IPv6 = &UnixSock.Bind;
                _Connect_IPv4 = &UnixSock.Connect;
                _Connect_IPv6 = &UnixSock.Connect;
                _SetOption = &UnixSock.SetOption;
                _GetOption = &UnixSock.GetOption;
                _SetBlocking = &UnixSock.SetBlocking;
                _Poll = &UnixSock.Poll;
                _SendTo_IPv4 = &UnixSock.SendTo;
                _SendTo_IPv6 = &UnixSock.SendTo;
                _ReceiveFrom_IPv4 = &UnixSock.ReceiveFrom;
                _ReceiveFrom_IPv6 = &UnixSock.ReceiveFrom;
                _GetName_IPv4 = &UnixSock.GetName;
                _GetName_IPv6 = &UnixSock.GetName;
                _SetIP_IPv4 = &UnixSock.SetIP;
                _SetIP_IPv6 = &UnixSock.SetIP;
                _GetIP_IPv4 = &UnixSock.GetIP;
                _GetIP_IPv6 = &UnixSock.GetIP;
                _SetHostName_IPv4 = &UnixSock.SetHostName;
                _SetHostName_IPv6 = &UnixSock.SetHostName;
                _GetHostName_IPv4 = &UnixSock.GetHostName;
                _GetHostName_IPv6 = &UnixSock.GetHostName;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError() => _GetLastSocketError();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Initialize() => _Initialize();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Cleanup() => _Cleanup();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint Create(bool ipv6) => _Create(ipv6);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Close(nint socket) => _Close(socket);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind(nint socket, sockaddr_in* socketAddress) => _Bind_IPv4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind(nint socket, sockaddr_in6* socketAddress) => _Bind_IPv6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(nint socket, sockaddr_in* socketAddress) => _Connect_IPv4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(nint socket, sockaddr_in6* socketAddress) => _Connect_IPv6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int length = sizeof(int)) => _SetOption(socket, level, name, value, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int* length = null) => _GetOption(socket, level, name, value, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(nint socket, bool blocking) => _SetBlocking(socket, blocking);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(nint socket, int microseconds, SelectMode mode, out bool status) => _Poll(socket, microseconds, mode, out status);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(nint socket, void* buffer, int length, sockaddr_in* socketAddress) => _SendTo_IPv4(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(nint socket, void* buffer, int length, sockaddr_in6* socketAddress) => _SendTo_IPv6(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom(nint socket, void* buffer, int length, sockaddr_in* socketAddress) => _ReceiveFrom_IPv4(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom(nint socket, void* buffer, int length, sockaddr_in6* socketAddress) => _ReceiveFrom_IPv6(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(nint socket, sockaddr_in* socketAddress) => _GetName_IPv4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(nint socket, sockaddr_in6* socketAddress) => _GetName_IPv6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP(sockaddr_in* pAddrBuf, ReadOnlySpan<char> ip) => _SetIP_IPv4(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP(sockaddr_in6* pAddrBuf, ReadOnlySpan<char> ip) => _SetIP_IPv6(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIP(sockaddr_in* pAddrBuf, Span<byte> ip) => _GetIP_IPv4(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIP(sockaddr_in6* pAddrBuf, Span<byte> ip) => _GetIP_IPv6(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostName(sockaddr_in* pAddrBuf, ReadOnlySpan<char> hostName) => _SetHostName_IPv4(pAddrBuf, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostName(sockaddr_in6* pAddrBuf, ReadOnlySpan<char> hostName) => _SetHostName_IPv6(pAddrBuf, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName(sockaddr_in* socketAddress, Span<byte> hostName) => _GetHostName_IPv4(socketAddress, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName(sockaddr_in6* socketAddress, Span<byte> hostName) => _GetHostName_IPv6(socketAddress, hostName);
    }
}