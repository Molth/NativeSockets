#pragma warning disable CS8981

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a scatter/gather buffer used
    ///     with sendmsg/recvmsg on Unix-like systems.
    /// </summary>
    internal readonly unsafe struct iovec
    {
        /// <summary>
        ///     Pointer to the data buffer.
        /// </summary>
        public readonly void* iov_base;

        /// <summary>
        ///     The length of the data buffer in bytes.
        /// </summary>
        public readonly nuint iov_len;

        /// <summary>
        ///     Initializes a new instance of the <see cref="iovec" /> structure.
        /// </summary>
        /// <param name="iovBase">Pointer to the data buffer.</param>
        /// <param name="iovLen">The length of the buffer.</param>
        public iovec(void* iovBase, nuint iovLen)
        {
            iov_base = iovBase;
            iov_len = iovLen;
        }
    }
}