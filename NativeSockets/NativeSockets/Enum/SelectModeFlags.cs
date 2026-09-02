using System;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Defines the polling modes.
    /// </summary>
    [Flags]
    public enum SelectModeFlags
    {
        /// <summary>
        ///     Read status mode.
        /// </summary>
        SelectRead = 1 << 0,

        /// <summary>
        ///     Write status mode.
        /// </summary>
        SelectWrite = 1 << 1,

        /// <summary>
        ///     Error status mode.
        /// </summary>
        SelectError = 1 << 2
    }
}