# NativeSockets2

**A P/Invoke wrapper over a native C UDP socket library, providing the same API as [NativeSockets](https://www.nuget.org/packages/NativeSockets) with a native backend.**

[![NuGet](https://img.shields.io/nuget/v/NativeSockets2.svg?style=flat-square)](https://www.nuget.org/packages/NativeSockets2)

---

## About

- NativeSockets2 is a lightweight, cross‑platform library for UDP networking.
- It is a P/Invoke wrapper around a native C library, delivering high performance through a native implementation.
- The library exposes the **exact same public API** as [NativeSockets](https://github.com/Molth/NativeSockets), making it a drop‑in replacement.
- Precompiled native binaries are included for all supported platforms and architectures.

---

## Supported Platforms

- Windows
- Linux
- Android
- macOS
- iOS
- tvOS
- watchOS
- visionOS

---

## Key Features

- **Zero garbage collection pressure**
- **Same API as NativeSockets** – seamless switching between managed and native backends.
- **Complete UDP support** – create, bind, connect, send, receive, and poll sockets.
- **Ipv4 and Ipv6** with dual‑mode support.
- **Scatter/gather I/O** – Efficient vectored send and receive operations.

---

## Native Library Dependency

- The NuGet package includes the native C library for all supported platforms.
- The correct binary is loaded automatically at runtime – no extra installation or configuration is needed.
- Just add the package and use the same API as you would with the original NativeSockets.
- You can build binaries using [GitHub Actions](https://github.com/Molth/NativeSockets/actions).

---

## VirtualSocket

A unified socket type that automatically picks the best available backend at runtime:

- When `NativeSocketPal.IsSupported` is `true`, `VirtualSocket` is backed by the native socket implementation.
- Otherwise it transparently falls back to `System.Net.Sockets.Socket`.

### Key Points

- No manual `NativeSocketPal.Startup` / `NativeSocketPal.Cleanup` calls are required – the underlying socket subsystem is managed automatically.
- The send/receive API mirrors the native socket API and returns an `IoResult` (byte count plus `SocketError`).
- `SocketFlags` are restricted: send operations honor only `None` and `DontRoute`; receive operations honor only `None` and `Peek`. Any other flags are silently ignored.
- For vectored receive operations, truncated data is reported as `IoResult.Err` with `SocketError.MessageSize`, even when the underlying operation succeeded.

### Notes

- On the managed fallback path, a `SendTo` that triggers an implicit bind leaves the socket actually bound, but `LocalEndPoint` cannot be queried and throws. This only occurs on .NET 8 and later, because the `SendTo(ReadOnlySpan<byte>, SocketFlags, SocketAddress)` overload does not set the underlying `_rightEndPoint` field. In that state `GetName` reports an error even though the datagram was delivered.
- The managed fallback path carries the overhead of `System.Net.Sockets`; prefer the native backend when performance matters.
- When `NativeSocketPal.IsSupported` is `false` and the socket is non-blocking, consider polling with `Poll` or `PollFlags` before sending or receiving, because frequent `SocketError.WouldBlock` exceptions have a severe impact on performance.