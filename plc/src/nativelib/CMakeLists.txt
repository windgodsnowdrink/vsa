cmake_minimum_required(VERSION 3.10)
project(greetings VERSION 1.0.0 LANGUAGES CXX)

# Set C++ standard
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Library target
add_library(greetings SHARED LoRaCrypto.cpp "aes.cpp" "out/aes.h" "cmac.cpp" "cmac.h" "utilities.cpp" "utilities.h")

# Installation rules
install(TARGETS greetings
    EXPORT greetingsTargets
    LIBRARY DESTINATION lib
    ARCHIVE DESTINATION lib
    RUNTIME DESTINATION bin
)
