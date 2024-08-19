//
// Created by askon on 8/18/2024.
//

#ifndef YAML_H
#define YAML_H

#include <string>
#include <unordered_map>
#include <variant>

namespace yaml {
    struct Node {
        std::string key;
        std::variant<std::string, std::vector<Node>> value;
    };

    struct RecursiveMap;
    using RecursiveVariant = std::variant<std::string, std::unique_ptr<RecursiveMap>>;
    struct RecursiveMap : public std::unordered_map<std::string, RecursiveVariant> {};
    RecursiveMap parse(const std::string &input);
}

#endif //YAML_H
