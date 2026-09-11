#ifndef _NATIVESOCKETPAL_H
#define _NATIVESOCKETPAL_H

#include "_Types.h"

#ifdef __cplusplus
extern "C"
{
#endif

#ifdef _WIN32

#ifdef _NATIVESOCKETPAL_BUILD_DLL
#define _NATIVESOCKETPAL_API __declspec(dllexport)
#else
#define _NATIVESOCKETPAL_API
#endif

#else
#define _NATIVESOCKETPAL_API __attribute__((visibility("default")))
#endif

#include "_SocketError.h"
#include "_SelectMode.h"
#include "_SelectModeFlags.h"
#include "_SocketFlags.h"
#include "_SocketOptionLevel.h"
#include "_SocketOptionName.h"

#ifdef _WIN32
#include "_win32.h"
#else
#include "_unix.h"
#endif

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
    _NATIVESOCKETPAL_API i32 _PollFlags(isize socket, i32 microseconds, i32 inFlags, i32 *outFlags);

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
    ///     Sends data from multiple buffers on a connected socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags);

    /// <summary>
    ///     Sends data from multiple buffers to an Ipv4 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendToVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Sends data from multiple buffers to an Ipv6 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _SendToVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 socketFlags, _sockaddr_in6 *socketAddress);

    /// <summary>
    ///     Receives data into multiple buffers on a connected socket.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveVectored(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags);

    /// <summary>
    ///     Receives data into multiple buffers from an Ipv4 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveFromVectoredIpv4(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in4 *socketAddress);

    /// <summary>
    ///     Receives data into multiple buffers from an Ipv6 endpoint.
    /// </summary>
    _NATIVESOCKETPAL_API i32 _ReceiveFromVectoredIpv6(isize socket, _NativeIoSlice *buffers, i32 bufferCount, i32 *inOutFlags, _sockaddr_in6 *socketAddress);

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
