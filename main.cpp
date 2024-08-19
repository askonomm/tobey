#include "include/markdown/markdown.h"
#include "utils.h"
#include <iostream>

#include "include/yaml/yaml.h"

int main() {
    //const std::string test_md = read_file("/Users/askon/CLionProjects/Tobey/test/test.md");
    const std::string test_yaml = read_file("/Users/askon/CLionProjects/Tobey/test/test.yaml");

    //std::cout << markdown::parse(test_md) << std::endl;
    const auto data = yaml::parse(test_yaml);
    const auto version = std::get<std::string>(data.at("version"));
    std::cout << "Version: " << version << std::endl;
    //const auto& version = std::move(data.at("version"));

    // print version
    //std::cout << "Version: " << std::get<std::string>(version) << std::endl;


    return 0;
}
