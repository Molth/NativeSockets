using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using unixsock;
using winsock;

#pragma warning disable CS1591

// ReSharper disable ALL

namespace NativeSockets
{
    public static unsafe class SocketPal
    {
        public static readonly ushort ADDRESS_FAMILY_INTER_NETWORK_V6;

        public static bool IsWindows => ADDRESS_FAMILY_INTER_NETWORK_V6 == WindowsSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
        public static bool IsLinux => ADDRESS_FAMILY_INTER_NETWORK_V6 == LinuxSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
        public static bool IsBsd => ADDRESS_FAMILY_INTER_NETWORK_V6 == BsdSock.ADDRESS_FAMILY_INTER_NETWORK_V6;

        private static readonly delegate* managed<SocketError> _GetLastSocketError;
        private static readonly delegate* managed<SocketError> _Initialize;
        private static readonly delegate* managed<SocketError> _Cleanup;
        private static readonly delegate* managed<bool, nint> _Create;
        private static readonly delegate* managed<nint, SocketError> _Close;
        private static readonly delegate* managed<nint, bool, SocketError> _SetDualMode6;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _Bind4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _Bind6;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _Connect4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _Connect6;
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, int*, int, SocketError> _SetOption;
        private static readonly delegate* managed<nint, SocketOptionLevel, SocketOptionName, int*, int*, SocketError> _GetOption;
        private static readonly delegate* managed<nint, bool, SocketError> _SetBlocking;
        private static readonly delegate* managed<nint, int, SelectMode, out bool, SocketError> _Poll;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in*, int> _SendTo4;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in6*, int> _SendTo6;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in*, int> _ReceiveFrom4;
        private static readonly delegate* managed<nint, void*, int, sockaddr_in6*, int> _ReceiveFrom6;
        private static readonly delegate* managed<nint, sockaddr_in*, SocketError> _GetName4;
        private static readonly delegate* managed<nint, sockaddr_in6*, SocketError> _GetName6;
        private static readonly delegate* managed<sockaddr_in*, ReadOnlySpan<char>, SocketError> _SetIP4;
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<char>, SocketError> _SetIP6;
        private static readonly delegate* managed<sockaddr_in*, Span<byte>, SocketError> _GetIP4;
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetIP6;
        private static readonly delegate* managed<sockaddr_in*, ReadOnlySpan<char>, SocketError> _SetHostName4;
        private static readonly delegate* managed<sockaddr_in6*, ReadOnlySpan<char>, SocketError> _SetHostName6;
        private static readonly delegate* managed<sockaddr_in*, Span<byte>, SocketError> _GetHostName4;
        private static readonly delegate* managed<sockaddr_in6*, Span<byte>, SocketError> _GetHostName6;

        static SocketPal()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ADDRESS_FAMILY_INTER_NETWORK_V6 = WindowsSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
                _GetLastSocketError = &WindowsSock.GetLastSocketError;
                _Initialize = &WindowsSock.Initialize;
                _Cleanup = &WindowsSock.Cleanup;
                _Create = &WindowsSock.Create;
                _Close = &WindowsSock.Close;
                _SetDualMode6 = &WindowsSock.SetDualMode6;
                _Bind4 = &WindowsSock.Bind4;
                _Bind6 = &WindowsSock.Bind6;
                _Connect4 = &WindowsSock.Connect4;
                _Connect6 = &WindowsSock.Connect6;
                _SetOption = &WindowsSock.SetOption;
                _GetOption = &WindowsSock.GetOption;
                _SetBlocking = &WindowsSock.SetBlocking;
                _Poll = &WindowsSock.Poll;
                _SendTo4 = &WindowsSock.SendTo4;
                _SendTo6 = &WindowsSock.SendTo6;
                _ReceiveFrom4 = &WindowsSock.ReceiveFrom4;
                _ReceiveFrom6 = &WindowsSock.ReceiveFrom6;
                _GetName4 = &WindowsSock.GetName4;
                _GetName6 = &WindowsSock.GetName6;
                _SetIP4 = &WindowsSock.SetIP4;
                _SetIP6 = &WindowsSock.SetIP6;
                _GetIP4 = &WindowsSock.GetIP4;
                _GetIP6 = &WindowsSock.GetIP6;
                _SetHostName4 = &WindowsSock.SetHostName4;
                _SetHostName6 = &WindowsSock.SetHostName6;
                _GetHostName4 = &WindowsSock.GetHostName4;
                _GetHostName6 = &WindowsSock.GetHostName6;
                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ADDRESS_FAMILY_INTER_NETWORK_V6 = LinuxSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
                _GetLastSocketError = &LinuxSock.GetLastSocketError;
                _Initialize = &LinuxSock.Initialize;
                _Cleanup = &LinuxSock.Cleanup;
                _Create = &LinuxSock.Create;
                _Close = &LinuxSock.Close;
                _SetDualMode6 = &LinuxSock.SetDualMode6;
                _Bind4 = &LinuxSock.Bind4;
                _Bind6 = &LinuxSock.Bind6;
                _Connect4 = &LinuxSock.Connect4;
                _Connect6 = &LinuxSock.Connect6;
                _SetOption = &LinuxSock.SetOption;
                _GetOption = &LinuxSock.GetOption;
                _SetBlocking = &LinuxSock.SetBlocking;
                _Poll = &LinuxSock.Poll;
                _SendTo4 = &LinuxSock.SendTo4;
                _SendTo6 = &LinuxSock.SendTo6;
                _ReceiveFrom4 = &LinuxSock.ReceiveFrom4;
                _ReceiveFrom6 = &LinuxSock.ReceiveFrom6;
                _GetName4 = &LinuxSock.GetName4;
                _GetName6 = &LinuxSock.GetName6;
                _SetIP4 = &LinuxSock.SetIP4;
                _SetIP6 = &LinuxSock.SetIP6;
                _GetIP4 = &LinuxSock.GetIP4;
                _GetIP6 = &LinuxSock.GetIP6;
                _SetHostName4 = &LinuxSock.SetHostName4;
                _SetHostName6 = &LinuxSock.SetHostName6;
                _GetHostName4 = &LinuxSock.GetHostName4;
                _GetHostName6 = &LinuxSock.GetHostName6;
                return;
            }

            ADDRESS_FAMILY_INTER_NETWORK_V6 = BsdSock.ADDRESS_FAMILY_INTER_NETWORK_V6;
            _GetLastSocketError = &BsdSock.GetLastSocketError;
            _Initialize = &BsdSock.Initialize;
            _Cleanup = &BsdSock.Cleanup;
            _Create = &BsdSock.Create;
            _Close = &BsdSock.Close;
            _SetDualMode6 = &BsdSock.SetDualMode6;
            _Bind4 = &BsdSock.Bind4;
            _Bind6 = &BsdSock.Bind6;
            _Connect4 = &BsdSock.Connect4;
            _Connect6 = &BsdSock.Connect6;
            _SetOption = &BsdSock.SetOption;
            _GetOption = &BsdSock.GetOption;
            _SetBlocking = &BsdSock.SetBlocking;
            _Poll = &BsdSock.Poll;
            _SendTo4 = &BsdSock.SendTo4;
            _SendTo6 = &BsdSock.SendTo6;
            _ReceiveFrom4 = &BsdSock.ReceiveFrom4;
            _ReceiveFrom6 = &BsdSock.ReceiveFrom6;
            _GetName4 = &BsdSock.GetName4;
            _GetName6 = &BsdSock.GetName6;
            _SetIP4 = &BsdSock.SetIP4;
            _SetIP6 = &BsdSock.SetIP6;
            _GetIP4 = &BsdSock.GetIP4;
            _GetIP6 = &BsdSock.GetIP6;
            _SetHostName4 = &BsdSock.SetHostName4;
            _SetHostName6 = &BsdSock.SetHostName6;
            _GetHostName4 = &BsdSock.GetHostName4;
            _GetHostName6 = &BsdSock.GetHostName6;
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
        public static SocketError SetDualMode6(nint socket, bool dualMode) => _SetDualMode6(socket, dualMode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind4(nint socket, sockaddr_in* socketAddress) => _Bind4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Bind6(nint socket, sockaddr_in6* socketAddress) => _Bind6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect4(nint socket, sockaddr_in* socketAddress) => _Connect4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Connect6(nint socket, sockaddr_in6* socketAddress) => _Connect6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int length = sizeof(int)) => _SetOption(socket, level, name, value, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, int* value, int* length = null) => _GetOption(socket, level, name, value, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetBlocking(nint socket, bool blocking) => _SetBlocking(socket, blocking);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError Poll(nint socket, int microseconds, SelectMode mode, out bool status) => _Poll(socket, microseconds, mode, out status);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo4(nint socket, void* buffer, int length, sockaddr_in* socketAddress) => _SendTo4(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo6(nint socket, void* buffer, int length, sockaddr_in6* socketAddress) => _SendTo6(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom4(nint socket, void* buffer, int length, sockaddr_in* socketAddress) => _ReceiveFrom4(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom6(nint socket, void* buffer, int length, sockaddr_in6* socketAddress) => _ReceiveFrom6(socket, buffer, length, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName4(nint socket, sockaddr_in* socketAddress) => _GetName4(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetName6(nint socket, sockaddr_in6* socketAddress) => _GetName6(socket, socketAddress);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP4(sockaddr_in* pAddrBuf, ReadOnlySpan<char> ip) => _SetIP4(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetIP6(sockaddr_in6* pAddrBuf, ReadOnlySpan<char> ip) => _SetIP6(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIP4(sockaddr_in* pAddrBuf, Span<byte> ip) => _GetIP4(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetIP6(sockaddr_in6* pAddrBuf, Span<byte> ip) => _GetIP6(pAddrBuf, ip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostName4(sockaddr_in* pAddrBuf, ReadOnlySpan<char> hostName) => _SetHostName4(pAddrBuf, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError SetHostName6(sockaddr_in6* pAddrBuf, ReadOnlySpan<char> hostName) => _SetHostName6(pAddrBuf, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName4(sockaddr_in* socketAddress, Span<byte> hostName) => _GetHostName4(socketAddress, hostName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetHostName6(sockaddr_in6* socketAddress, Span<byte> hostName) => _GetHostName6(socketAddress, hostName);
    }
}