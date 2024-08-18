#include "include/markdown/utils.h"
#include <fstream>
#include <string>
#include <vector>

std::string read_file(const std::string &filename) {
    std::ifstream file(filename);
    std::string contents;

    if (file) {
        file.seekg(0, std::ios::end);
        const std::streampos length = file.tellg();
        file.seekg(0, std::ios::beg);
        std::vector<char> buffer(length);
        file.read(&buffer[0], length);

        contents = std::string(buffer.begin(), buffer.end());
    }

    return str_trim(contents);
}