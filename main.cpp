#include <filesystem>

#include "src/tobey.hpp"

int main(int argc, char * argv[]) {
    if (argc > 1 && std::string(argv[1]) == "--watch")
    {
        tobey::run(std::filesystem::current_path().string());
        tobey::watch(std::filesystem::current_path().string());
    }

    tobey::run(std::filesystem::current_path().string());

    return 0;
}
