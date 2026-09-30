using System.Runtime.InteropServices;
#if NET5_0_OR_GREATER
using System;
#endif

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Provides cross-platform operating system detection.
    /// </summary>
    internal static class OsName
    {
        /// <summary>
        ///     Gets a value indicating whether the current platform is Browser.
        /// </summary>
        /// <returns>true if the current platform is Browser; otherwise, false.</returns>
        public static bool IsBrowser() =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsBrowser();
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
#endif

        /// <summary>
        ///     Gets a value indicating whether the current platform is WASI.
        /// </summary>
        /// <returns>true if the current platform is WASI; otherwise, false.</returns>
        public static bool IsWasi() =>
#if NET8_0_OR_GREATER
            OperatingSystem.IsWasi();
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("WASI"));
#endif

        /// <summary>
        ///     Gets a value indicating whether the current platform is iOS.
        /// </summary>
        /// <returns>true if the current platform is iOS; otherwise, false.</returns>
        public static bool IsIos() =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsIOS() ||
            OperatingSystem.IsTvOS() ||
            OperatingSystem.IsWatchOS() ||
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST")) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("TVOS")) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("WATCHOS")) ||
#endif
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("VISIONOS"));

        /// <summary>
        ///     Gets a value indicating whether the current platform is Apple.
        /// </summary>
        /// <returns>true if the current platform is Apple; otherwise, false.</returns>
        public static bool IsApple() =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsMacOS() ||
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
#endif
            IsIos();

        /// <summary>
        ///     Gets a value indicating whether the current platform is FreeBSD.
        /// </summary>
        /// <returns>true if the current platform is FreeBSD; otherwise, false.</returns>
        public static bool IsFreeBsd() =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsFreeBSD();
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("FREEBSD"));
#endif

        /// <summary>
        ///     Gets a value indicating whether the current platform is Linux.
        /// </summary>
        /// <returns>true if the current platform is Linux; otherwise, false.</returns>
        public static bool IsLinux() =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsLinux() ||
            OperatingSystem.IsAndroid() ||
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")) ||
#endif
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("MAGICOS")) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("HARMONYOS"));

        /// <summary>
        ///     Gets a value indicating whether the current platform is Windows.
        /// </summary>
        /// <returns>true if the current platform is Windows; otherwise, false.</returns>
        public static bool IsWindows() =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsWindows();
#else
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
    }
}