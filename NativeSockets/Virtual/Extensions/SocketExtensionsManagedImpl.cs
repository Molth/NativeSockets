using System;
using System.Net.Sockets;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides managed socket extension operations backed by <see cref="Socket" />.
    /// </summary>
    internal static unsafe class SocketExtensionsManagedImpl
    {
        /// <summary>
        ///     Sends data to a socket address.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The data buffer.</param>
        /// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags" /> values.</param>
        /// <param name="socketAddress">The destination socket address.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes sent, or -1 on error.</returns>
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
        public static int SendTo(Socket socket, ReadOnlySpan<byte> buffer, SocketFlags socketFlags, in NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = ManagedVirtualSocketPalImpl.SendTo(socket, buffer, socketFlags, socketAddress);
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
        public static int ReceiveFrom(Socket socket, Span<byte> buffer, SocketFlags socketFlags, ref NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = ManagedVirtualSocketPalImpl.ReceiveFrom(socket, buffer, socketFlags, ref socketAddress);
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
        public static int SendVectored(Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, out SocketError socketError)
        {
            IoResult result = ManagedVirtualSocketPalImpl.SendVectored(socket, buffers, socketFlags);
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
        public static int SendToVectored(Socket socket, ReadOnlySpan<NativeIoSlice> buffers, SocketFlags socketFlags, in NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = ManagedVirtualSocketPalImpl.SendToVectored(socket, buffers, socketFlags, socketAddress);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Receives data into multiple buffers on a connected socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
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
        public static int ReceiveVectored(Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags inOutFlags, out SocketError socketError)
        {
            IoResult result = ManagedVirtualSocketPalImpl.ReceiveVectored(socket, buffers, ref inOutFlags);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Receives data into multiple buffers from a socket address.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffers">The array of <see cref="NativeIoSlice" />.</param>
        /// <param name="inOutFlags">When this method returns, contains the flags returned by the receive operation.</param>
        /// <param name="socketAddress">The sender's socket address.</param>
        /// <param name="socketError">When this method returns, contains the error code of the operation.</param>
        /// <returns>The number of bytes received, or -1 on error.</returns>
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
        public static int ReceiveFromVectored(Socket socket, Span<NativeIoSlice> buffers, ref SocketFlags inOutFlags, ref NativeSocketAddress socketAddress, out SocketError socketError)
        {
            IoResult result = ManagedVirtualSocketPalImpl.ReceiveFromVectored(socket, buffers, ref inOutFlags, ref socketAddress);
            socketError = result.SocketError;
            return result.BytesTransferred;
        }

        /// <summary>
        ///     Gets the socket extensions implementation for this class.
        /// </summary>
        /// <returns>The <see cref="SocketExtensionsImpl" /> for the managed implementation.</returns>
        public static SocketExtensionsImpl GetImpl()
        {
            SocketExtensionsImpl impl;
            impl.PollFlags = &ManagedVirtualSocketPalImpl.PollFlags;
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