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
                const auto relative_path = std::filesystem::relative(entry.path(), directory);
                files.push_back(relative_path.string());
            }
        }

        return files;
    }

    std::vector<std::string> get_layout_files(const std::string &directory) {
        std::vector<std::string> files;

        // if directory does not exist, attempt creating it
        if (!std::filesystem::exists(directory)) {
            std::filesystem::create_directory(directory);
        }

        for (const auto &entry : std::filesystem::directory_iterator(directory)) {
            if (entry.is_regular_file() && entry.path().extension() == ".html") {
                const auto relative_path = std::filesystem::relative(entry.path(), directory);
                files.push_back(relative_path.string());
            }
        }

        return files;
    }

    void run(const std::string& root_dir) {
        const auto dir_separator = std::string(1, std::filesystem::path::preferred_separator);
        const auto content_files = get_content_files(root_dir + dir_separator + "content");
        const auto layout_files = get_layout_files(root_dir + dir_separator + "layouts");
    }
}