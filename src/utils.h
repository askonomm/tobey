#ifndef UTILS_H
#define UTILS_H

#include <string>

namespace utils
{
    std::string read_file(const std::string &filename);

    void write_file(const std::string &output_path, const std::string &output);

    std::string str_replace(const std::string &subject, const std::string &search, const std::string &replace);
}

#endif //UTILS_H
