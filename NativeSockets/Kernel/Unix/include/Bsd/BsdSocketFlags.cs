using System.Net.Sockets;
using System.Runtime.CompilerServices;
using static NativeSockets.StdSocketFlags;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides conversion between managed <see cref="SocketFlags" /> and native bsd socket flag values.
    /// </summary>
    internal static class BsdSocketFlags
    {
        /// <summary>
        ///     Flag for peeking at the message.
        /// </summary>
        private const int MSG_PEEK = 0x0002;

        /// <summary>
        ///     Flag for bypassing routing.
        /// </summary>
        private const int MSG_DONTROUTE = 0x0004;

        /// <summary>
        ///     Flag indicating the message was truncated.
        /// </summary>
        private const int MSG_TRUNC = 0x0010;

        /// <summary>
        ///     Converts a managed <see cref="SocketFlags" /> value to
        ///     its native bsd integer representation for send operations.
        /// </summary>
        /// <param name="stdFlags">The managed flags.</param>
        /// <returns>The native integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToNativeSendSocketFlags(SocketFlags stdFlags) => ((int)stdFlags & SF_MSG_DONTROUTE) == 0 ? 0 : MSG_DONTROUTE;

        /// <summary>
        ///     Converts a managed <see cref="SocketFlags" /> value to
        ///     its native bsd integer representation for receive operations.
        /// </summary>
        /// <param name="stdFlags">The managed flags.</param>
        /// <returns>The native integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToNativeReceiveSocketFlags(SocketFlags stdFlags) => ((int)stdFlags & SF_MSG_PEEK) == 0 ? 0 : MSG_PEEK;

        /// <summary>
        ///     Converts a native bsd socket flag integer value to
        ///     a managed <see cref="SocketFlags" /> for receive operations.
        /// </summary>
        /// <param name="nativeFlags">The native integer value.</param>
        /// <returns>The managed <see cref="SocketFlags" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags FromNativeReceiveSocketFlags(int nativeFlags) => (SocketFlags)((nativeFlags & MSG_TRUNC) == 0 ? 0 : SF_MSG_TRUNC);
    }
}