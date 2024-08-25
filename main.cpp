#include <filesystem>

#include "src/tobey.hpp"

int main(int argc, char * argv[]) {
    tobey::run(std::filesystem::current_path().string());

    if (argc > 1 && std::string(argv[1]) == "--watch")
    {
        tobey::watch(std::filesystem::current_path().string());
    }

    return 0;
}
