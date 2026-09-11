#ifndef _SELECTMODE_H
#define _SELECTMODE_H

/// <summary>
///     Select modes for socket polling.
/// </summary>
typedef enum _SelectMode
{
    _SELECT_MODE_SELECT_READ = 0,
    _SELECT_MODE_SELECT_WRITE = 1,
    _SELECT_MODE_SELECT_ERROR = 2
} _SelectMode;

#endif
