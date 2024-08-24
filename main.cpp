#include <filesystem>

#include "src/tobey.hpp"

int main(int argc, char * argv[]) {
    // --watch?
    const auto watching = argc > 1 && std::string(argv[1]) == "--watch";

    if (watching)
    {
        tobey::watch(std::filesystem::current_path().string());
    }
    else
    {
        tobey::run(std::filesystem::current_path().string());
    }

    return 0;
}
