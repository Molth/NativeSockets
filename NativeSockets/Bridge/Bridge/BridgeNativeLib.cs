using System.Net.Sockets;
using System.Runtime.InteropServices;

#pragma warning disable SYSLIB1054

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations for sending and receiving data.
    /// </summary>
    internal static unsafe class BridgeNativeLib
    {
        /// <summary>
        ///     The name of the native library containing the socket functions.
        /// </summary>
        private const string NATIVE_LIBRARY = "nativesocketpal";

        /// <summary>
        ///     Indicates the calling convention of an entry point.
        /// </summary>
        private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetAddressFamilyInterNetworkV4", CallingConvention = CALLING_CONVENTION)]
        public static extern ushort GetAddressFamilyInterNetworkV4();

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetAddressFamilyInterNetworkV6", CallingConvention = CALLING_CONVENTION)]
        public static extern ushort GetAddressFamilyInterNetworkV6();

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetLastSocketError", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetLastSocketError();

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_Startup", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Startup();

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_Cleanup", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Cleanup();

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">true to create an Ipv6 socket; false for Ipv4.</param>
        /// <returns>The native socket handle, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_Create", CallingConvention = CALLING_CONVENTION)]
        public static extern nint Create(int ipv6);

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_Close", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Close(nint socket);

        /// <summary>
        ///     Enables or disables dual-mode (Ipv6/Ipv4) on an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="dualMode">true to enable dual-mode; false to disable.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SetDualModeIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetDualModeIpv6(nint socket, int dualMode);

        /// <summary>
        ///     Binds a socket to an Ipv4 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_BindIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Binds a socket to an Ipv6 address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_BindIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_ConnectIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_ConnectIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError ConnectIpv6(nint socket, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to the option value.</param>
        /// <param name="length">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SetOption", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int length);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <param name="length">Pointer to the length of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetOption", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, byte* value, int* length);

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">true for blocking; false for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SetBlocking", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetBlocking(nint socket, int blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_Poll", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Poll(nint socket, int microseconds, SelectMode mode, out int status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_PollFlags", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError PollFlags(nint socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer in bytes.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_Send", CallingConvention = CALLING_CONVENTION)]
        public static extern int Send(nint socket, void* buffer, int length, SocketFlags socketFlags);

        /// <summary>
        ///     Sends data to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SendToIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern int SendToIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Sends data to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address structure.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SendToIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern int SendToIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_Receive", CallingConvention = CALLING_CONVENTION)]
        public static extern int Receive(nint socket, void* buffer, int length, SocketFlags socketFlags);

        /// <summary>
        ///     Receives data from an Ipv4 endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_ReceiveFromIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern int ReceiveFromIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Receives data from an Ipv6 endpoint, filling the provided address structure.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 address structure.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_ReceiveFromIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern int ReceiveFromIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Sends a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SendMessage", CallingConvention = CALLING_CONVENTION)]
        public static extern int SendMessage(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags);

        /// <summary>
        ///     Sends a message to an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SendMessageToIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern int SendMessageToIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Sends a message to an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SendMessageToIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern int SendMessageToIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Receives a message on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_ReceiveMessage", CallingConvention = CALLING_CONVENTION)]
        public static extern int ReceiveMessage(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags);

        /// <summary>
        ///     Receives a message from an Ipv4 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_ReceiveMessageFromIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern int ReceiveMessageFromIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Receives a message from an Ipv6 endpoint.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" /> structures.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_ReceiveMessageFromIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern int ReceiveMessageFromIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* socketFlags, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Gets the local name (address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetNameIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Gets the local name (address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.SocketError" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetNameIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Sets the Ipv4 address in the given address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">The ip address as a span of bytes.</param>
        /// <param name="ipLength">Address bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SetIpIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetIpIpv4(sockaddr_in4* socketAddress, byte* ip, int ipLength);

        /// <summary>
        ///     Sets the Ipv6 address in the given address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="ip">The ip address as a span of bytes.</param>
        /// <param name="ipLength">Address bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> if successful; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SetIpIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetIpIpv6(sockaddr_in6* socketAddress, byte* ip, int ipLength);

        /// <summary>
        ///     Retrieves the Ipv4 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <param name="ipLength">Address bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetIpIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetIpIpv4(sockaddr_in4* socketAddress, byte* ip, int ipLength);

        /// <summary>
        ///     Retrieves the Ipv6 address from a socket address structure.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="ip">A span to receive the address bytes.</param>
        /// <param name="ipLength">Address bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetIpIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetIpIpv6(sockaddr_in6* socketAddress, byte* ip, int ipLength);

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">The host name as a span of bytes.</param>
        /// <param name="hostNameLength">Host name bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SetHostNameIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetHostNameIpv4(sockaddr_in4* socketAddress, byte* hostName, int hostNameLength);

        /// <summary>
        ///     Sets the host name (reverse DNS) for an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">The host name as a span of bytes.</param>
        /// <param name="hostNameLength">Host name bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_SetHostNameIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetHostNameIpv6(sockaddr_in6* socketAddress, byte* hostName, int hostNameLength);

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv4 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv4 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <param name="hostNameLength">Host name bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetHostNameIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetHostNameIpv4(sockaddr_in4* socketAddress, byte* hostName, int hostNameLength);

        /// <summary>
        ///     Gets the host name (reverse DNS) from an Ipv6 address.
        /// </summary>
        /// <param name="socketAddress">Pointer to the Ipv6 address structure.</param>
        /// <param name="hostName">A span to receive the host name bytes.</param>
        /// <param name="hostNameLength">Host name bytes length.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise <see cref="SocketError.Fault" />.</returns>
        [DllImport(NATIVE_LIBRARY, EntryPoint = "_GetHostNameIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetHostNameIpv6(sockaddr_in6* socketAddress, byte* hostName, int hostNameLength);
    }
}