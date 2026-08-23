#pragma warning disable CS8981

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Represents a message header used with <c>sendmsg</c>
    ///     and <c>recvmsg</c> on Unix-like systems.
    /// </summary>
    internal unsafe struct msghdr
    {
        /// <summary>
        ///     Optional address of the destination or source socket.
        /// </summary>
        public void* msg_name;

        /// <summary>
        ///     Length of the address pointed to by <see cref="msg_name" />.
        /// </summary>
        public nuint msg_namelen;

        /// <summary>
        ///     Pointer to an array of <see cref="iovec" /> structures describing the scatter/gather buffers.
        /// </summary>
        public iovec* msg_iov;

        /// <summary>
        ///     Number of elements in the <see cref="msg_iov" /> array.
        /// </summary>
        public int msg_iovlen;

        /// <summary>
        ///     Pointer to ancillary data (control messages).
        /// </summary>
        public void* msg_control;

        /// <summary>
        ///     Length of the ancillary data buffer.
        /// </summary>
        public nuint msg_controllen;

        /// <summary>
        ///     Flags received or set for the message.
        /// </summary>
        public int msg_flags;
    }
}