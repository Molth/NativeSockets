#if !NATIVE_SOCKETS_USE_BRIDGE
using System;

// ReSharper disable All

namespace NativeSockets
{
    /// <summary>
    ///     Indicates that the decorated parameter is expected to be a constant value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ConstantAttribute : Attribute
    {
    }
}
#endif