#include "../include/markdown/utils.hpp"
#include <fstream>
#include <string>
#include <vector>
#include <algorithm>

namespace utils
{
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

    void write_file(const std::string &output_path, const std::string &output) {
        std::ofstream outfile(output_path);
        outfile << output << std::endl;
        outfile.close();
    }

    std::string str_replace(const std::string &subject, const std::string &search, const std::string &replace) {
        size_t pos = subject.find(search);
        std::string mutable_subject = subject;

        mutable_subject.replace(pos, search.length(), replace);

        return mutable_subject;
    }
}