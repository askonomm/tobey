#include <filesystem>
#include <iostream>

#include "src/tobey.h"

int main() {
    tobey::run(std::filesystem::current_path().string());

    return 0;
}
