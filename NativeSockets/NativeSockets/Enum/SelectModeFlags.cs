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
        ///     Use no flags for this call.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Read status mode.
        /// </summary>
        SelectRead = 1,

        /// <summary>
        ///     Write status mode.
        /// </summary>
        SelectWrite = 2,

        /// <summary>
        ///     Error status mode.
        /// </summary>
        SelectError = 4
    }
}