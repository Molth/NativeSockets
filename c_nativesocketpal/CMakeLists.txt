cmake_minimum_required(VERSION 4.0)

project(nativesocketpal
    VERSION 1.0.0
    DESCRIPTION "Platform abstraction layer for native sockets"
    LANGUAGES C
)

set(CMAKE_C_STANDARD 11)
set(CMAKE_C_STANDARD_REQUIRED ON)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)

include(GNUInstallDirs)

option(NATIVESOCKETPAL_STATIC "Build the static library" OFF)
option(NATIVESOCKETPAL_SHARED "Build the shared library" OFF)

set(NATIVESOCKETPAL_SOURCES
    _nativesocketpal.c
)

set(NATIVESOCKETPAL_PUBLIC_HEADERS
    include/_Types.h
    include/_SocketError.h
    include/_SelectMode.h
    include/_SelectModeFlags.h
    include/_SocketFlags.h
    include/_SocketOptionLevel.h
    include/_SocketOptionName.h
    include/_nativesocketpal.h
    include/_win32.h
    include/_unix.h
)

if(WIN32)
    set(NATIVESOCKETPAL_PLATFORM_LIBS ws2_32)
endif()

function(nativesocketpal_configure target)
    target_include_directories(${target} PUBLIC
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/include>
        $<INSTALL_INTERFACE:${CMAKE_INSTALL_INCLUDEDIR}>
    )
    if(WIN32)
        target_link_libraries(${target} PUBLIC ${NATIVESOCKETPAL_PLATFORM_LIBS})
    endif()
    if(MSYS OR MINGW)
        target_compile_definitions(${target} PRIVATE
            WINVER=0x0601
            _WIN32_WINNT=0x0601
        )
        target_link_options(${target} PRIVATE -static)
    endif()
endfunction()

if(NATIVESOCKETPAL_STATIC)
    add_library(nativesocketpal_static STATIC ${NATIVESOCKETPAL_SOURCES} ${NATIVESOCKETPAL_PUBLIC_HEADERS})
    nativesocketpal_configure(nativesocketpal_static)
    if(WIN32)
        set_target_properties(nativesocketpal_static PROPERTIES OUTPUT_NAME nativesocketpal_static)
    else()
        set_target_properties(nativesocketpal_static PROPERTIES OUTPUT_NAME nativesocketpal)
    endif()
    set_target_properties(nativesocketpal_static PROPERTIES POSITION_INDEPENDENT_CODE ON)
endif()

if(NATIVESOCKETPAL_SHARED)
    add_library(nativesocketpal SHARED ${NATIVESOCKETPAL_SOURCES} ${NATIVESOCKETPAL_PUBLIC_HEADERS})
    nativesocketpal_configure(nativesocketpal)
    target_compile_definitions(nativesocketpal PRIVATE _NATIVESOCKETPAL_BUILD_DLL)
    set_target_properties(nativesocketpal PROPERTIES OUTPUT_NAME nativesocketpal)
endif()

install(FILES ${NATIVESOCKETPAL_PUBLIC_HEADERS} DESTINATION ${CMAKE_INSTALL_INCLUDEDIR})

if(NATIVESOCKETPAL_STATIC)
    install(TARGETS nativesocketpal_static ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR})
endif()

if(NATIVESOCKETPAL_SHARED)
    install(TARGETS nativesocketpal
        RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
        LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
        ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR}
    )
endif()
