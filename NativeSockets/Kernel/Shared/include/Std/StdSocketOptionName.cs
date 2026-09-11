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
        ///     The idle time (in seconds) before the first keepalive probe is sent.
        ///     Corresponds to the <c>TCP_KEEPIDLE</c> socket option on Linux/Unix.
        /// </summary>
        public const SocketOptionName SO_TCP_KEEPALIVE_TIME =
#if NET5_0_OR_GREATER
            SocketOptionName.TcpKeepAliveTime;
#else
            (SocketOptionName)3;
#endif
    }
}