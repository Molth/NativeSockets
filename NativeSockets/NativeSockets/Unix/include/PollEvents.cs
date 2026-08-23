using System;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Event flags used with the <c>poll</c> system call on Unix-like systems.
    /// </summary>
    [Flags]
    internal enum PollEvents : short
    {
        /// <summary>
        ///     Data other than high-priority data is available to be read.
        /// </summary>
        POLLIN = 0x0001,

        /// <summary>
        ///     High-priority data (out-of-band) is available to be read.
        /// </summary>
        POLLPRI = 0x0002,

        /// <summary>
        ///     Data can be written without blocking.
        /// </summary>
        POLLOUT = 0x0004,

        /// <summary>
        ///     An error has occurred on the device or socket.
        /// </summary>
        POLLERR = 0x0008,

        /// <summary>
        ///     The device or socket has been disconnected (hang up).
        /// </summary>
        POLLHUP = 0x0010,

        /// <summary>
        ///     The file descriptor is not open or invalid.
        /// </summary>
        POLLNVAL = 0x0020
    }
}