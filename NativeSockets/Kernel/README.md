# NativeSockets

**A pure C# udp socket library with zero external dependencies.**

[![NuGet](https://img.shields.io/nuget/v/NativeSockets.svg?style=flat-square)](https://www.nuget.org/packages/NativeSockets/)

---

## About

- NativeSockets is a lightweight, cross‑platform library for udp networking. 
- It is written entirely in managed C# and does not rely on any third‑party binaries or platform‑specific packages. 
- The library provides a consistent API across all supported operating systems, with automatic runtime adaptation to the underlying environment.

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
- FreeBSD

---

## Key Features

- **Zero garbage collection pressure**
- **Pure managed code** – No external dependencies to deploy or manage.
- **Complete udp support** – create, bind, connect, send, receive, and poll sockets.
- **Ipv4 and Ipv6** with dual‑mode support.
- **Scatter/gather I/O** – Efficient vectored send and receive operations.

---

## No External Dependencies

- The library works out‑of‑the‑box on any supported platform. 
- There are no additional runtime libraries, native binaries, or platform‑specific packages to install – just add the NuGet package and start using it.

---

## VirtualSocket

A unified socket type that automatically picks the best available backend at runtime:

- When `NativeSocketPal.IsSupported` is `true`, `VirtualSocket` is backed by the native socket implementation.
- Otherwise, it transparently falls back to `System.Net.Sockets.Socket`.

### Key Points

- No manual `NativeSocketPal.Startup` / `NativeSocketPal.Cleanup` calls are required – the underlying socket subsystem is managed automatically.
- The send/receive API mirrors the native socket API and returns an `IoResult` (byte count plus `SocketError`).
- `SocketFlags` are restricted: send operations honor only `None` and `DontRoute`; receive operations honor only `None` and `Peek`. Any other flags are silently ignored.
- For vectored receive operations, truncated data is reported as `IoResult.Err` with `SocketError.MessageSize`, even when the underlying operation succeeded.

### Notes

- On the managed fallback path, a `SendTo` that triggers an implicit bind leaves the socket actually bound, but `LocalEndPoint` cannot be queried and throws. This only occurs on .NET 8 and later, because the `SendTo(ReadOnlySpan<byte>, SocketFlags, SocketAddress)` overload does not set the underlying `_rightEndPoint` field. In that state `GetName` reports an error even though the datagram was delivered.
- The managed fallback path carries the overhead of `System.Net.Sockets`; prefer the native backend when performance matters.
- When `NativeSocketPal.IsSupported` is `false` and the socket is non-blocking, consider polling with `Poll` or `PollFlags` before sending or receiving, because frequent `SocketError.WouldBlock` exceptions have a severe impact on performance.