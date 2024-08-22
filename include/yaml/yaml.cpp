#include <iostream>
#include <memory>
#include <optional>
#include <string>
#include <variant>
#include <vector>
#include <sstream>

namespace yaml {
    struct Node {
        std::string key;
        std::variant<std::string, std::vector<Node>> value;
    };

    std::string trim(const std::string& str) {
        const auto str_begin = str.find_first_not_of(" \t");
        const auto str_end = str.find_last_not_of(" \t");

        if (str_begin == std::string::npos) return "";

        return str.substr(str_begin, str_end - str_begin + 1);
    }

    std::string parse_multiline_value(std::istringstream &stream, const int indent_level) {
        std::string value;
        std::string line;

        while(std::getline(stream, line)) {
            const size_t current_indent = line.find_first_not_of(" \t");

            // stop reading when there's less indentation
            if (current_indent == std::string::npos || current_indent < indent_level) {
                const auto off = static_cast<std::streamoff>(-line.size() - 1);
                stream.seekg(off, std::ios_base::cur);
                break;
            }

            value += trim(line) + '\n';
        }

        // remove the last newline character
        if (!value.empty()) {
            value.pop_back();
        }

        return value;
    }

    std::vector<Node> parse_block(std::istringstream &stream, const int indent_level) {
        std::vector<Node> nodes;
        std::string line;

        while(std::getline(stream, line)) {
            const size_t current_indent = line.find_first_not_of(" \t");

            // skip empty lines
            if (current_indent == std::string::npos) continue;

            // if the current line is less indented, it belongs to a previous block
            if (current_indent < indent_level) {
                const auto off = static_cast<std::streamoff>(-line.size() - 1);
                stream.seekg(off, std::ios_base::cur);
                break;
            }

            line = trim(line);
            if (line.empty()) continue;

            // list item - belongs to the previously created node
            if (line.front() == '-') {
                Node node;
                node.key = "";
                node.value = trim(line.substr(1));
                nodes.push_back(node);
                continue;
            }

            const auto colon_pos = line.find(':');

            if (colon_pos == std::string::npos) {
                throw std::runtime_error("Invalid yaml syntax");
            }

            Node node;
            node.key = trim(line.substr(0, colon_pos));
            auto value = trim(line.substr(colon_pos + 1));
            const auto next_indent = static_cast<int>(current_indent + 1);

            // nested block
            if (value.empty() && (stream.peek() == ' ' || stream.peek() == '\t')) {
                node.value = parse_block(stream, next_indent);
                nodes.push_back(node);
                continue;
            }

            // multiline string
            if (value == "|") {
                node.value = parse_multiline_value(stream, next_indent);
                nodes.push_back(node);
                continue;
            }

            // single value
            // if it has surrounding quotes, remove them
            if (value.front() == '"' && value.back() == '"') {
                value = value.substr(1, value.size() - 2);
            }

            node.value = value;
            nodes.push_back(node);
        }

        return nodes;
    }

     std::vector<Node> parse(const std::string &input) {
        std::istringstream stream(input);

        return parse_block(stream, 0);
    }

    std::optional<Node> find_maybe_node(const std::vector<Node> &nodes, const std::string &key) {
        for (const auto &node : nodes) {
            if (node.key == key) {
                return node;
            }

            if (std::holds_alternative<std::vector<Node>>(node.value)) {
                const auto &nested_nodes = std::get<std::vector<Node>>(node.value);

                if (const auto result = find_maybe_node(nested_nodes, key)) {
                    return result;
                }
            }
        }

        return std::nullopt;
    }

    std::optional<std::string> find_maybe_str(const std::vector<Node> &nodes, const std::string &key) {
        if (const auto node = find_maybe_node(nodes, key)) {
            return std::get<std::string>(node->value);
        }

        return std::nullopt;
    }
}
