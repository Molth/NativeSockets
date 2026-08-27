using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-specific error handling for sockets.
    /// </summary>
    internal static class FreeBsdSocketError
    {
        /// <summary>
        ///     Retrieves the last socket error code from the underlying platform.
        /// </summary>
        /// <returns>The last <see cref="SocketError" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketError GetLastError()
        {
            int errno = Marshal.GetLastWin32Error();
            return FromNative(errno);
        }

        /// <summary>
        ///     Maps a native error number to the corresponding <see cref="SocketError" /> value.
        /// </summary>
        /// <param name="errno">The native error number (errno).</param>
        /// <returns>The corresponding <see cref="SocketError" /> value.</returns>
        private static SocketError FromNative(int errno) => errno switch
        {
            0 => SocketError.Success,
            4 => SocketError.Interrupted,
            13 => SocketError.AccessDenied,
            14 => SocketError.Fault,
            22 => SocketError.InvalidArgument,
            23 => SocketError.TooManyOpenSockets,
            32 => SocketError.Shutdown,
            35 => SocketError.WouldBlock,
            36 => SocketError.InProgress,
            37 => SocketError.AlreadyInProgress,
            38 => SocketError.NotSocket,
            39 => SocketError.DestinationAddressRequired,
            40 => SocketError.MessageSize,
            41 => SocketError.ProtocolType,
            42 => SocketError.ProtocolOption,
            43 => SocketError.ProtocolNotSupported,
            44 => SocketError.SocketNotSupported,
            45 => SocketError.OperationNotSupported,
            46 => SocketError.ProtocolFamilyNotSupported,
            47 => SocketError.AddressFamilyNotSupported,
            48 => SocketError.AddressAlreadyInUse,
            49 => SocketError.AddressNotAvailable,
            50 => SocketError.NetworkDown,
            51 => SocketError.NetworkUnreachable,
            52 => SocketError.NetworkReset,
            53 => SocketError.ConnectionAborted,
            54 => SocketError.ConnectionReset,
            55 => SocketError.NoBufferSpaceAvailable,
            56 => SocketError.IsConnected,
            57 => SocketError.NotConnected,
            58 => SocketError.Disconnecting,
            60 => SocketError.TimedOut,
            61 => SocketError.ConnectionRefused,
            64 => SocketError.HostDown,
            65 => SocketError.HostUnreachable,
            85 => SocketError.OperationAborted,
            _ => SocketError.SocketError
        };
    }
}