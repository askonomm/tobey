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
}

#endif //YAML_H
