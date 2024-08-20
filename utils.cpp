#include "include/markdown/utils.h"
#include <fstream>
#include <string>
#include <vector>
#include <algorithm>

std::string read_file(const std::string &filename) {
    std::ifstream file(filename, std::ios::binary);
    std::string contents;

    if (file) {
        file.seekg(0, std::ios::end);
        const std::streampos length = file.tellg();
        file.seekg(0, std::ios::beg);
        std::vector<char> buffer(length);
        file.read(&buffer[0], length);

        const auto null_pos = std::ranges::find(buffer, '\0');
        contents.assign(buffer.begin(), null_pos);
    }

    return contents;
}