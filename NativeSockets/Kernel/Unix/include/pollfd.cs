using System.Runtime.InteropServices;

#pragma warning disable CS8981

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a file descriptor to be monitored by
    ///     the <c>poll</c> system call on Unix-like systems.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct pollfd
    {
        /// <summary>
        ///     The file descriptor to monitor.
        /// </summary>
        public int fd;

        /// <summary>
        ///     The events of interest (bitmask of <see cref="PollEvents" /> values).
        /// </summary>
        public short events;

        /// <summary>
        ///     The events that occurred (filled by the <c>poll</c> call).
        /// </summary>
        public short revents;
    }
}