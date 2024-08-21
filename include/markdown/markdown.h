#ifndef MARKDOWN_H
#define MARKDOWN_H

#include "block_parser_interface.h"
#include <string>
#include <vector>
#include <memory>

namespace markdown {
    std::string parse(const std::string &input, const std::vector<std::unique_ptr<block_parser_interface>>& block_parsers);
    std::string parse(const std::string &input);
}

#endif