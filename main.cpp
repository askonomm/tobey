#include "include/markdown/markdown.h"
#include "utils.h"
#include <iostream>

int main() {
    const std::string test_md = read_markdown_file("/Users/askon/CLionProjects/Tobey/test/test.md");

    std::cout << markdown::parse(test_md) << std::endl;

    return 0;
}