// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Represents the result of an I/O operation.
    /// </summary>
    public enum IoTag
    {
        /// <summary>
        ///     The operation succeeded.
        /// </summary>
        Ok = 1,

        /// <summary>
        ///     The operation failed.
        /// </summary>
        Err = 2
    }
}