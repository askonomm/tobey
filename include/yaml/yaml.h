//
// Created by askon on 8/18/2024.
//

#ifndef YAML_H
#define YAML_H

#include <string>
#include <memory>
#include <optional>
#include <vector>
#include <variant>

namespace yaml {
    struct Node {
        std::string key;
        std::variant<std::string, std::vector<Node>> value;
    };

    std::vector<Node> parse(const std::string &input);

    std::optional<Node> find(const std::vector<Node> &nodes, const std::string &key);

    std::optional<std::string> find_node_str_value(const std::vector<Node> &nodes, const std::string &key);
}

#endif //YAML_H
