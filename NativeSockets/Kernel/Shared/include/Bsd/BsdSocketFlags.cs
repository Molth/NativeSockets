using System.Net.Sockets;
using System.Runtime.CompilerServices;
using static NativeSockets.StdSocketFlags;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides conversion between managed <see cref="SocketFlags" /> and native bsd socket flag values.
    /// </summary>
    internal static class BsdSocketFlags
    {
        /// <summary>
        ///     Native flag indicating the message was truncated.
        /// </summary>
        private const int MSG_TRUNC = 0x0010;

        /// <summary>
        ///     Native flag indicating control data was truncated.
        /// </summary>
        private const int MSG_CTRUNC = 0x0020;

        /// <summary>
        ///     Bitmask of all managed <see cref="SocketFlags" /> values that are supported for conversion to native bsd flags.
        /// </summary>
        private const int SUPPORTED_MANAGED_FLAGS_MASK = 0
                                                         | SF_MSG_OOB
                                                         | SF_MSG_PEEK
                                                         | SF_MSG_DONTROUTE
                                                         | SF_MSG_TRUNC
                                                         | SF_MSG_CTRUNC;

        /// <summary>
        ///     Bitmask of all native bsd socket flag values that are supported for conversion back to managed flags.
        /// </summary>
        private const int SUPPORTED_NATIVE_FLAGS_MASK = SF_MSG_OOB | SF_MSG_DONTROUTE | MSG_TRUNC | MSG_CTRUNC;

        /// <summary>
        ///     Converts a managed <see cref="SocketFlags" /> value to its native bsd integer representation.
        /// </summary>
        /// <param name="palFlags">The managed flags.</param>
        /// <returns>The native integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToNativeSocketFlags(SocketFlags palFlags)
        {
            int flags = (int)palFlags;

            if ((flags & ~SUPPORTED_MANAGED_FLAGS_MASK) != 0)
                return 0;

            int platformFlags = (flags & SF_MSG_OOB)
                                | (flags & SF_MSG_PEEK)
                                | (flags & SF_MSG_DONTROUTE)
                                | ((flags & SF_MSG_TRUNC) == 0 ? 0 : MSG_TRUNC)
                                | ((flags & SF_MSG_CTRUNC) == 0 ? 0 : MSG_CTRUNC);

            return platformFlags;
        }

        /// <summary>
        ///     Converts a native bsd socket flag integer value to a managed <see cref="SocketFlags" />.
        /// </summary>
        /// <param name="platformFlags">The native integer value.</param>
        /// <returns>The managed <see cref="SocketFlags" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SocketFlags FromNativeSocketFlags(int platformFlags)
        {
            platformFlags &= SUPPORTED_NATIVE_FLAGS_MASK;

            int result = (platformFlags & SF_MSG_OOB) |
                         (platformFlags & SF_MSG_DONTROUTE) |
                         ((platformFlags & MSG_TRUNC) == 0 ? 0 : SF_MSG_TRUNC) |
                         ((platformFlags & MSG_CTRUNC) == 0 ? 0 : SF_MSG_CTRUNC);

            return (SocketFlags)result;
        }
    }
}