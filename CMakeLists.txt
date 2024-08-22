cmake_minimum_required(VERSION 3.29)
project(tobey)

# Set the C++ standard
set(CMAKE_CXX_STANDARD 26)

# Glob all source files recursively
file(GLOB_RECURSE SOURCES "include/*.cpp" "include/*.h" "src/*.cpp" "src/*.h" "libs/*.hpp")

# Add the executable target
add_executable(tobey main.cpp ${SOURCES})

# static linking
set(CMAKE_EXE_LINKER_FLAGS "-static-libgcc -static-libstdc++ -static")