#include <iostream>
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
            node.value = value;
            nodes.push_back(node);
        }

        return nodes;
    }

    std::vector<Node> parse(const std::string &input) {
        std::istringstream stream(input);

        return parse_block(stream, 0);
    }

    void debug_print(const std::vector<Node>& nodes, const int indent = 0) {
        for (const auto&[key, value] : nodes) {
            std::string indent_str(indent, ' ');

            if (!key.empty()) {
                std::cout << indent_str << "Key: " << key << std::endl;
            }

            if (std::holds_alternative<std::string>(value)) {
                std::cout << indent_str << "Value: " << std::get<std::string>(value) << std::endl;
            } else if(std::holds_alternative<std::vector<Node>>(value)) {
                std::cout << indent_str << "Block:\n";
                debug_print(std::get<std::vector<Node>>(value), indent + 2);
            }
        }
    }
}
