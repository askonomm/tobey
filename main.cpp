#include "include/markdown/markdown.h"
#include "utils.h"
#include <iostream>

#include "include/yaml/yaml.h"
#include "include/frontmatter/frontmatter.h"

int main() {
    //const std::string test_md = read_file("/Users/askon/CLionProjects/Tobey/test/test.md");
    //const std::string test_yaml = read_file("/Users/askon/CLionProjects/Tobey/test/test.yaml");
    const std::string test_fm = read_file("/Users/askon/CLionProjects/Tobey/test/testfm.md");

    //std::cout << markdown::parse(test_md) << std::endl;
    const auto [nodes, markdown] = frontmatter::parse(test_fm);
    const auto title = yaml::find_node_str_value(nodes, "title");

    std::cout << "Markdown: " << markdown << std::endl;
    std::cout << "Title: " << *title << std::endl;
    return 0;
}
