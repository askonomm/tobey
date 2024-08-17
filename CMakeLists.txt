cmake_minimum_required(VERSION 3.29)
project(Tobey)

set(CMAKE_CXX_STANDARD 26)

file(GLOB_RECURSE SOURCES "include/*.cpp" "include/*.h")

add_executable(Tobey main.cpp
        utils.h
        utils.cpp
        ${SOURCES}
        include/yaml/yaml.cpp
)
