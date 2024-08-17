cmake_minimum_required(VERSION 3.29)
project(Tobey)

# Set the C++ standard
set(CMAKE_CXX_STANDARD 26)

# Glob all source files recursively
file(GLOB_RECURSE SOURCES "include/*.cpp" "include/*.h")

# Add the executable target
add_executable(Tobey main.cpp
        utils.h
        utils.cpp
        ${SOURCES}
        include/yaml/yaml.cpp
)