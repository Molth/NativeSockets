#ifndef _NATIVESOCKETPAL_H
#define _NATIVESOCKETPAL_H

#include <stdint.h>
#include <stddef.h>

#ifdef __cplusplus
extern "C"
{
#endif

#ifdef _WIN32
#ifdef _NATIVESOCKETPAL_BUILD_DLL
#define _NATIVESOCKETPAL_API __declspec(dllexport)
#else
#define _NATIVESOCKETPAL_API __declspec(dllimport)
#endif
#else
#define _NATIVESOCKETPAL_API __attribute__((visibility("default")))
#endif

    typedef uint16_t u16;
    typedef intptr_t isize;
    typedef size_t usize;
    typedef int32_t i32;
    typedef uint32_t u32;
    typedef uint8_t u8;
    typedef int64_t i64;

/* SocketError */
#define _SOCKET_ERROR_SUCCESS 0
#define _SOCKET_ERROR_SOCKET_ERROR -1
#define _SOCKET_ERROR_INTERRUPTED 10004
#define _SOCKET_ERROR_ACCESS_DENIED 10013
#define _SOCKET_ERROR_FAULT 10014
#define _SOCKET_ERROR_INVALID_ARGUMENT 10022
#define _SOCKET_ERROR_TOO_MANY_OPEN_SOCKETS 10024
#define _SOCKET_ERROR_WOULD_BLOCK 10035
#define _SOCKET_ERROR_IN_PROGRESS 10036
#define _SOCKET_ERROR_ALREADY_IN_PROGRESS 10037
#define _SOCKET_ERROR_NOT_SOCKET 10038
#define _SOCKET_ERROR_DESTINATION_ADDRESS_REQUIRED 10039
#define _SOCKET_ERROR_MESSAGE_SIZE 10040
#define _SOCKET_ERROR_PROTOCOL_TYPE 10041
#define _SOCKET_ERROR_PROTOCOL_OPTION 10042
#define _SOCKET_ERROR_PROTOCOL_NOT_SUPPORTED 10043
#define _SOCKET_ERROR_SOCKET_NOT_SUPPORTED 10044
#define _SOCKET_ERROR_OPERATION_NOT_SUPPORTED 10045
#define _SOCKET_ERROR_ADDRESS_FAMILY_NOT_SUPPORTED 10047
#define _SOCKET_ERROR_ADDRESS_ALREADY_IN_USE 10048
#define _SOCKET_ERROR_ADDRESS_NOT_AVAILABLE 10049
#define _SOCKET_ERROR_NETWORK_DOWN 10050
#define _SOCKET_ERROR_NETWORK_UNREACHABLE 10051
#define _SOCKET_ERROR_NETWORK_RESET 10052
#define _SOCKET_ERROR_CONNECTION_ABORTED 10053
#define _SOCKET_ERROR_CONNECTION_RESET 10054
#define _SOCKET_ERROR_NO_BUFFER_SPACE_AVAILABLE 10055
#define _SOCKET_ERROR_IS_CONNECTED 10056
#define _SOCKET_ERROR_NOT_CONNECTED 10057
#define _SOCKET_ERROR_SHUTDOWN 10058
#define _SOCKET_ERROR_TIMED_OUT 10060
#define _SOCKET_ERROR_CONNECTION_REFUSED 10061
#define _SOCKET_ERROR_HOST_DOWN 10064
#define _SOCKET_ERROR_HOST_UNREACHABLE 10065
#define _SOCKET_ERROR_PROCESS_LIMIT 10067
#define _SOCKET_ERROR_SYSTEM_NOT_READY 10091
#define _SOCKET_ERROR_VERSION_NOT_SUPPORTED 10092
#define _SOCKET_ERROR_NOT_INITIALIZED 10093
#define _SOCKET_ERROR_DISCONNECTING 10101
#define _SOCKET_ERROR_TYPE_NOT_FOUND 10109
#define _SOCKET_ERROR_HOST_NOT_FOUND 11001
#define _SOCKET_ERROR_TRY_AGAIN 11002
#define _SOCKET_ERROR_NO_RECOVERY 11003
#define _SOCKET_ERROR_NO_DATA 11004

/* SocketFlags */
#define _SOCKET_FLAGS_NONE 0
#define _SOCKET_FLAGS_OUT_OF_BAND 1
#define _SOCKET_FLAGS_PEEK 2
#define _SOCKET_FLAGS_DONT_ROUTE 4
#define _SOCKET_FLAGS_TRUNCATED 256
#define _SOCKET_FLAGS_CONTROL_DATA_TRUNCATED 512
#define _SOCKET_FLAGS_BROADCAST 1024
#define _SOCKET_FLAGS_PARTIAL 32768

/* SelectMode */
#define _SELECT_MODE_SELECT_READ 0
#define _SELECT_MODE_SELECT_WRITE 1
#define _SELECT_MODE_SELECT_ERROR 2

/* SocketOptionLevel */
#define _SOCKET_OPTION_LEVEL_SOCKET 65535
#define _SOCKET_OPTION_LEVEL_IP 0
#define _SOCKET_OPTION_LEVEL_IPV6 41
#define _SOCKET_OPTION_LEVEL_TCP 6
#define _SOCKET_OPTION_LEVEL_UDP 17

/* SocketOptionName */
#define _SOCKET_OPTION_NAME_DEBUG 1
#define _SOCKET_OPTION_NAME_ACCEPT_CONNECTION 2
#define _SOCKET_OPTION_NAME_REUSE_ADDRESS 4
#define _SOCKET_OPTION_NAME_KEEP_ALIVE 8
#define _SOCKET_OPTION_NAME_DONT_ROUTE 16
#define _SOCKET_OPTION_NAME_BROADCAST 32
#define _SOCKET_OPTION_NAME_LINGER 128
#define _SOCKET_OPTION_NAME_OUT_OF_BAND_INLINE 256
#define _SOCKET_OPTION_NAME_SEND_BUFFER 4097
#define _SOCKET_OPTION_NAME_RECEIVE_BUFFER 4098
#define _SOCKET_OPTION_NAME_SEND_LOW_WATER 4099
#define _SOCKET_OPTION_NAME_RECEIVE_LOW_WATER 4100
#define _SOCKET_OPTION_NAME_SEND_TIMEOUT 4101
#define _SOCKET_OPTION_NAME_RECEIVE_TIMEOUT 4102
#define _SOCKET_OPTION_NAME_ERROR 4103
#define _SOCKET_OPTION_NAME_TYPE 4104
#define _SOCKET_OPTION_NAME_MAX_CONNECTIONS 2147483647
#define _SOCKET_OPTION_NAME_IP_OPTIONS 1
#define _SOCKET_OPTION_NAME_HEADER_INCLUDED 2
#define _SOCKET_OPTION_NAME_TYPE_OF_SERVICE 3
#define _SOCKET_OPTION_NAME_IP_TIME_TO_LIVE 4
#define _SOCKET_OPTION_NAME_MULTICAST_INTERFACE 9
#define _SOCKET_OPTION_NAME_MULTICAST_TIME_TO_LIVE 10
#define _SOCKET_OPTION_NAME_MULTICAST_LOOPBACK 11
#define _SOCKET_OPTION_NAME_ADD_MEMBERSHIP 12
#define _SOCKET_OPTION_NAME_DROP_MEMBERSHIP 13
#define _SOCKET_OPTION_NAME_DONT_FRAGMENT 14
#define _SOCKET_OPTION_NAME_ADD_SOURCE_MEMBERSHIP 15
#define _SOCKET_OPTION_NAME_DROP_SOURCE_MEMBERSHIP 16
#define _SOCKET_OPTION_NAME_BLOCK_SOURCE 17
#define _SOCKET_OPTION_NAME_UNBLOCK_SOURCE 18
#define _SOCKET_OPTION_NAME_PACKET_INFORMATION 19
#define _SOCKET_OPTION_NAME_NO_DELAY 1
#define _SOCKET_OPTION_NAME_BSD_COMPAT 14
#define _SOCKET_OPTION_NAME_EXPEDITED 2
#define _SOCKET_OPTION_NAME_IPV6_HOP_LIMIT 4
#define _SOCKET_OPTION_NAME_IPV6_PROTECTION_LEVEL 23
#define _SOCKET_OPTION_NAME_IPV6_V6ONLY 27

    /// <summary>
    ///     Represents a native Ipv4 socket address structure (<c>sockaddr_in</c>).
    /// </summary>
    /// <remarks>
    ///     This structure is used for Ipv4 socket operations and is
    ///     compatible with the native <c>sockaddr_in</c> on both Windows and Unix.
    ///     It contains the address family, port, Ipv4 address, and a zero‑padding field.
    /// </remarks>
    typedef struct _sockaddr_in4
    {
        /// <summary>
        ///     The address family (must be <see cref="AddressFamily.InterNetwork" />).
        /// </summary>
        u16 sin4_family;

        /// <summary>
        ///     The port number in network byte order.
        /// </summary>
        u16 sin4_port;

        /// <summary>
        ///     The Ipv4 address.
        /// </summary>
        u32 sin4_addr;

        /// <summary>
        ///     Padding to align the structure to the size of <c>sockaddr</c> (8 bytes of zeros).
        /// </summary>
        u8 sin4_zero[8];
    } _sockaddr_in4;

    /// <summary>
    ///     Represents a native Ipv6 socket address structure (<c>sockaddr_in6</c>).
    /// </summary>
    /// <remarks>
    ///     This structure is used for Ipv6 socket operations and matches the native layout of <c>sockaddr_in6</c>.
    ///     It includes the address family, port, flow information, the 128‑bit Ipv6 address, and a scope id.
    /// </remarks>
    typedef struct _sockaddr_in6
    {
        /// <summary>
        ///     The address family (must be <see cref="AddressFamily.InterNetworkV6" />).
        /// </summary>
        u16 sin6_family;

        /// <summary>
        ///     The port number in network byte order.
        /// </summary>
        u16 sin6_port;

        /// <summary>
        ///     The flow information (usually 0).
        /// </summary>
        u32 sin6_flowinfo;

        /// <summary>
        ///     The 128‑bit Ipv6 address as a 16‑byte array.
        /// </summary>
        u8 sin6_addr[16];

        /// <summary>
        ///     The scope id for link‑local or site‑local addresses.
        /// </summary>
        u32 sin6_scope_id;
    } _sockaddr_in6;

    /// <summary>
    ///     Represents a generic socket address storage structure that can hold any address family (<c>sockaddr_storage</c>).
    /// </summary>
    /// <remarks>
    ///     This structure is large enough to contain both Ipv4 and Ipv6 addresses, and is aligned to the most strict alignment
    ///     requirement of the system. It is used for functions that need to accept any address family without knowing the
    ///     exact type.
    /// </remarks>
    typedef struct _sockaddr_storage
    {
        /// <summary>
        ///     The address family of the stored address.
        /// </summary>
        u16 ss_family;

        /// <summary>
        ///     Padding.
        /// </summary>
        u8 __padding[6];

        /// <summary>
        ///     Alignment padding to ensure the structure is properly aligned in memory.
        /// </summary>
        i64 __ss_align;

        /// <summary>
        ///     Padding.
        /// </summary>
        u8 __ss_padding[112];
    } _sockaddr_storage;

    /// <summary>
    ///     Represents a contiguous region of arbitrary native memory.
    /// </summary>
    typedef struct _NativeIoSlice
    {
        /// <summary>
        ///     Represents a contiguous region of arbitrary memory.
        /// </summary>
        void *_buffer;

        /// <summary>
        ///     Gets the total numbers of elements the internal data structure can hold.
        /// </summary>
        i32 _length;
    } _NativeIoSlice;

/// <summary>
///     Read status mode.
/// </summary>
#define _SELECT_MODE_FLAGS_READ (1 << 0)

/// <summary>
///     Write status mode.
/// </summary>
#define _SELECT_MODE_FLAGS_WRITE (1 << 1)

/// <summary>
///     Error status mode.
/// </summary>
#define _SELECT_MODE_FLAGS_ERROR (1 << 2)

    /// <summary>
    ///     Gets the address family value for Ipv4 used by the current platform.
    /// </summary>
    _NATIVESOCKETPAL_API u16 _GetAddressFamilyInterNetworkV4(void);

    /// <summary>
    ///     Gets the address family value for Ipv6 used by the current platform.
    /// </summary>
    _NATIVESOCKETPAL_API u16 _GetAddressFamilyInterNetworkV6(void);

    /// <summary>
    ///     Retrieves the last socket error code from the underlying platform.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetLastSocketError(void);

    /// <summary>
    ///     Starts up the platform-specific socket subsystem (e.g., WSAStartup on Windows).
    /// </summary>
    _NATIVESOCKETPAL_API i32 _Startup(void);

    /// <summary>
    ///     Cleans up the platform-specific socket subsystem (e.g., WSACleanup on Windows).
    /// </summary>
    _NATIVESOCKETPAL_API i32 _Cleanup(void);

    /// <summary>
    ///     Creates a native socket handle for the specified address family (Ipv4 or Ipv6).
    /// </summary>
    _NATIVESOCKETPAL_API isize _Create(i32 ipv6);

    /// <summary>
    ///     Closes a native socket handle.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _Close(isize socket);

    /// <summary>
    ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SetDualModeIpv6(isize socket, i32 dualMode);

    /// <summary>
    ///     Binds a socket to an Ipv4 address.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _BindIpv4(isize socket, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Binds a socket to an Ipv6 address.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _BindIpv6(isize socket, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Connects a socket to an Ipv4 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ConnectIpv4(isize socket, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Connects a socket to an Ipv6 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ConnectIpv6(isize socket, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Sets a socket option.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SetOption(isize socket, i32 level, i32 name, u8 *value, i32 length);

    /// <summary>
    ///     Gets a socket option.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetOption(isize socket, i32 level, i32 name, u8 *value, i32 *length);

    /// <summary>
    ///     Sets a socket's blocking mode.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SetBlocking(isize socket, i32 blocking);

    /// <summary>
    ///     Polls a socket for pending events.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _Poll(isize socket, i32 microseconds, i32 mode, i32 *status);

    /// <summary>
    ///     Polls a socket for pending events.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _PollFlags(isize socket, i32 microseconds, i32 mode, i32 *status);

    /// <summary>
    ///     Sends data on a connected socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _Send(isize socket, void *buffer, i32 length, i32 socketFlags);

    /// <summary>
    ///     Sends data to an Ipv4 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendToIpv4(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Sends data to an Ipv6 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendToIpv6(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Receives data on a connected socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _Receive(isize socket, void *buffer, i32 length, i32 socketFlags);

    /// <summary>
    ///     Receives data from an Ipv4 endpoint, filling the provided address structure.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveFromIpv4(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Receives data from an Ipv6 endpoint, filling the provided address structure.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveFromIpv6(isize socket, void *buffer, i32 length, i32 socketFlags, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Sends a message on a connected socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendMessage(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags);

    /// <summary>
    ///     Sends a message to an Ipv4 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendMessageToIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Sends a message to an Ipv6 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendMessageToIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Receives a message on a connected socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveMessage(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *socketFlags);

    /// <summary>
    ///     Receives a message from an Ipv4 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveMessageFromIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *socketFlags, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Receives a message from an Ipv6 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveMessageFromIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *socketFlags, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Gets the local name (address) of an Ipv4 socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetNameIpv4(isize socket, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Gets the local name (address) of an Ipv6 socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetNameIpv6(isize socket, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Sets the Ipv4 address in the given address structure.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SetIpIpv4(_sockaddr_in4 *socketAddress, const u8 *ip, i32 ipLength);

    /// <summary>
    ///     Sets the Ipv6 address in the given address structure.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SetIpIpv6(_sockaddr_in6 *socketAddress, const u8 *ip, i32 ipLength);

    /// <summary>
    ///     Retrieves the Ipv4 address from a socket address structure.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetIpIpv4(_sockaddr_in4 *socketAddress, u8 *ip, i32 ipLength);

    /// <summary>
    ///     Retrieves the Ipv6 address from a socket address structure.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetIpIpv6(_sockaddr_in6 *socketAddress, u8 *ip, i32 ipLength);

    /// <summary>
    ///     Sets the host name (reverse DNS) for an Ipv4 address.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SetHostNameIpv4(_sockaddr_in4 *socketAddress, const u8 *hostName, i32 hostNameLength);

    /// <summary>
    ///     Sets the host name (reverse DNS) for an Ipv6 address.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SetHostNameIpv6(_sockaddr_in6 *socketAddress, const u8 *hostName, i32 hostNameLength);

    /// <summary>
    ///     Gets the host name (reverse DNS) from an Ipv4 address.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetHostNameIpv4(_sockaddr_in4 *socketAddress, u8 *hostName, i32 hostNameLength);

    /// <summary>
    ///     Gets the host name (reverse DNS) from an Ipv6 address.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _GetHostNameIpv6(_sockaddr_in6 *socketAddress, u8 *hostName, i32 hostNameLength);

#ifdef __cplusplus
}
#endif

#endif
