using System;
using System.Net.Sockets;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides native socket extension operations backed by <see cref="NativeSocket" />.
    /// </summary>
    internal static unsafe class SocketExtensionsNativeImpl
    {
        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="mode">The select mode.</param>
        /// <param name="status">When this method returns, contains true if the socket is ready, false otherwise.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError Poll(Socket socket, int microseconds, SelectMode mode, out bool status) => NativeVirtualSocketPal.Poll(new VirtualSocket(socket), microseconds, mode, out status);

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        public static SocketError PollFlags(Socket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags) => NativeVirtualSocketPal.PollFlags(new VirtualSocket(socket), microseconds, inFlags, out outFlags);

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static int SendTo(Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = NativeVirtualSocketPal.SendTo(new VirtualSocket(socket), buffer, socketFlags, socketAddress);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Receives data from a socket address, filling the provided socket address.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The receive buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static int ReceiveFrom(Socket socket, Span<byte> buffer, SocketFlags socketFlags, out NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = NativeVirtualSocketPal.ReceiveFrom(new VirtualSocket(socket), buffer, socketFlags, out socketAddress);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static int SendVectored(Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, out SocketError socketError)
        {
            IoResult result = NativeVirtualSocketPal.SendVectored(new VirtualSocket(socket), buffers, socketFlags);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Sends data from multiple buffers to a socket address.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        public static int SendToVectored(Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = NativeVirtualSocketPal.SendToVectored(new VirtualSocket(socket), buffers, socketFlags, socketAddress);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 If the data was truncated, returns <see cref="IoResult.Err" /> with
        ///                 <see cref="SocketError.MessageSize" />,
        ///                 even if the underlying operation succeeded.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static int ReceiveVectored(Socket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, out SocketError socketError)
        {
            IoResult result = NativeVirtualSocketPal.ReceiveVectored(new VirtualSocket(socket), buffers, socketFlags);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="PlatformNotSupportedException">
        ///     Thrown when <see cref="NativeSocketPal.IsSupported" /> is
        ///     <see langword="false" />.
        /// </exception>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///                 Any other flags are silently ignored.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 If the data was truncated, returns <see cref="IoResult.Err" /> with
        ///                 <see cref="SocketError.MessageSize" />,
        ///                 even if the underlying operation succeeded.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static int ReceiveFromVectored(Socket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, out NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = NativeVirtualSocketPal.ReceiveFromVectored(new VirtualSocket(socket), buffers, socketFlags, out socketAddress);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Gets the socket extensions implementation for this class.
        /// </summary>
        /// <returns>The <see cref="SocketExtensionsImpl" /> for the native implementation.</returns>
        public static SocketExtensionsImpl GetImpl()
        {
            SocketExtensionsImpl impl;
            impl.Poll = &Poll;
            impl.PollFlags = &PollFlags;
            impl.SendTo = &SendTo;
            impl.ReceiveFrom = &ReceiveFrom;
            impl.SendVectored = &SendVectored;
            impl.SendToVectored = &SendToVectored;
            impl.ReceiveVectored = &ReceiveVectored;
            impl.ReceiveFromVectored = &ReceiveFromVectored;
            return impl;
        }
    }
}