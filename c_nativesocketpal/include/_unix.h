#ifndef _UNIX_H
#define _UNIX_H

#ifndef _WIN32

#include <sys/socket.h>

#ifndef MSG_TRUNC
#define MSG_TRUNC 0x20
#endif

#ifndef MSG_CTRUNC
#define MSG_CTRUNC 0x08
#endif

typedef socklen_t _socklen_t;

#endif

#endif
