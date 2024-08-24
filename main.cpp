#include <filesystem>
#include <iostream>

#include "src/tobey.h"

int main() {
    //tobey::run(std::filesystem::current_path().string());
    tobey::run("/home/asko/Code/faultd.com");

    return 0;
}
