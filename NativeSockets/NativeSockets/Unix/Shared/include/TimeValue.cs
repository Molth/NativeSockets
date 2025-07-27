using System.Runtime.InteropServices;

#pragma warning disable CS1591
#pragma warning disable CS8981

// ReSharper disable ALL

namespace NativeSockets
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TimeValue
    {
        public nint Seconds;
        public nint Microseconds;
    }
}