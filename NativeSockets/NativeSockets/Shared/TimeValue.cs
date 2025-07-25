using System.Runtime.InteropServices;

#pragma warning disable CS1591
#pragma warning disable CS8981
#pragma warning disable SYSLIB1054

// ReSharper disable ALL

namespace winsock
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TimeValue
    {
        public int Seconds;
        public int Microseconds;
    }
}