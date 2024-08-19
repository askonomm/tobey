//
// Created by askon on 8/18/2024.
//

#ifndef YAML_H
#define YAML_H

#include <string>
#include <variant>

namespace yaml {
    struct Node {
        std::string key;
        std::variant<std::string, std::vector<Node>> value;
    };

    std::vector<Node> parse(const std::string &input);

    std::variant<std::string, std::vector<Node>> get_value(const std::vector<Node>& nodes, const std::string& key);

    void debug_print(const std::vector<Node>& nodes, int indent = 0);
}

#endif //YAML_H
