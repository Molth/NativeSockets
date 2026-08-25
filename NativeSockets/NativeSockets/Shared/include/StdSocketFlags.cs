using System.Net.Sockets;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides standardized integer values for <see cref="SocketFlags" /> constants,
    ///     used as a common base for platform‑specific flag conversions.
    /// </summary>
    internal static class StdSocketFlags
    {
        /// <summary>
        ///     Managed flag for out‑of‑band data.
        /// </summary>
        public const int SOCKET_FLAGS_MSG_OOB = (int)SocketFlags.OutOfBand;

        /// <summary>
        ///     Managed flag for peeking at the message.
        /// </summary>
        public const int SOCKET_FLAGS_MSG_PEEK = (int)SocketFlags.Peek;

        /// <summary>
        ///     Managed flag for bypassing routing.
        /// </summary>
        public const int SOCKET_FLAGS_MSG_DONTROUTE = (int)SocketFlags.DontRoute;

        /// <summary>
        ///     Managed flag indicating the message was truncated.
        /// </summary>
        public const int SOCKET_FLAGS_MSG_TRUNC = (int)SocketFlags.Truncated;

        /// <summary>
        ///     Managed flag indicating control data was truncated.
        /// </summary>
        public const int SOCKET_FLAGS_MSG_CTRUNC = (int)SocketFlags.ControlDataTruncated;

        /// <summary>
        ///     Extended managed flag for non‑blocking operation.
        /// </summary>
        public const int SOCKET_FLAGS_MSG_DONTWAIT = 0x1000;

        /// <summary>
        ///     Extended managed flag for error queue.
        /// </summary>
        public const int SOCKET_FLAGS_MSG_ERRQUEUE = 0x2000;
    }
}