#include <filesystem>
#include <iostream>
#include <stack>
#include <thread>
#include <unordered_map>
#include <utility>
#include <vector>

#include "../include/frontmatter/frontmatter.hpp"
#include "../include/yaml/yaml.h"
#include "../libs/inja.hpp"
#include "templating.hpp"
#include "utils.hpp"

/**
 * Recursively traverses `directory` for any files matching `extension`,
 * and returns a vector of strings containing the full file paths.
 *
 * @param directory to search
 * @param extension file extension to search for
 * @return vector of strings containing full file paths
 */
std::vector<std::string> get_files(const std::string &directory,
                                   const std::string &extension) {
  std::vector<std::string> files;
  std::stack<std::string> directories;

  // if directory does not exist, return early
  if (!std::filesystem::exists(directory)) {
    return files;
  }

  // start with the root directory
  directories.push(directory);

  // traverse the stack of directories
  while (!directories.empty()) {
    auto current_directory = directories.top();
    directories.pop();

    for (const auto &entry :
         std::filesystem::directory_iterator(current_directory)) {
      if (entry.is_regular_file() && entry.path().extension() == extension) {
        files.push_back(entry.path().string());
      } else if (entry.is_directory()) {
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
std::vector<std::string> get_content_files(const std::string &directory) {
  return get_files(directory, ".md");
}

/**
 *
 * @param directory directory to search
 * @return vector of strings containing full file paths
 */
std::vector<std::string> get_layout_files(const std::string &directory) {
  return get_files(directory, ".html");
}

/**
 * Reads the content of each file in `files` and returns a vector of vectors
 * of yaml::Node.
 *
 * @param files vector of strings containing full file paths
 * @return vector of vectors of yaml::Node
 */
std::vector<std::vector<yaml::Node>>
get_content(const std::string &root_dir,
            const std::vector<std::string> &files) {
  std::vector<std::vector<yaml::Node>> content;

  for (const std::string &file : files) {
    const auto fm = frontmatter::parse(utils::read_file(file));
    auto nodes = std::get<0>(fm);
    const auto html = std::get<1>(fm);

    nodes.push_back(yaml::Node("html", html));
    nodes.push_back(yaml::Node("__file_path", file));

    // compose directory
    const auto dir_sep =
        std::string(1, std::filesystem::path::preferred_separator);
    auto directory =
        utils::str_replace(file, root_dir + dir_sep + "content", "");
    directory = directory.substr(0, directory.find_last_of('/'));

    if (!directory.empty()) {
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
 * @return unordered map of strings containing the layout name and layout
 * content
 */
std::unordered_map<std::string, std::string>
get_layouts(const std::vector<std::string> &files) {
  std::unordered_map<std::string, std::string> layouts;

  for (const std::string &file : files) {
    layouts[std::filesystem::path(file).filename().string()] =
        utils::read_file(file);
  }

  return layouts;
}

/**
 * Filters the content based on the `where` clause in the DSL node.
 *
 * @param dsl_node The entire DSL node `where` from the frontmatter
 * @param content Vector of vectors of yaml::Node containing all the data
 * @return Vector of vectors of yaml::Node containing the data that matches the
 * `where` clause
 */
std::vector<std::vector<yaml::Node>>
dsl_where_clause(const yaml::Node &dsl_node,
                 const std::vector<std::vector<yaml::Node>> &content) {
  const auto &where_conditions =
      std::get<std::vector<yaml::Node>>(dsl_node.value);

  auto matches_conditions = [&](const std::vector<yaml::Node> &nodes) {
    for (const auto &condition : where_conditions) {
      const auto condition_value = std::get<std::string>(condition.value);
      const auto node = yaml::find_maybe_str(nodes, condition.key);

      if (!node || *node != condition_value) {
        return false;
      }
    }

    return true;
  };

  std::vector<std::vector<yaml::Node>> _content_filtered;
  std::ranges::copy_if(content, std::back_inserter(_content_filtered),
                       matches_conditions);

  return _content_filtered;
}

/**
 * Sorts the content based on the `sort` clause in the DSL node.
 *
 * @param dsl_node The entire DSL node `sort` from the frontmatter
 * @param content Vector of vectors of yaml::Node containing all the data
 * @return Vector of vectors of yaml::Node containing the data sorted by the
 * `sort` clause
 */
std::vector<std::vector<yaml::Node>>
dsl_sort_clause(const yaml::Node &dsl_node,
                const std::vector<std::vector<yaml::Node>> &content) {
  auto _content_filtered = content;
  const auto sort_child_nodes =
      std::get<std::vector<yaml::Node>>(dsl_node.value);
  const auto sort_by = yaml::find_maybe_str(sort_child_nodes, "by");
  const auto sort_order = yaml::find_maybe_str(sort_child_nodes, "order");

  if (sort_by && sort_order) {
    std::ranges::sort(_content_filtered,
                      [sort_by, sort_order](auto &a, auto &b) {
                        const auto a_node = yaml::find_maybe_str(a, *sort_by);
                        const auto b_node = yaml::find_maybe_str(b, *sort_by);

                        if (!a_node || !b_node) {
                          return false;
                        }

                        if (*sort_order == "asc") {
                          return *a_node < *b_node;
                        }

                        if (*sort_order == "desc") {
                          return *a_node > *b_node;
                        }

                        return false;
                      });
  }

  return _content_filtered;
}

/**
 * Returns a vector of yaml::Node containing the data from `dsl_node` that
 * matches the data in `nodes`.
 *
 * @param dsl_node The entire DSL node `data` from the frontmatter
 * @param content Vector of vectors of yaml::Node containing all the data
 * @return
 */
std::vector<std::vector<yaml::Node>>
dsl_get_data(const yaml::Node &dsl_node,
             const std::vector<std::vector<yaml::Node>> &content) {
  std::vector<std::vector<yaml::Node>> content_filtered = content;
  const auto dsl_node_nodes = std::get<std::vector<yaml::Node>>(dsl_node.value);

  // where clause
  if (const auto where_node = yaml::find_maybe_node(dsl_node_nodes, "where")) {
    content_filtered = dsl_where_clause(*where_node, content_filtered);
  }

  // sort clause
  if (const auto sort_node = yaml::find_maybe_node(dsl_node_nodes, "sort")) {
    content_filtered = dsl_sort_clause(*sort_node, content_filtered);
  }

  return content_filtered;
}

/**
 *
 * @param data The json data to attach the DSL data to
 * @param data_node The DSL node from the frontmatter
 * @param content Vector of vectors of yaml::Node containing all the data
 * @return Updated json data with the DSL data attached
 */
inja::json
attach_dsl_data(const inja::json &data, const yaml::Node &data_node,
                const std::vector<std::vector<yaml::Node>> &content) {
  const auto dsl_nodes = std::get<std::vector<yaml::Node>>(data_node.value);
  auto _data = data;

  // let's traverse each dsl node
  for (const auto &dsl_node : dsl_nodes) {
    _data[dsl_node.key] = inja::json::array({});

    // get results for the dsl node
    const auto dsl_data_content = dsl_get_data(dsl_node, content);

    // if no results, continue
    if (dsl_data_content.empty()) {
      continue;
    }

    // add all results to the data node
    for (const auto &dsl_data_nodes : dsl_data_content) {
      inja::json child_data;

      for (const auto &child_node : dsl_data_nodes) {
        const auto key = child_node.key;
        const auto value = std::get<std::string>(child_node.value);

        child_data[key] = value;
      }

      _data[dsl_node.key].push_back(child_data);
    }
  }

  return _data;
}

namespace tobey {
/**
 * Runs the Tobey static site generator.
 *
 * @param root_dir the root directory of the project
 */
void run(const std::string &root_dir) {
  const auto dir_sep =
      std::string(1, std::filesystem::path::preferred_separator);

  // compose content into a vector of vectors of yaml::Node
  const auto content_files = get_content_files(root_dir + dir_sep + "content");
  const auto content = get_content(root_dir, content_files);

  // compose layouts into an unordered map
  const auto layout_files = get_layout_files(root_dir + dir_sep + "layouts");
  const auto layouts = get_layouts(layout_files);

  // delete output directory if it exists
  const auto output_root_dir = root_dir + dir_sep + "output";

  if (std::filesystem::exists(output_root_dir)) {
    std::filesystem::remove_all(output_root_dir);
  }

  // write output
  for (const auto &nodes : content) {
    const auto layout = yaml::find_maybe_str(nodes, "layout");
    const auto slug = yaml::find_maybe_str(nodes, "slug");

    if (!layout) {
      throw std::runtime_error("\"layout\" not found in frontmatter");
    }

    if (!slug) {
      throw std::runtime_error("\"slug\" not found in frontmatter");
    }

    auto &output = layouts.at(*layout);
    inja::json data;

    // add frontmatter to data
    for (const auto &node : nodes) {
      // ignore data node
      if (node.key == "data") {
        continue;
      }

      const auto key = node.key;
      const auto value = std::get<std::string>(node.value);

      data[key] = value;
    }

    // create dynamic data with a DSL
    if (const auto data_node = yaml::find_maybe_node(nodes, "data")) {
      data = attach_dsl_data(data, *data_node, content);
    }

    // file path
    const auto file_path = yaml::find_maybe_str(nodes, "__file_path");

    if (!file_path) {
      throw std::runtime_error("\"__file_path\" not found in frontmatter");
    }

    // compose output path
    const auto content_dir = root_dir + dir_sep + "content";
    const auto relative_path = utils::str_replace(*file_path, content_dir, "");
    auto relative_dir =
        std::filesystem::path(relative_path).parent_path().string();

    // set dir to empty if root
    if (relative_dir == "/") {
      relative_dir = "";
    }

    // Inja environment
    inja::Environment env;
    env.add_callback("format_date", 2, templating::format_date);

    // If the slug has a dot in it, we don't want to use it as a directory,
    // so that we could allow for root level files to be created, as well as
    // things like XML feeds, etc.
    if (std::string(*slug).find('.') != std::string::npos) {
      const std::string output_dir = std::string(output_root_dir)
                                         .append(relative_dir)
                                         .append(dir_sep)
                                         .append(*slug);

      const auto output_dir_parent =
          std::filesystem::path(output_dir).parent_path().string();
      create_directories(std::filesystem::path(output_dir_parent));
      std::cout << "Writing to " << output_dir << std::endl;
      utils::write_file(output_dir, env.render(output, data));
    }
    // otherwise slug is a directory that we create an index.html in.
    else {
      const std::string output_dir = std::string(output_root_dir)
                                         .append(relative_dir)
                                         .append(dir_sep)
                                         .append(*slug)
                                         .append(dir_sep);

      create_directories(std::filesystem::path(output_dir));
      std::cout << "Writing to " << output_dir + "index.html" << std::endl;
      utils::write_file(output_dir + "index.html", env.render(output, data));
    }
  }
}

bool skip_watch_file(const std::string &root_dir,
                     const std::filesystem::directory_entry &file) {
  // skip output directory
  if (file.path().string().starts_with(root_dir + "/output")) {
    return true;
  }

  // skip dot files / directories
  if (file.path().string().starts_with(root_dir + "/.")) {
    return true;
  }

  // skip non files
  if (!file.is_regular_file()) {
    return true;
  }

  return false;
}

/**
 * Watches for changes in the project directory and re-compiles the project.
 *
 * @param root_dir The root directory of the project
 */
[[noreturn]] void watch(const std::string &root_dir) {
  std::cout << "Watching for changes..." << std::endl;
  std::unordered_map<std::string, std::filesystem::file_time_type> files;

  // create initial set of files
  for (auto &file : std::filesystem::recursive_directory_iterator(root_dir)) {
    if (skip_watch_file(root_dir, file))
      continue;

    const auto last_write_time = std::filesystem::last_write_time(file);
    files[file.path().string()] = last_write_time;
  }

  // watch for changes
  while (true) {
    std::this_thread::sleep_for(std::chrono::seconds(1));

    // if file was deleted
    for (auto it = files.begin(); it != files.end();) {
      if (!std::filesystem::exists(it->first)) {
        std::cout << "File deleted: " << it->first << std::endl;
        it = files.erase(it);
        run(root_dir);
      } else {
        ++it;
      }
    }

    for (auto &file : std::filesystem::recursive_directory_iterator(root_dir)) {
      if (skip_watch_file(root_dir, file))
        continue;

      const auto last_write_time = std::filesystem::last_write_time(file);

      // if the file exists in the map and the last write time is different
      if (files.contains(file.path().string())) {
        if (files[file.path().string()].time_since_epoch().count() ==
            last_write_time.time_since_epoch().count()) {
          continue;
        }

        std::cout << "File changed: " << file.path().string() << std::endl;
        files[file.path().string()] = last_write_time;
        run(root_dir);
        break;
      }

      // if the file does not exist in the map
      if (!files.contains(file.path().string())) {
        std::cout << "File added: " << file.path().string() << std::endl;
        files[file.path().string()] = last_write_time;
        run(root_dir);
        break;
      }
    }
  }
}
} // namespace tobey
