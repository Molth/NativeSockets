// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a buffer used with Winsock scatter/gather
    ///     operations (WSASend, WSARecv, etc.).
    /// </summary>
    internal readonly unsafe struct WSABuffer
    {
        /// <summary>
        ///     The length of the buffer in bytes.
        /// </summary>
        public readonly nuint Length;

        /// <summary>
        ///     Pointer to the data buffer.
        /// </summary>
        public readonly void* Pointer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WSABuffer" /> structure.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        /// <param name="pointer">Pointer to the buffer data.</param>
        public WSABuffer(nuint length, void* pointer)
        {
            Length = length;
            Pointer = pointer;
        }
    }
}