cmake_minimum_required(VERSION 3.29)
project(Tobey)

# Set the C++ standard
set(CMAKE_CXX_STANDARD 26)

# Glob all source files recursively
file(GLOB_RECURSE SOURCES "include/*.cpp" "include/*.h" "src/*.cpp" "src/*.h")

# Add the executable target
add_executable(Tobey main.cpp
        src/utils.h
        src/utils.cpp
        ${SOURCES}
)

# static linking
set(CMAKE_EXE_LINKER_FLAGS "-static-libgcc -static-libstdc++ -static")