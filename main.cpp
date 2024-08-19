#include "include/markdown/markdown.h"
#include "utils.h"
#include <iostream>

#include "include/yaml/yaml.h"

int main() {
    //const std::string test_md = read_file("/Users/askon/CLionProjects/Tobey/test/test.md");
    const std::string test_yaml = read_file("/Users/askon/CLionProjects/Tobey/test/test.yaml");

    //std::cout << markdown::parse(test_md) << std::endl;
    auto nodes = yaml::parse(test_yaml);

    // print nodes
    debug_print(nodes);



    return 0;
}
