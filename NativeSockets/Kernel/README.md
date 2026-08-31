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
- **Pure managed code** – No external dependencies to deploy or manage.
- **Complete udp support** – create, bind, connect, send, receive, and poll sockets.
- **Ipv4 and Ipv6** with dual‑mode support.
- **Allocation‑free extension methods** for `System.Net.Sockets.Socket`:
    - `SendToNonAlloc` – send data without temporary allocations.
    - `ReceiveFromNonAlloc` – receive data and capture the remote endpoint without allocations.
- **Scatter/gather I/O** – Efficient vectored send and receive operations.
- **Host name resolution and reverse lookups** – Resolve names to addresses and back, all without allocations.

---

## No External Dependencies

- The library works out‑of‑the‑box on any supported platform. 
- There are no additional runtime libraries, native binaries, or platform‑specific packages to install – just add the NuGet package and start using it.