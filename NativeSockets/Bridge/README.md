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
- macOS
- iOS
- Android
- FreeBSD
- tvOS
- watchOS
- visionOS

---

## Key Features

- **Zero garbage collection pressure**
- **Same API as NativeSockets** – seamless switching between managed and native backends.
- **Complete UDP support** – create, bind, connect, send, receive, and poll sockets.
- **Ipv4 and Ipv6** with dual‑mode support.
- **Allocation‑free extension methods** for `System.Net.Sockets.Socket`:
  - `SendToNonAlloc` – send data without temporary allocations.
  - `ReceiveFromNonAlloc` – receive data and capture the remote endpoint without allocations.
- **Scatter/gather I/O** – Efficient vectored send and receive operations.
- **Host name resolution and reverse lookups** – Resolve names to addresses and back, all without allocations.

---

## Native Library Dependency

- The NuGet package includes the native C library for all supported platforms.
- The correct binary is loaded automatically at runtime – no extra installation or configuration is needed.
- Just add the package and use the same API as you would with the original NativeSockets.
- You can build binaries using [GitHub Actions](https://github.com/Molth/NativeSockets/actions).