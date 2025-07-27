using System;

#pragma warning disable CS1591
#pragma warning disable CS8981

// ReSharper disable ALL

namespace NativeSockets
{
    public struct pollfd
    {
        public int fd;
        public short events;
        public short revents;
    }
}