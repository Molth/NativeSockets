using System.Net.Sockets;
using System.Runtime.CompilerServices;
using static NativeSockets.StdSocketFlags;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides conversion between managed <see cref="SocketFlags" /> and native Linux socket flag values.
    /// </summary>
    internal static class LinuxSocketFlags
    {
        /// <summary>
        ///     Flag for out‑of‑band data.
        /// </summary>
        private const int MSG_OOB = 0x0001;

        /// <summary>
        ///     Flag for peeking at the message.
        /// </summary>
        private const int MSG_PEEK = 0x0002;

        /// <summary>
        ///     Flag for bypassing routing.
        /// </summary>
        private const int MSG_DONTROUTE = 0x0004;

        /// <summary>
        ///     Flag indicating control data was truncated.
        /// </summary>
        private const int MSG_CTRUNC = 0x0008;

        /// <summary>
        ///     Flag indicating the message was truncated.
        /// </summary>
        private const int MSG_TRUNC = 0x0020;

        /// <summary>
        ///     Converts a managed <see cref="SocketFlags" /> value to its native Linux integer representation.
        /// </summary>
        /// <param name="stdFlags">The managed flags.</param>
        /// <returns>The native integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToNativeSocketFlags(SocketFlags stdFlags)
        {
            int flags = (int)stdFlags;

            int platformFlags = ((flags & SF_MSG_OOB) == 0 ? 0 : MSG_OOB)
                                | ((flags & SF_MSG_PEEK) == 0 ? 0 : MSG_PEEK)
                                | ((flags & SF_MSG_DONTROUTE) == 0 ? 0 : MSG_DONTROUTE)
                                | ((flags & SF_MSG_TRUNC) == 0 ? 0 : MSG_TRUNC)
                                | ((flags & SF_MSG_CTRUNC) == 0 ? 0 : MSG_CTRUNC);

            return platformFlags;
        }

        /// <summary>
        ///     Converts a native Linux socket flag integer value to a managed <see cref="SocketFlags" />.
        /// </summary>
        /// <param name="nativeFlags">The native integer value.</param>
        /// <returns>The managed <see cref="SocketFlags" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags FromNativeSocketFlags(int nativeFlags)
        {
            int result = ((nativeFlags & MSG_OOB) == 0 ? 0 : SF_MSG_OOB) |
                         ((nativeFlags & MSG_PEEK) == 0 ? 0 : SF_MSG_PEEK) |
                         ((nativeFlags & MSG_DONTROUTE) == 0 ? 0 : SF_MSG_DONTROUTE) |
                         ((nativeFlags & MSG_TRUNC) == 0 ? 0 : SF_MSG_TRUNC) |
                         ((nativeFlags & MSG_CTRUNC) == 0 ? 0 : SF_MSG_CTRUNC);

            return (SocketFlags)result;
        }
    }
}