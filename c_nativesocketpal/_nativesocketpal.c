#include "include/_nativesocketpal.h"

#include <string.h>
#include <stdlib.h>

#ifdef _WIN32

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>

#else

#include <sys/socket.h>
#include <sys/uio.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <poll.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include <errno.h>

#endif

#ifdef _WIN32

#include "_win32.c"

#else

#include "_SelectMode.c"
#include "_SelectModeFlags.c"
#include "_SocketError.c"
#include "_SocketFlags.c"
#include "_SocketOptionLevel.c"
#include "_SocketOptionName.c"

#include "_unix.c"

#endif
