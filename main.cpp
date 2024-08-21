#include <filesystem>
#include <iostream>

#include "src/tobey.h"

int main() {
    std::cout << "Hello, World!" << std::endl;
    tobey::run(std::filesystem::current_path().string());
    setvbuf (stdout, nullptr, _IONBF, 0);

    printf("Hello, World!\n");
    return 1;
}
