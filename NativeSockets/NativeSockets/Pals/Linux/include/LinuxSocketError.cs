using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides platform-specific error handling for sockets.
    /// </summary>
    internal static class LinuxSocketError
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
            125 => SocketError.OperationAborted,
            115 => SocketError.IOPending,
            4 => SocketError.Interrupted,
            13 => SocketError.AccessDenied,
            14 => SocketError.Fault,
            22 => SocketError.InvalidArgument,
            23 => SocketError.TooManyOpenSockets,
            11 => SocketError.WouldBlock,
            114 => SocketError.AlreadyInProgress,
            88 => SocketError.NotSocket,
            89 => SocketError.DestinationAddressRequired,
            90 => SocketError.MessageSize,
            91 => SocketError.ProtocolType,
            92 => SocketError.ProtocolOption,
            93 => SocketError.ProtocolNotSupported,
            94 => SocketError.SocketNotSupported,
            95 => SocketError.OperationNotSupported,
            96 => SocketError.ProtocolFamilyNotSupported,
            97 => SocketError.AddressFamilyNotSupported,
            98 => SocketError.AddressAlreadyInUse,
            99 => SocketError.AddressNotAvailable,
            100 => SocketError.NetworkDown,
            101 => SocketError.NetworkUnreachable,
            102 => SocketError.NetworkReset,
            103 => SocketError.ConnectionAborted,
            104 => SocketError.ConnectionReset,
            105 => SocketError.NoBufferSpaceAvailable,
            106 => SocketError.IsConnected,
            107 => SocketError.NotConnected,
            32 => SocketError.Shutdown,
            110 => SocketError.TimedOut,
            111 => SocketError.ConnectionRefused,
            112 => SocketError.HostDown,
            113 => SocketError.HostUnreachable,
            10067 => SocketError.ProcessLimit,
            10091 => SocketError.SystemNotReady,
            10092 => SocketError.VersionNotSupported,
            10093 => SocketError.NotInitialized,
            108 => SocketError.Disconnecting,
            10109 => SocketError.TypeNotFound,
            -131073 => SocketError.HostNotFound,
            11003 => SocketError.NoRecovery,
            61 => SocketError.NoData,
            _ => SocketError.SocketError
        };
    }
}