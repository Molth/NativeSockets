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
        ///     Flag indicating a partial message.
        /// </summary>
        public const int SF_MSG_PARTIAL = (int)SocketFlags.Partial;
    }
}