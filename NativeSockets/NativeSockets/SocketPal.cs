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
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _Bind_4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _Bind_6;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _Connect_4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _Connect_6;
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, int*, int, SocketError> _SetOption;
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, int*, int*, SocketError> _GetOption;
        private static readonly delegate* managed<nint, bool, SocketError> _SetBlocking;
        private static readonly delegate* managed<nint, int, SelectMode, out bool, SocketError> _Poll;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in*, int> _SendTo_4;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in6*, int> _SendTo_6;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in*, int> _ReceiveFrom_4;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in6*, int> _ReceiveFrom_6;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _GetName_4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _GetName_6;
        private static readonly delegate* managed<sockaddr_in*, ReadOnlySpan<char>, SocketError> _SetIP_4;
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<char>, SocketError> _SetIP_6;
        private static readonly delegate* managed<sockaddr_in*, Span<byte>, SocketError> _GetIP_4;
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetIP_6;
        private static readonly delegate* managed<sockaddr_in*, ReadOnlySpan<char>, SocketError> _SetHostName_4;
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<char>, SocketError> _SetHostName_6;
        private static readonly delegate* managed<sockaddr_in*, Span<byte>, SocketError> _GetHostName_4;
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetHostName_6;

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
                _Bind_4 = &WinSock.Bind;
                _Bind_6 = &WinSock.Bind;
                _Connect_4 = &WinSock.Connect;
                _Connect_6 = &WinSock.Connect;
                _SetOption = &WinSock.SetOption;
                _GetOption = &WinSock.GetOption;
                _SetBlocking = &WinSock.SetBlocking;
                _Poll = &WinSock.Poll;
                _SendTo_4 = &WinSock.SendTo;
                _SendTo_6 = &WinSock.SendTo;
                _ReceiveFrom_4 = &WinSock.ReceiveFrom;
                _ReceiveFrom_6 = &WinSock.ReceiveFrom;
                _GetName_4 = &WinSock.GetName;
                _GetName_6 = &WinSock.GetName;
                _SetIP_4 = &WinSock.SetIP;
                _SetIP_6 = &WinSock.SetIP;
                _GetIP_4 = &WinSock.GetIP;
                _GetIP_6 = &WinSock.GetIP;
                _SetHostName_4 = &WinSock.SetHostName;
                _SetHostName_6 = &WinSock.SetHostName;
                _GetHostName_4 = &WinSock.GetHostName;
                _GetHostName_6 = &WinSock.GetHostName;
            }
            else
            {
                ADDRESS_FAMILY_INTER_NETWORK_V6 = UnixSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
                _GetLastSocketError = &UnixSock.GetLastSocketError;
                _Initialize = &UnixSock.Initialize;
                _Cleanup = &UnixSock.Cleanup;
                _Create = &UnixSock.Create;
                _Close = &UnixSock.Close;
                _Bind_4 = &UnixSock.Bind;
                _Bind_6 = &UnixSock.Bind;
                _Connect_4 = &UnixSock.Connect;
                _Connect_6 = &UnixSock.Connect;
                _SetOption = &UnixSock.SetOption;
                _GetOption = &UnixSock.GetOption;
                _SetBlocking = &UnixSock.SetBlocking;
                _Poll = &UnixSock.Poll;
                _SendTo_4 = &UnixSock.SendTo;
                _SendTo_6 = &UnixSock.SendTo;
                _ReceiveFrom_4 = &UnixSock.ReceiveFrom;
                _ReceiveFrom_6 = &UnixSock.ReceiveFrom;
                _GetName_4 = &UnixSock.GetName;
                _GetName_6 = &UnixSock.GetName;
                _SetIP_4 = &UnixSock.SetIP;
                _SetIP_6 = &UnixSock.SetIP;
                _GetIP_4 = &UnixSock.GetIP;
                _GetIP_6 = &UnixSock.GetIP;
                _SetHostName_4 = &UnixSock.SetHostName;
                _SetHostName_6 = &UnixSock.SetHostName;
                _GetHostName_4 = &UnixSock.GetHostName;
                _GetHostName_6 = &UnixSock.GetHostName;
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
        public static SocketError Bind(nint socket, sockaddr_in* socketAddress) => _Bind_4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind(nint socket, sockaddr_in6* socketAddress) => _Bind_6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(nint socket, sockaddr_in* socketAddress) => _Connect_4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect(nint socket, sockaddr_in6* socketAddress) => _Connect_6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int length = sizeof(int)) => _SetOption(socket, level, name, value, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int* length = null) => _GetOption(socket, level, name, value, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(nint socket, bool blocking) => _SetBlocking(socket, blocking);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(nint socket, int microseconds, SelectMode mode, out bool status) => _Poll(socket, microseconds, mode, out status);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(nint socket, void* buffer, int length, sockaddr_in* socketAddress) => _SendTo_4(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(nint socket, void* buffer, int length, sockaddr_in6* socketAddress) => _SendTo_6(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom(nint socket, void* buffer, int length, sockaddr_in* socketAddress) => _ReceiveFrom_4(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom(nint socket, void* buffer, int length, sockaddr_in6* socketAddress) => _ReceiveFrom_6(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(nint socket, sockaddr_in* socketAddress) => _GetName_4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName(nint socket, sockaddr_in6* socketAddress) => _GetName_6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP(sockaddr_in* pAddrBuf, ReadOnlySpan<char> ip) => _SetIP_4(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP(sockaddr_in6* pAddrBuf, ReadOnlySpan<char> ip) => _SetIP_6(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIP(sockaddr_in* pAddrBuf, Span<byte> ip) => _GetIP_4(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIP(sockaddr_in6* pAddrBuf, Span<byte> ip) => _GetIP_6(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostName(sockaddr_in* pAddrBuf, ReadOnlySpan<char> hostName) => _SetHostName_4(pAddrBuf, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostName(sockaddr_in6* pAddrBuf, ReadOnlySpan<char> hostName) => _SetHostName_6(pAddrBuf, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName(sockaddr_in* socketAddress, Span<byte> hostName) => _GetHostName_4(socketAddress, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName(sockaddr_in6* socketAddress, Span<byte> hostName) => _GetHostName_6(socketAddress, hostName);
    }
}