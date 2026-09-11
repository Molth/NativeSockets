#ifndef _WIN32_H
#define _WIN32_H

#ifdef _WIN32

typedef i32 _socklen_t;

#define SIO_UDP_CONNRESET (IOC_IN | IOC_VENDOR | 12)

#endif

#endif
