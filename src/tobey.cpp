#include <vector>
#include <filesystem>
#include <unordered_map>

#include "tobey.h"
#include "utils.h"
#include "../include/frontmatter/frontmatter.h"

namespace yaml {
    struct Node;
}

namespace tobey {
    std::vector<std::string> get_files(const std::string &directory, const std::string &extension) {
        std::vector<std::string> files;

        // if directory does not exist, attempt creating it
        if (!std::filesystem::exists(directory)) {
            std::filesystem::create_directory(directory);
        }

        for (const auto &entry : std::filesystem::directory_iterator(directory)) {
            if (entry.is_regular_file() && entry.path().extension() == extension) {
                const auto relative_path = std::filesystem::relative(entry.path(), directory);
                files.push_back(relative_path.string());
            }
        }

        return files;
    }

    std::vector<std::string> get_content_files(const std::string &directory) {
        return get_files(directory, ".md");
    }

    std::tuple<std::vector<yaml::Node>, std::string> compose_content(const std::string &content) {
        return frontmatter::parse(content);
    }

    std::vector<std::string> get_layout_files(const std::string &directory) {
        return get_files(directory, ".html");
    }

    void run(const std::string& root_dir) {
        const auto dir_separator = std::string(1, std::filesystem::path::preferred_separator);

        // compose content
        const auto content_files = get_content_files(root_dir + dir_separator + "content");
        std::vector<std::tuple<std::vector<yaml::Node>, std::string>> content;

        for(const std::string& file : content_files) {
            const auto path = std::string(root_dir)
                .append(dir_separator)
                .append("content")
                .append(dir_separator)
                .append(file);

            content.push_back(compose_content(read_file(path)));
        }

        // compose layout
        const auto layout_files = get_layout_files(root_dir + dir_separator + "layouts");
        std::unordered_map<std::string, std::string> layouts;

        for (const std::string& file : layout_files) {
            const auto path = std::string(root_dir)
                .append(dir_separator)
                .append("layouts")
                .append(dir_separator)
                .append(file);

            layouts[file] = read_file(path);
        }

        // create output directory
        const auto output_dir = std::string(root_dir)
            .append(dir_separator)
            .append("output");

        if (!std::filesystem::exists(output_dir)) {
            std::filesystem::create_directory(output_dir);
        }

        // write output
        for (const auto& [nodes, markdown] : content) {
            const auto layout = yaml::find_maybe_str(nodes, "layout");

            if (!layout) {
                throw std::runtime_error("Layout not found in frontmatter");
            }

            const auto layout_content = layouts[*layout];
            const auto output_path = std::string(output_dir)
                .append(dir_separator)
                .append(*yaml::find_maybe_str(nodes, "slug"))
                .append(".html");

            write_file(output_path, output);
        }
    }
}
