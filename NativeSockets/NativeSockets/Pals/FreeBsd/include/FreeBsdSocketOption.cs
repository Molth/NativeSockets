using System.Net.Sockets;
using static NativeSockets.StdSocketOptionName;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides conversion from managed socket option values to native FreeBSD socket option values.
    /// </summary>
    internal static class FreeBsdSocketOption
    {
        /// <summary>
        ///     Converts a managed <see cref="SocketOptionLevel" /> to the native FreeBSD socket option level value.
        /// </summary>
        /// <param name="level">The managed socket option level.</param>
        /// <returns>
        ///     The native integer value for the socket option level.
        ///     On FreeBSD, <see cref="SocketOptionLevel.Socket" /> already equals SOL_SOCKET (0xffff),
        ///     so no conversion is needed.
        /// </returns>
        public static int ToNativeSocketOptionLevel(SocketOptionLevel level) => (int)level;

        /// <summary>
        ///     Converts a managed <see cref="SocketOptionName" /> to the native FreeBSD socket option name value
        ///     for the specified level, handling level‑specific mappings.
        /// </summary>
        /// <param name="level">The managed socket option level (must be the original managed enum value).</param>
        /// <param name="name">The managed socket option name.</param>
        /// <returns>The native integer value for the socket option name.</returns>
        public static int ToNativeSocketOptionName(SocketOptionLevel level, SocketOptionName name)
        {
            int result = level switch
            {
                SocketOptionLevel.IP => name switch
                {
                    SocketOptionName.DontFragment => 67,
                    SocketOptionName.PacketInformation => 39,
                    SocketOptionName.AddSourceMembership => 70,
                    SocketOptionName.DropSourceMembership => 71,
                    SocketOptionName.BlockSource => 72,
                    SocketOptionName.UnblockSource => 73,
                    _ => (int)name
                },
                SocketOptionLevel.IPv6 => name switch
                {
                    SocketOptionName.HopLimit => 4,
                    SocketOptionName.PacketInformation => 61,
                    _ => (int)name
                },
                SocketOptionLevel.Tcp => name switch
                {
                    SocketOptionName.NoDelay => 1,
                    SO_TCP_KEEPALIVE_RETRYCOUNT => 258,
                    SO_TCP_KEEPALIVE_TIME => 16,
                    SO_TCP_KEEPALIVE_INTERVAL => 257,
                    SO_TCP_FASTOPEN => 261,
                    _ => (int)name
                },
                _ => (int)name
            };

            return result;
        }
    }
}