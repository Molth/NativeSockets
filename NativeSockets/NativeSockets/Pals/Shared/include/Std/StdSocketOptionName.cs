using System.Net.Sockets;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides standardized socket option name constants for Linux/Unix TCP extensions.
    ///     These values correspond to platform-specific <c>TCP_*</c> constants (e.g., from <c>netinet/tcp.h</c>)
    ///     and are used as <see cref="SocketOptionName" /> values.
    /// </summary>
    internal static class StdSocketOptionName
    {
        /// <summary>
        ///     The maximum number of keepalive probes to send before declaring the connection dead.
        ///     Corresponds to the <c>TCP_KEEPCNT</c> socket option on Linux/Unix.
        /// </summary>
        public const SocketOptionName SO_TCP_KEEPALIVE_RETRYCOUNT = (SocketOptionName)16;

        /// <summary>
        ///     The idle time (in seconds) before the first keepalive probe is sent.
        ///     Corresponds to the <c>TCP_KEEPIDLE</c> socket option on Linux/Unix.
        /// </summary>
        public const SocketOptionName SO_TCP_KEEPALIVE_TIME = (SocketOptionName)3;

        /// <summary>
        ///     The interval (in seconds) between successive keepalive probes.
        ///     Corresponds to the <c>TCP_KEEPINTVL</c> socket option on Linux/Unix.
        /// </summary>
        public const SocketOptionName SO_TCP_KEEPALIVE_INTERVAL = (SocketOptionName)17;

        /// <summary>
        ///     Enables TCP Fast Open (TFO) on a socket.
        ///     Corresponds to the <c>TCP_FASTOPEN</c> socket option on Linux/Unix.
        /// </summary>
        public const SocketOptionName SO_TCP_FASTOPEN = (SocketOptionName)15;
    }
}