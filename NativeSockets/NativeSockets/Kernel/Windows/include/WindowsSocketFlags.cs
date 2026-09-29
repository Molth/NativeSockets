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
        ///     Mask of the managed <see cref="SocketFlags" /> values that map directly to native Windows socket flags.
        /// </summary>
        private const SocketFlags SUPPORTED_FLAGS_MASK = (SocketFlags)(SF_MSG_OOB | SF_MSG_PEEK | SF_MSG_DONTROUTE | SF_MSG_TRUNC | SF_MSG_CTRUNC);

        /// <summary>
        ///     Converts a managed <see cref="SocketFlags" /> value to its native Windows integer representation.
        /// </summary>
        /// <param name="stdFlags">The managed flags.</param>
        /// <returns>The native integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags ToNativeSocketFlags(SocketFlags stdFlags)
        {
            SocketFlags platformFlags = stdFlags;

            platformFlags &= SUPPORTED_FLAGS_MASK;

            return platformFlags;
        }

        /// <summary>
        ///     Converts a native Windows socket flag integer value to a managed <see cref="SocketFlags" />.
        /// </summary>
        /// <param name="nativeFlags">The native integer value.</param>
        /// <returns>The managed <see cref="SocketFlags" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags FromNativeSocketFlags(SocketFlags nativeFlags)
        {
            nativeFlags &= SUPPORTED_FLAGS_MASK;

            return nativeFlags;
        }
    }
}