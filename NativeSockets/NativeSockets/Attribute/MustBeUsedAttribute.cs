using System;

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Indicates that the value returned by,
    ///     or written to an <see langword="out" /> parameter of,
    ///     a method must be checked by the caller,
    ///     and must not be ignored or discarded.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    public sealed class MustBeUsedAttribute : Attribute
    {
    }
}