#if !NET6_0_OR_GREATER
using System;
#endif
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using winsock;

#pragma warning disable CA1401
#pragma warning disable CS1591
#pragma warning disable CS8981
#pragma warning disable SYSLIB1054

// ReSharper disable ALL

namespace unixsock
{
    public static unsafe class UnixSock
    {
        private const string NATIVE_LIBRARY = "libc";

#if !NET6_0_OR_GREATER
        private static ReadOnlySpan<(int errno, SocketError error)> SocketErrors => new[]
        {
            (0, SocketError.Success),
            (1, SocketError.AccessDenied),
            (2, SocketError.AddressNotAvailable),
            (4, SocketError.Interrupted),
            (6, SocketError.HostNotFound),
            (7, SocketError.MessageSize),
            (9, SocketError.OperationAborted),
            (11, SocketError.WouldBlock),
            (12, SocketError.NoBufferSpaceAvailable),
            (13, SocketError.AccessDenied),
            (14, SocketError.Fault),
            (20, SocketError.InvalidArgument),
            (22, SocketError.InvalidArgument),
            (23, SocketError.TooManyOpenSockets),
            (24, SocketError.TooManyOpenSockets),
            (28, SocketError.NoBufferSpaceAvailable),
            (32, SocketError.Shutdown),
            (36, SocketError.InvalidArgument),
            (40, SocketError.AccessDenied),
            (59, SocketError.TooManyOpenSockets),
            (61, SocketError.NoData),
            (63, SocketError.NoBufferSpaceAvailable),
            (67, SocketError.NetworkUnreachable),
            (70, SocketError.ConnectionReset),
            (72, SocketError.NetworkUnreachable),
            (74, SocketError.InvalidArgument),
            (75, SocketError.MessageSize),
            (84, SocketError.InvalidArgument),
            (88, SocketError.NotSocket),
            (89, SocketError.DestinationAddressRequired),
            (90, SocketError.MessageSize),
            (91, SocketError.ProtocolType),
            (92, SocketError.ProtocolOption),
            (93, SocketError.ProtocolNotSupported),
            (94, SocketError.SocketNotSupported),
            (96, SocketError.ProtocolFamilyNotSupported),
            (97, SocketError.AddressFamilyNotSupported),
            (98, SocketError.AddressAlreadyInUse),
            (99, SocketError.AddressNotAvailable),
            (100, SocketError.NetworkDown),
            (101, SocketError.NetworkUnreachable),
            (102, SocketError.NetworkReset),
            (103, SocketError.ConnectionAborted),
            (104, SocketError.ConnectionReset),
            (105, SocketError.NoBufferSpaceAvailable),
            (106, SocketError.IsConnected),
            (107, SocketError.NotConnected),
            (108, SocketError.Disconnecting),
            (110, SocketError.TimedOut),
            (111, SocketError.ConnectionRefused),
            (112, SocketError.HostDown),
            (113, SocketError.HostUnreachable),
            (114, SocketError.AlreadyInProgress),
            (115, SocketError.InProgress)
        };
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastSocketError()
        {
#if NET6_0_OR_GREATER
            return (SocketError)Marshal.GetLastPInvokeError();
#else
            int errno = Marshal.GetLastWin32Error();
            int low = 0;
            int high = SocketErrors.Length - 1;
            ref (int errno, SocketError error) reference = ref MemoryMarshal.GetReference(SocketErrors);
            int index;
            while (low <= high)
            {
                int i = (int)(((uint)high + (uint)low) >> 1);
                int c = errno.CompareTo(Unsafe.Add(ref reference, i).errno);
                switch (c)
                {
                    case 0:
                        index = i;
                        goto label;
                    case > 0:
                        low = i + 1;
                        break;
                    default:
                        high = i - 1;
                        break;
                }
            }

            index = ~low;
            label:
            return index >= 0 ? SocketErrors[index].error : (SocketError)errno;
#endif
        }

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