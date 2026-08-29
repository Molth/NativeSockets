using System.Net.Sockets;
using static NativeSockets.StdSocketOptionName;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Provides conversion from managed socket option values to native Linux socket option values.
    /// </summary>
    internal static class LinuxSocketOption
    {
        /// <summary>
        ///     Converts a managed <see cref="SocketOptionLevel" /> to the native Linux socket option level value.
        /// </summary>
        /// <param name="level">The managed socket option level.</param>
        /// <returns>
        ///     The native integer value for the socket option level.
        ///     For <see cref="SocketOptionLevel.Socket" />, returns 1 (SOL_SOCKET).
        /// </returns>
        public static int ToNativeSocketOptionLevel(SocketOptionLevel level) => level == SocketOptionLevel.Socket ? 1 : (int)level;

        /// <summary>
        ///     Converts a managed <see cref="SocketOptionName" /> to the native Linux socket option name value
        ///     for the specified level, handling level‑specific mappings.
        /// </summary>
        /// <param name="level">The socket option level, which determines the namespace of the option name.</param>
        /// <param name="name">The managed socket option name.</param>
        /// <returns>The native integer value for the socket option name.</returns>
        public static int ToNativeSocketOptionName(SocketOptionLevel level, SocketOptionName name)
        {
            int result = level switch
            {
                SocketOptionLevel.Socket => name switch
                {
                    SocketOptionName.AcceptConnection => 30,
                    SocketOptionName.ReuseAddress => 2,
                    SocketOptionName.KeepAlive => 9,
                    SocketOptionName.DontRoute => 5,
                    SocketOptionName.Broadcast => 6,
                    SocketOptionName.Linger => 13,
                    SocketOptionName.OutOfBandInline => 10,
                    SocketOptionName.SendBuffer => 7,
                    SocketOptionName.ReceiveBuffer => 8,
                    SocketOptionName.SendLowWater => 19,
                    SocketOptionName.ReceiveLowWater => 18,
                    SocketOptionName.SendTimeout => 21,
                    SocketOptionName.ReceiveTimeout => 20,
                    SocketOptionName.Error => 4,
                    SocketOptionName.Type => 3,
                    _ => (int)name
                },
                SocketOptionLevel.IP => name switch
                {
                    SocketOptionName.IPOptions => 4,
                    SocketOptionName.HeaderIncluded => 3,
                    SocketOptionName.TypeOfService => 1,
                    SocketOptionName.IpTimeToLive => 2,
                    SocketOptionName.MulticastInterface => 32,
                    SocketOptionName.MulticastTimeToLive => 33,
                    SocketOptionName.MulticastLoopback => 34,
                    SocketOptionName.AddMembership => 35,
                    SocketOptionName.DropMembership => 36,
                    SocketOptionName.DontFragment => 10,
                    SocketOptionName.PacketInformation => 8,
                    SocketOptionName.AddSourceMembership => 39,
                    SocketOptionName.DropSourceMembership => 40,
                    SocketOptionName.BlockSource => 38,
                    SocketOptionName.UnblockSource => 37,
                    _ => (int)name
                },
                SocketOptionLevel.IPv6 => name switch
                {
                    SocketOptionName.IPv6Only => 26,
                    SocketOptionName.HopLimit => 16,
                    SocketOptionName.MulticastInterface => 17,
                    SocketOptionName.MulticastTimeToLive => 18,
                    SocketOptionName.MulticastLoopback => 19,
                    SocketOptionName.AddMembership => 20,
                    SocketOptionName.DropMembership => 21,
                    SocketOptionName.PacketInformation => 49,
                    SocketOptionName.IpTimeToLive => 16,
                    _ => (int)name
                },
                SocketOptionLevel.Tcp => name switch
                {
                    SocketOptionName.NoDelay => 1,
                    SO_TCP_KEEPALIVE_RETRYCOUNT => 6,
                    SO_TCP_KEEPALIVE_TIME => 4,
                    SO_TCP_KEEPALIVE_INTERVAL => 5,
                    SO_TCP_FASTOPEN => 23,
                    _ => (int)name
                },
                SocketOptionLevel.Udp => name switch
                {
                    SocketOptionName.NoChecksum => 101,
                    _ => (int)name
                },
                _ => (int)name
            };

            return result;
        }
    }
}