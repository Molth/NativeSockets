using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides extension methods for <see cref="Socket" />.
    /// </summary>
    public static unsafe class SocketExtensions
    {
        /// <summary>
        ///     The socket extensions implementation selected during initialization.
        /// </summary>
        private static readonly SocketExtensionsImpl Impl;

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        static SocketExtensions() => Impl = NativeSocketPal.IsSupported ? SocketExtensionsNativeImpl.GetImpl() : SocketExtensionsManagedImpl.GetImpl();

        /// <summary>
        ///     Polls a socket for pending events.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="microseconds">The timeout in microseconds.</param>
        /// <param name="inFlags">The select mode.</param>
        /// <param name="outFlags">When this method returns, contains the poll result flags.</param>
        /// <returns><see cref="SocketError.Success" /> on success; otherwise an error code.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket" /> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError PollFlags(this Socket socket, int microseconds, SelectModeFlags inFlags, out SelectModeFlags outFlags)
        {
            ThrowHelpers.ThrowIfNull(socket, ExceptionArgument.socket);
            return Impl.PollFlags(socket, microseconds, inFlags, out outFlags);
        }

        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket" /> is null.</exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendTo(this Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress, out SocketError socketError)
        {
            ThrowHelpers.ThrowIfNull(socket, ExceptionArgument.socket);
            return Impl.SendTo(socket, buffer, socketFlags, socketAddress, out socketError);
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket" /> is null.</exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.Peek" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFrom(this Socket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress, out SocketError socketError)
        {
            ThrowHelpers.ThrowIfNull(socket, ExceptionArgument.socket);
            return Impl.ReceiveFrom(socket, buffer, socketFlags, ref socketAddress, out socketError);
        }

        /// <summary>
        ///     Sends data from multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket" /> is null.</exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendVectored(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, out SocketError socketError)
        {
            ThrowHelpers.ThrowIfNull(socket, ExceptionArgument.socket);
            return Impl.SendVectored(socket, buffers, socketFlags, out socketError);
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket" /> is null.</exception>
        /// <remarks>
        ///     Only the following flag value is honored: <see cref="SocketFlags.DontRoute" />.
        ///     Any other flags are silently ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SendToVectored(this Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress, out SocketError socketError)
        {
            ThrowHelpers.ThrowIfNull(socket, ExceptionArgument.socket);
            return Impl.SendToVectored(socket, buffers, socketFlags, socketAddress, out socketError);
        }

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket" /> is null.</exception>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveVectored(this Socket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, out SocketError socketError)
        {
            ThrowHelpers.ThrowIfNull(socket, ExceptionArgument.socket);
            return Impl.ReceiveVectored(socket, buffers, socketFlags, out socketError);
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket" /> is null.</exception>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReceiveFromVectored(this Socket socket, Span<NativeIoSlice> buffers, SocketFlags socketFlags, ref NativeSocketAddress socketAddress, out SocketError socketError)
        {
            ThrowHelpers.ThrowIfNull(socket, ExceptionArgument.socket);
            return Impl.ReceiveFromVectored(socket, buffers, socketFlags, ref socketAddress, out socketError);
        }
    }
}