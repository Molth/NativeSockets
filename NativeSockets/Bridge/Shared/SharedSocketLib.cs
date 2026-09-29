using System.Net.Sockets;
using System.Runtime.InteropServices;
using static NativeSockets.SharedNativeLibName;

#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time.

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-abstracted socket operations.
    /// </summary>
    internal static unsafe class SharedSocketLib
    {
        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_GetAddressFamilyInterNetworkV4", CallingConvention = CALLING_CONVENTION)]
        public static extern ushort GetAddressFamilyInterNetworkV4();

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_GetAddressFamilyInterNetworkV6", CallingConvention = CALLING_CONVENTION)]
        public static extern ushort GetAddressFamilyInterNetworkV6();

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_GetLastSocketError", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetLastSocketError();

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_Startup", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Startup();

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_Cleanup", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Cleanup();

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <param name="ipv6">Non-zero for Ipv6; 0 for Ipv4.</param>
        /// <param name="socket">
        ///     Pointer to a value that, when this method returns,
        ///     contains the native socket handle, or -1 on error.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_Create", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Create(int ipv6, nint* socket);

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <param name="socket">The native socket handle to close.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_Close", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Close(nint socket);

        /// <summary>
        ///     Binds a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_BindIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError BindIpv4(nint socket, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Binds a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_BindIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError BindIpv6(nint socket, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_ConnectIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError ConnectIpv4(nint socket, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Connects a socket to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_ConnectIpv6", CallingConvention = CALLING_CONVENTION)]
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
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///                 platform equivalents by the underlying socket layer.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 The <paramref name="value" /> bytes are passed through unmodified; the platform interprets the
        ///                 buffer according to the mapped option.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
        ///                 <paramref name="level" /> and <paramref name="name" /> is guaranteed.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SetOption", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetOption(nint socket, SocketOptionLevel level, SocketOptionName name, void* value, int length);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <param name="length">Pointer to the length of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 The <paramref name="level" /> and <paramref name="name" /> values are mapped to their native
        ///                 platform equivalents by the underlying socket layer.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 The <paramref name="value" /> buffer is passed through unmodified; the platform populates the
        ///                 buffer according to the mapped option.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
        ///                 <paramref name="level" /> and <paramref name="name" /> is guaranteed.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_GetOption", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetOption(nint socket, SocketOptionLevel level, SocketOptionName name, void* value, int* length);

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to the option value.</param>
        /// <param name="length">The length of the option value in bytes.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SetRawOption", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetRawOption(nint socket, int level, int name, void* value, int length);

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="level">The option level.</param>
        /// <param name="name">The option name.</param>
        /// <param name="value">Pointer to a buffer to receive the option value.</param>
        /// <param name="length">Pointer to the length of the buffer; on output, the actual size of the option.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_GetRawOption", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetRawOption(nint socket, int level, int name, void* value, int* length);

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="blocking">Non-zero for blocking; 0 for non-blocking.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SetBlocking", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError SetBlocking(nint socket, int blocking);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">
        ///     Pointer to a value that, when this method returns,
        ///     contains non-zero if the socket is ready, 0 otherwise.
        /// </param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_Poll", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError Poll(nint socket, int microseconds, SelectMode mode, int* status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">Pointer to a value that, when this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_PollFlags", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError PollFlags(nint socket, int microseconds, SelectModeFlags inFlags, SelectModeFlags* outFlags);

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv4 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv4 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_GetNameIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetNameIpv4(nint socket, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv6 socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="socketAddress">Pointer to the Ipv6 socket address to receive the name.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_GetNameIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern SocketError GetNameIpv6(nint socket, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer in bytes.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_Send", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult Send(nint socket, void* buffer, int length, SocketFlags socketFlags);

        /// <summary>
        ///     Sends data to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SendToIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult SendToIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Sends data to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the data buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SendToIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult SendToIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_Receive", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult Receive(nint socket, void* buffer, int length, SocketFlags socketFlags);

        /// <summary>
        ///     Receives data from an Ipv4 socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_ReceiveFromIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult ReceiveFromIpv4(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Receives data from an Ipv6 socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffer">Pointer to the receive buffer.</param>
        /// <param name="length">Length of the buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_ReceiveFromIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult ReceiveFromIpv6(nint socket, void* buffer, int length, SocketFlags socketFlags, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SendVectored", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult SendVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags);

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv4 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SendToVectoredIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult SendToVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">Pointer to the destination Ipv6 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_SendToVectoredIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult SendToVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags socketFlags, sockaddr_in6* socketAddress);

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">
        ///     Pointer to a value that, when this method returns,
        ///     contains the flags returned by the receive operation.
        /// </param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_ReceiveVectored", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult ReceiveVectored(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags);

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv4 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">
        ///     Pointer to a value that, when this method returns,
        ///     contains the flags returned by the receive operation.
        /// </param>
        /// <param name="socketAddress">Pointer to the sender's Ipv4 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_ReceiveFromVectoredIpv4", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult ReceiveFromVectoredIpv4(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in4* socketAddress);

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv6 socket address.
        /// </summary>
        /// <param name="socket">The socket handle.</param>
        /// <param name="buffers">Pointer to an array of <see cref="NativeIoSlice" />.</param>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="inOutFlags">
        ///     Pointer to a value that, when this method returns,
        ///     contains the flags returned by the receive operation.
        /// </param>
        /// <param name="socketAddress">Pointer to the sender's Ipv6 socket address.</param>
        /// <returns>An <see cref="IoResult" /> containing the number of bytes transferred and the socket error.</returns>
        /// <remarks>
        ///     Only the following flag values are honored:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.OutOfBand" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Peek" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.DontRoute" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.Truncated" />
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="SocketFlags.ControlDataTruncated" />
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Any other flags are silently ignored.
        /// </remarks>
        [DllImport(DLL_NAME_NATIVESOCKETPAL, EntryPoint = "_ReceiveFromVectoredIpv6", CallingConvention = CALLING_CONVENTION)]
        public static extern IoResult ReceiveFromVectoredIpv6(nint socket, NativeIoSlice* buffers, int bufferCount, SocketFlags* inOutFlags, sockaddr_in6* socketAddress);
    }
}