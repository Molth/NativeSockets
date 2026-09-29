using System.Net.Sockets;
using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides the platform-specific socket operations as a single struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SocketPalImpl
    {
        /// <summary>
        ///     Gets the address family value for Ipv4 used by the current platform.
        /// </summary>
        public ushort ADDRESS_FAMILY_INTER_NETWORK_V4;

        /// <summary>
        ///     Gets the address family value for Ipv6 used by the current platform.
        /// </summary>
        public ushort ADDRESS_FAMILY_INTER_NETWORK_V6;

        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        public delegate* managed<SocketError> GetLastSocketError;

        /// <summary>
        ///     Starts up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<SocketError> Startup;

        /// <summary>
        ///     Cleans up the platform-specific socket subsystem.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<SocketError> Cleanup;

        /// <summary>
        ///     Creates a native socket handle.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<bool, out nint, SocketError> Create;

        /// <summary>
        ///     Closes a native socket handle.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, SocketError> Close;

        /// <summary>
        ///     Binds a socket to an Ipv4 socket address.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, sockaddr_in4*, SocketError> BindIpv4;

        /// <summary>
        ///     Binds a socket to an Ipv6 socket address.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, sockaddr_in6*, SocketError> BindIpv6;

        /// <summary>
        ///     Connects a socket to an Ipv4 socket address.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, sockaddr_in4*, SocketError> ConnectIpv4;

        /// <summary>
        ///     Connects a socket to an Ipv6 socket address.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, sockaddr_in6*, SocketError> ConnectIpv6;

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 The level and name values are mapped to their native
        ///                 platform equivalents by the underlying socket layer.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 The value bytes are passed through unmodified; the platform interprets the
        ///                 buffer according to the mapped option.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
        ///                 level and name is guaranteed.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public delegate* managed<nint, SocketOptionLevel, SocketOptionName, void*, int, SocketError> SetOption;

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 The level and name values are mapped to their native
        ///                 platform equivalents by the underlying socket layer.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 The value buffer is passed through unmodified; the platform populates the
        ///                 buffer according to the mapped option.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Behavior is not guaranteed to be consistent across platforms; only the mapping of
        ///                 level and name is guaranteed.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public delegate* managed<nint, SocketOptionLevel, SocketOptionName, void*, int*, SocketError> GetOption;

        /// <summary>
        ///     Sets a socket option.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, int, int, void*, int, SocketError> SetRawOption;

        /// <summary>
        ///     Gets a socket option.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, int, int, void*, int*, SocketError> GetRawOption;

        /// <summary>
        ///     Sets a socket's blocking mode.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, bool, SocketError> SetBlocking;

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, int, SelectMode, out bool, SocketError> Poll;

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, int, SelectModeFlags, out SelectModeFlags, SocketError> PollFlags;

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv4 socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, sockaddr_in4*, SocketError> GetNameIpv4;

        /// <summary>
        ///     Gets the local name (socket address) of an Ipv6 socket.
        /// </summary>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        public delegate* managed<nint, sockaddr_in6*, SocketError> GetNameIpv6;

        /// <summary>
        ///     Sends data on a connected socket.
        /// </summary>
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
        public delegate* managed<nint, void*, int, SocketFlags, IoResult> Send;

        /// <summary>
        ///     Sends data to an Ipv4 socket address.
        /// </summary>
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
        public delegate* managed<nint, void*, int, SocketFlags, sockaddr_in4*, IoResult> SendToIpv4;

        /// <summary>
        ///     Sends data to an Ipv6 socket address.
        /// </summary>
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
        public delegate* managed<nint, void*, int, SocketFlags, sockaddr_in6*, IoResult> SendToIpv6;

        /// <summary>
        ///     Receives data on a connected socket.
        /// </summary>
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
        public delegate* managed<nint, void*, int, SocketFlags, IoResult> Receive;

        /// <summary>
        ///     Receives data from an Ipv4 socket address, filling the provided socket address.
        /// </summary>
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
        public delegate* managed<nint, void*, int, SocketFlags, sockaddr_in4*, IoResult> ReceiveFromIpv4;

        /// <summary>
        ///     Receives data from an Ipv6 socket address, filling the provided socket address.
        /// </summary>
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
        public delegate* managed<nint, void*, int, SocketFlags, sockaddr_in6*, IoResult> ReceiveFromIpv6;

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
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
        public delegate* managed<nint, NativeIoSlice*, int, SocketFlags, IoResult> SendVectored;

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv4 socket address.
        /// </summary>
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
        public delegate* managed<nint, NativeIoSlice*, int, SocketFlags, sockaddr_in4*, IoResult> SendToVectoredIpv4;

        /// <summary>
        ///     Sends data from multiple buffers to an Ipv6 socket address.
        /// </summary>
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
        public delegate* managed<nint, NativeIoSlice*, int, SocketFlags, sockaddr_in6*, IoResult> SendToVectoredIpv6;

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
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
        public delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, IoResult> ReceiveVectored;

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv4 socket address.
        /// </summary>
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
        public delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, sockaddr_in4*, IoResult> ReceiveFromVectoredIpv4;

        /// <summary>
        ///     Receives data into multiple buffers from an Ipv6 socket address.
        /// </summary>
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
        public delegate* managed<nint, NativeIoSlice*, int, SocketFlags*, sockaddr_in6*, IoResult> ReceiveFromVectoredIpv6;
    }
}