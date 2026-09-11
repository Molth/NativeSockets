#ifndef _SELECTMODEFLAGS_H
#define _SELECTMODEFLAGS_H

/// <summary>
///     Select mode flags for socket polling.
/// </summary>
typedef enum _SelectModeFlags
{
    _SELECT_MODE_FLAGS_NONE = 0,
    _SELECT_MODE_FLAGS_READ = 1,
    _SELECT_MODE_FLAGS_WRITE = 2,
    _SELECT_MODE_FLAGS_ERROR = 4
} _SelectModeFlags;

#endif
