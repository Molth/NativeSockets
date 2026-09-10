cmake_minimum_required(VERSION 4.0)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
project(nativesocketpal C)

set(CMAKE_C_STANDARD 11)
set(CMAKE_C_STANDARD_REQUIRED ON)

set(SOURCES
    _nativesocketpal.c
    _nativesocketpal.h
)

set(NATIVESOCKETPAL_STATIC "0" CACHE BOOL "Create a static library")
set(NATIVESOCKETPAL_SHARED "0" CACHE BOOL "Create a shared library")

if (MSYS OR MINGW)
    string(APPEND CMAKE_C_FLAGS " -static")
    add_definitions(-DWINVER=0x0601)
    add_definitions(-D_WIN32_WINNT=0x0601)
endif()

if (WIN32)
    set(NATIVESOCKETPAL_LIBS ws2_32)
endif()

if (NATIVESOCKETPAL_STATIC)
    add_library(nativesocketpal_static STATIC ${SOURCES})
    target_include_directories(nativesocketpal_static PUBLIC
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
        $<INSTALL_INTERFACE:include>
    )

    if (WIN32)
        target_link_libraries(nativesocketpal_static PUBLIC ${NATIVESOCKETPAL_LIBS})
    endif()

    set_target_properties(nativesocketpal_static PROPERTIES
        OUTPUT_NAME "nativesocketpal"
    )
endif()

if (NATIVESOCKETPAL_SHARED)
    add_library(nativesocketpal SHARED ${SOURCES})

    target_compile_definitions(nativesocketpal PRIVATE _NATIVESOCKETPAL_BUILD_DLL)

    target_include_directories(nativesocketpal PUBLIC
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
        $<INSTALL_INTERFACE:include>
    )

    if (WIN32)
        target_link_libraries(nativesocketpal PUBLIC ${NATIVESOCKETPAL_LIBS})
    endif()

    set_target_properties(nativesocketpal PROPERTIES
        OUTPUT_NAME "nativesocketpal"
    )
endif()

if (NATIVESOCKETPAL_SHARED)
    install(TARGETS nativesocketpal
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib
    )
endif()

if (NATIVESOCKETPAL_STATIC)
    install(TARGETS nativesocketpal_static
        ARCHIVE DESTINATION lib
    )
endif()

install(FILES _nativesocketpal.h DESTINATION include)