using System.Runtime.InteropServices;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents a time value used with select and other functions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct TimeValue
    {
        /// <summary>
        ///     The number of seconds.
        /// </summary>
        public int Seconds;

        /// <summary>
        ///     The number of microseconds.
        /// </summary>
        public int Microseconds;
    }
}