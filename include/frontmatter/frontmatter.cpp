#include <iostream>
#include <sstream>
#include <string>
#include <unordered_map>
#include <variant>

#include "../yaml/yaml.h"
#include "../markdown/markdown.hpp"
#include "../markdown/utils.hpp"

namespace frontmatter {
    std::tuple<std::vector<yaml::Node>, std::string> parse(const std::string &input) {
        std::istringstream stream(input);
        std::string line;
        std::string yaml_str;
        std::string markdown_str;
        bool reading_yaml = false;

        while(std::getline(stream, line)) {
            if (str_trim(line) == "---" && !reading_yaml) {
                reading_yaml = true;
                continue;
            }

            if (str_trim(line) == "---" && reading_yaml) {
                reading_yaml = false;
                continue;
            }

            if (reading_yaml) {
                yaml_str += line + '\n';
            } else {
                markdown_str += line + '\n';
            }
        }

        return {yaml::parse(yaml_str), markdown::parse(markdown_str)};
    }
}
