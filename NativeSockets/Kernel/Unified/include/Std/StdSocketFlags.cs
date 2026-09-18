using System.Net.Sockets;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides standardized integer values for <see cref="SocketFlags" /> constants,
    ///     used as a common base for platform‑specific flag conversions.
    /// </summary>
    internal static class StdSocketFlags
    {
        /// <summary>
        ///     Flag for out‑of‑band data.
        /// </summary>
        public const int SF_MSG_OOB = (int)SocketFlags.OutOfBand;

        /// <summary>
        ///     Flag for peeking at the message.
        /// </summary>
        public const int SF_MSG_PEEK = (int)SocketFlags.Peek;

        /// <summary>
        ///     Flag for bypassing routing.
        /// </summary>
        public const int SF_MSG_DONTROUTE = (int)SocketFlags.DontRoute;

        /// <summary>
        ///     Flag indicating the message was truncated.
        /// </summary>
        public const int SF_MSG_TRUNC = (int)SocketFlags.Truncated;

        /// <summary>
        ///     Flag indicating control data was truncated.
        /// </summary>
        public const int SF_MSG_CTRUNC = (int)SocketFlags.ControlDataTruncated;

        /// <summary>
        ///     Extended flag for non‑blocking operation.
        /// </summary>
        public const int SF_MSG_DONTWAIT = 0x1000;

        /// <summary>
        ///     Extended flag for error queue.
        /// </summary>
        public const int SF_MSG_ERRQUEUE = 0x2000;
    }
}