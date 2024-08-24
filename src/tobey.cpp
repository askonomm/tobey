#include <vector>
#include <filesystem>
#include <unordered_map>
#include <iostream>
#include <stack>

#include "tobey.h"
#include "utils.h"
#include "../include/frontmatter/frontmatter.h"
#include "../include/yaml/yaml.h"
#include "../libs/inja.hpp"

/**
 * Recursively traverses `directory` for any files matching `extension`,
 * and returns a vector of strings containing the full file paths.
 *
 * @param directory to search
 * @param extension file extension to search for
 * @return vector of strings containing full file paths
 */
std::vector<std::string> get_files(const std::string& directory, const std::string& extension)
{
    std::vector<std::string> files;
    std::stack<std::string> directories;

    // if directory does not exist, return early
    if (!std::filesystem::exists(directory))
    {
        return files;
    }

    // start with the root directory
    directories.push(directory);

    // traverse the stack of directories
    while (!directories.empty())
    {
        auto current_directory = directories.top();
        directories.pop();

        for (const auto& entry : std::filesystem::directory_iterator(current_directory))
        {
            if (entry.is_regular_file() && entry.path().extension() == extension)
            {
                files.push_back(entry.path().string());
            }
            else if (entry.is_directory())
            {
                directories.push(entry.path().string());
            }
        }
    }

    return files;
}

/**
 * Returns a vector of strings containing the full file paths of all
 * `.md` files in `directory`.
 *
 * @param directory to search
 * @return vector of strings containing full file paths
 */
std::vector<std::string> get_content_files(const std::string& directory)
{
    return get_files(directory, ".md");
}

/**
 *
 * @param directory directory to search
 * @return vector of strings containing full file paths
 */
std::vector<std::string> get_layout_files(const std::string& directory)
{
    return get_files(directory, ".html");
}

/**
 * Reads the content of each file in `files` and returns a vector of vectors
 * of yaml::Node.
 *
 * @param files vector of strings containing full file paths
 * @return vector of vectors of yaml::Node
 */
std::vector<std::vector<yaml::Node>> get_content(const std::string &root_dir, const std::vector<std::string>& files)
{
    std::vector<std::vector<yaml::Node>> content;

    for (const std::string& file : files)
    {
        const auto fm = frontmatter::parse(utils::read_file(file));
        auto nodes = std::get<0>(fm);
        const auto html = std::get<1>(fm);

        nodes.push_back(yaml::Node("html", html));
        nodes.push_back(yaml::Node("__file_path", file));

        // compose directory
        const auto dir_sep = std::string(1, std::filesystem::path::preferred_separator);
        auto directory = utils::str_replace(file, root_dir + dir_sep + "content", "");
        directory = directory.substr(0, directory.find_last_of('/'));

        if (!directory.empty())
        {
            directory = directory.substr(1);
        }

        nodes.push_back(yaml::Node("__directory", directory));

        // add data node
        content.push_back(nodes);
    }

    return content;
}

/**
 * Reads the content of each file in `files` and returns an unordered map
 * where the key is the layout name and the value is the layout content.
 *
 * @param files vector of strings containing full file paths
 * @return unordered map of strings containing the layout name and layout content
 */
std::unordered_map<std::string, std::string> get_layouts(const std::vector<std::string>& files)
{
    std::unordered_map<std::string, std::string> layouts;

    for (const std::string& file : files)
    {
        layouts[std::filesystem::path(file).filename().string()] = utils::read_file(file);
    }

    return layouts;
}

/**
 * Returns a vector of yaml::Node containing the data from `dsl_node` that
 * matches the data in `nodes`.
 *
 * @param dsl_node The entire DSL node `data` from the frontmatter
 * @param content Vector of vectors of yaml::Node containing all the data
 * @return
 */
std::vector<std::vector<yaml::Node>> dsl_get_data(const yaml::Node& dsl_node, const std::vector<std::vector<yaml::Node>>& content)
{
    std::vector<std::vector<yaml::Node>> content_matched;
    const auto dsl_node_nodes = std::get<std::vector<yaml::Node>>(dsl_node.value);

    // where clause
    if (const auto where_node = yaml::find_maybe_node(dsl_node_nodes, "where"))
    {
        const auto where_conditions = std::get<std::vector<yaml::Node>>((*where_node).value);
        const auto where_conditions_n = where_conditions.size();

        for (const auto &nodes : content)
        {
            auto conditions_matches = 0;

            for (const auto &condition : where_conditions)
            {
                const auto condition_value = std::get<std::string>(condition.value);
                const auto node = yaml::find_maybe_node(nodes, condition.key);

                // could not find node matching condition.key,
                // and thus cannot match the condition
                if (!node)
                {
                    continue;
                }

                // found a matching node, now check if the value matches
                // TODO: check that the value is a string
                if (std::get<std::string>((*node).value) == condition_value)
                {
                    conditions_matches++;
                }
            }

            if (conditions_matches == where_conditions_n)
            {
                content_matched.push_back(nodes);
            }
        }
    }

    return content_matched;
}

inja::json attach_dsl_data(const inja::json& data, const yaml::Node& data_node, const std::vector<std::vector<yaml::Node>>& content)
{
    const auto dsl_nodes = std::get<std::vector<yaml::Node>>(data_node.value);
    auto _data = data;

    // let's traverse each dsl node
    for (const auto& dsl_node : dsl_nodes)
    {
        _data[dsl_node.key] = inja::json::array({});

        // get results for the dsl node
        const auto dsl_data_content = dsl_get_data(dsl_node, content);

        // if no results, continue
        if (dsl_data_content.empty())
        {
            continue;
        }

        // add all results to the data node
        for(const auto& dsl_data_nodes : dsl_data_content)
        {
            inja::json child_data;

            for (const auto& child_node : dsl_data_nodes)
            {
                const auto key = child_node.key;
                const auto value = std::get<std::string>(child_node.value);

                child_data[key] = value;
            }

            _data[dsl_node.key].push_back(child_data);
        }
    }

    return _data;
}

namespace tobey
{
    /**
     * Runs the Tobey static site generator.
     *
     * @param root_dir the root directory of the project
     */
    void run(const std::string& root_dir)
    {
        const auto dir_sep = std::string(1, std::filesystem::path::preferred_separator);

        // compose content into a vector of vectors of yaml::Node
        const auto content_files = get_content_files(root_dir + dir_sep + "content");
        const auto content = get_content(root_dir, content_files);

        // compose layouts into an unordered map
        const auto layout_files = get_layout_files(root_dir + dir_sep + "layouts");
        const auto layouts = get_layouts(layout_files);

        // create output directory
        const auto output_dir = root_dir + dir_sep + "output";

        // write output
        for (const auto& nodes : content)
        {
            const auto layout = yaml::find_maybe_str(nodes, "layout");
            const auto slug = yaml::find_maybe_str(nodes, "slug");

            if (!layout)
            {
                throw std::runtime_error("\"layout\" not found in frontmatter");
            }

            if (!slug)
            {
                throw std::runtime_error("\"slug\" not found in frontmatter");
            }

            auto& output = layouts.at(*layout);
            inja::json data;

            // add frontmatter to data
            for (const auto& node : nodes)
            {
                // ignore data node
                if (node.key == "data")
                {
                    continue;
                }

                const auto key = node.key;
                const auto value = std::get<std::string>(node.value);

                data[key] = value;
            }

            // create dynamic data with a DSL
            if (const auto data_node = yaml::find_maybe_node(nodes, "data"))
            {
                data = attach_dsl_data(data, *data_node, content);
            }

            // file path
            const auto file_path = yaml::find_maybe_str(nodes, "__file_path");

            if (!file_path)
            {
                throw std::runtime_error("\"__file_path\" not found in frontmatter");
            }

            // compose output path
            const auto content_dir = root_dir + dir_sep + "content";
            const auto relative_path = utils::str_replace(*file_path, content_dir, "");
            auto relative_dir = std::filesystem::path(relative_path).parent_path().string();

            // set dir to empty if root
            if (relative_dir == "/")
            {
                relative_dir = "";
            }

            const auto output_path = std::string(output_dir)
                                     .append(relative_dir)
                                     .append(dir_sep)
                                     .append(*slug)
                                     .append(".html");

            // create parent dir if needed
            const auto parent_dir = std::filesystem::path(output_path).parent_path();

            if (!exists(parent_dir))
            {
                std::filesystem::create_directory(parent_dir);
            }

            // write to output
            std::cout << "Writing to " << output_path << std::endl;
            utils::write_file(output_path, inja::render(output, data));
        }
    }
}
