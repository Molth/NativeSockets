using System.Net.Sockets;
using System.Runtime.CompilerServices;
using static NativeSockets.StdSocketFlags;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides conversion between managed <see cref="SocketFlags" /> and native Windows socket flag values.
    /// </summary>
    internal static class WindowsSocketFlags
    {
        /// <summary>
        ///     Converts a managed <see cref="SocketFlags" /> value to
        ///     its native Windows integer representation for send operations.
        /// </summary>
        /// <param name="stdFlags">The managed flags.</param>
        /// <returns>The native integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags ToNativeSendSocketFlags(SocketFlags stdFlags) => stdFlags & (SocketFlags)SF_MSG_DONTROUTE;

        /// <summary>
        ///     Converts a managed <see cref="SocketFlags" /> value to
        ///     its native Windows integer representation for receive operations.
        /// </summary>
        /// <param name="stdFlags">The managed flags.</param>
        /// <returns>The native integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags ToNativeReceiveSocketFlags(SocketFlags stdFlags) => stdFlags & (SocketFlags)SF_MSG_PEEK;

        /// <summary>
        ///     Converts a native Windows socket flag integer value to
        ///     a managed <see cref="SocketFlags" /> for receive operations.
        /// </summary>
        /// <param name="nativeFlags">The native integer value.</param>
        /// <returns>The managed <see cref="SocketFlags" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags FromNativeReceiveSocketFlags(SocketFlags nativeFlags) => nativeFlags & (SocketFlags)(SF_MSG_PEEK | SF_MSG_TRUNC | SF_MSG_PARTIAL);
    }
}