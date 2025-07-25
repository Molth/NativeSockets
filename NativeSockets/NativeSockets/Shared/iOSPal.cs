using System.Net.Sockets;
using System.Runtime.InteropServices;
using winsock;

#pragma warning disable CS1591
#pragma warning disable CS8981
#pragma warning disable SYSLIB1054

// ReSharper disable ALL

namespace bsdsock
{
    internal static unsafe class iOSPal
    {
        private const string NATIVE_LIBRARY = "__Internal";

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getpid();

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern SocketError bind(nint __socketHandle_native, sockaddr* __socketAddress_native, int __socketAddressSize_native);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern SocketError getsockname(nint __socketHandle_native, sockaddr* __socketAddress_native, int* __socketAddressSize_native);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint socket(int af, int type, int protocol);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int fcntl(nint fd, int cmd, int arg);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern SocketError setsockopt(nint __socketHandle_native, SocketOptionLevel __optionLevel_native, SocketOptionName __optionName_native, int* __optionValue_native, int __optionLength_native);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern SocketError getsockopt(nint s, int level, int optname, byte* optval, int* optlen);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern SocketError connect(nint s, sockaddr* name, int namelen);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern SocketError close(nint __socketHandle_native);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sendto(nint __socketHandle_native, byte* __pinnedBuffer_native, int __len_native, SocketFlags __socketFlags_native, byte* __socketAddress_native, int __socketAddressSize_native);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int recvfrom(nint __socketHandle_native, byte* __pinnedBuffer_native, int __len_native, SocketFlags __socketFlags_native, byte* __socketAddress_native, int* __socketAddressSize_native);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int select(int __ignoredParameter_native, nint* __readfds_native, nint* __writefds_native, nint* __exceptfds_native, TimeValue* __timeout_native);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int inet_pton(int Family, void* pszAddrString, void* pAddrBuf);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getaddrinfo(byte* pNodeName, byte* pServiceName, addrinfo* pHints, addrinfo** ppResult);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern void freeaddrinfo(addrinfo* pAddrInfo);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* inet_ntop(int Family, void* pAddr, ref byte pStringBuf, nuint StringBufSize);

        [DllImport(NATIVE_LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getnameinfo(sockaddr* pSockaddr, int SockaddrLength, ref byte pNodeBuffer, ulong NodeBufferSize, byte* pServiceBuffer, ulong ServiceBufferSize, int Flags);
    }
}