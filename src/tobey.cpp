#include <iostream>
#include <vector>
#include <filesystem>

#include "tobey.h"

namespace tobey {
    std::vector<std::string> get_content_files(const std::string &directory) {
        std::vector<std::string> files;

        // if directory does not exist, attempt creating it
        if (!std::filesystem::exists(directory)) {
            std::filesystem::create_directory(directory);
        }

        for (const auto &entry : std::filesystem::directory_iterator(directory)) {
            if (entry.is_regular_file() && entry.path().extension() == ".md") {
                files.push_back(entry.path().string());
            }
        }

        return files;
    }

    void run(const std::string& root_dir) {
        std::cout << "Hello, Tobey!" << std::endl;
        std::cout << "Root directory: " << root_dir << std::endl;

        const auto dir_separator = std::string(1, std::filesystem::path::preferred_separator);
        const auto content_dir = root_dir + dir_separator + "content";
        std::vector<std::string> files = get_content_files(content_dir);

        for (const auto &file : files) {
            std::cout << file << std::endl;
        }
    }
}