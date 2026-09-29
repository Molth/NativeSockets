using System.Net.Sockets;
using static NativeSockets.StdSocketOptionName;

// ReSharper disable ALL

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
        /// <returns>
        ///     The native integer value for the socket option name, or -1 when the option is not supported
        ///     on FreeBSD. Returning -1 guarantees that the subsequent option call fails instead of passing
        ///     through an unmapped value that could behave unexpectedly.
        /// </returns>
        public static int ToNativeSocketOptionName(SocketOptionLevel level, SocketOptionName name) => level switch
        {
            SocketOptionLevel.Socket => name switch
            {
                SocketOptionName.Debug => 1,
                SocketOptionName.AcceptConnection => 2,
                SocketOptionName.ReuseAddress => 4,
                SocketOptionName.ExclusiveAddressUse => 4,
                SocketOptionName.KeepAlive => 8,
                SocketOptionName.DontRoute => 16,
                SocketOptionName.Broadcast => 32,
                SocketOptionName.UseLoopback => 64,
                SocketOptionName.Linger => 128,
                SocketOptionName.OutOfBandInline => 256,
                SocketOptionName.ReuseUnicastPort => 512,
                SocketOptionName.SendBuffer => 4097,
                SocketOptionName.ReceiveBuffer => 4098,
                SocketOptionName.SendLowWater => 4099,
                SocketOptionName.ReceiveLowWater => 4100,
                SocketOptionName.SendTimeout => 4101,
                SocketOptionName.ReceiveTimeout => 4102,
                SocketOptionName.Error => 4103,
                SocketOptionName.Type => 4104,
                _ => -1
            },
            SocketOptionLevel.IP => name switch
            {
                SocketOptionName.IPOptions => 1,
                SocketOptionName.HeaderIncluded => 2,
                SocketOptionName.TypeOfService => 3,
                SocketOptionName.IpTimeToLive => 4,
                SocketOptionName.MulticastInterface => 9,
                SocketOptionName.MulticastTimeToLive => 10,
                SocketOptionName.MulticastLoopback => 11,
                SocketOptionName.AddMembership => 12,
                SocketOptionName.DropMembership => 13,
                SocketOptionName.DontFragment => 67,
                SocketOptionName.AddSourceMembership => 70,
                SocketOptionName.DropSourceMembership => 71,
                SocketOptionName.BlockSource => 72,
                SocketOptionName.UnblockSource => 73,
                _ => -1
            },
            SocketOptionLevel.IPv6 => name switch
            {
                SocketOptionName.HopLimit => 4,
                SocketOptionName.MulticastInterface => 9,
                SocketOptionName.MulticastTimeToLive => 10,
                SocketOptionName.MulticastLoopback => 11,
                SocketOptionName.AddMembership => 12,
                SocketOptionName.DropMembership => 13,
                SocketOptionName.IPv6Only => 27,
                SocketOptionName.PacketInformation => 36,
                _ => -1
            },
            SocketOptionLevel.Tcp => name switch
            {
                SocketOptionName.NoDelay => 1,
                SO_TCP_KEEPALIVE_TIME => 256,
                SocketOptionName.BlockSource => 512,
                SocketOptionName.DontRoute => 1024,
                SocketOptionName.AddSourceMembership => 1025,
                _ => -1
            },
            SocketOptionLevel.Udp => -1,
            _ => -1
        };
    }
}