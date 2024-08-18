#include "include/markdown/markdown.h"
#include "utils.h"
#include <iostream>

#include "include/yaml/yaml.h"

int main() {
    //const std::string test_md = read_file("/Users/askon/CLionProjects/Tobey/test/test.md");
    const std::string test_yaml = read_file("/Users/askon/CLionProjects/Tobey/test/test.yaml");

    //std::cout << markdown::parse(test_md) << std::endl;
    auto nodes = yaml::parse(test_yaml);

    for (const auto& node : nodes) {
        std::cout << "Key: " << node.key << std::endl;
        if (std::holds_alternative<std::string>(node.value)) {
            std::cout << "Value: " << std::get<std::string>(node.value) << std::endl;
        } else {
            std::cout << "Nested nodes:" << std::endl;
            for (const auto& subnode : std::get<std::vector<yaml::Node>>(node.value)) {
                std::cout << "  Subkey: " << subnode.key << ", Subvalue: " << std::get<std::string>(subnode.value) << std::endl;
            }
        }
    }

    return 0;
}
