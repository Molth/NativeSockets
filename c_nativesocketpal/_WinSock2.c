#ifndef _WINSOCK2_C
#define _WINSOCK2_C

/// <summary>
///     Writes the 12‑byte prefix to an Ipv6 address.
/// </summary>
/// <param name="addr">The 12‑byte span containing the Ipv4‑mapped Ipv6 address data.</param>
static void _WriteIpv6Prefix(u8 *addr)
{
    memset(addr, 0, 10);
    addr[10] = 0xFF;
    addr[11] = 0xFF;
}

#endif
