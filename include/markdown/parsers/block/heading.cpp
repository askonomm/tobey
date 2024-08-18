#include "heading.h"
#include <regex>

namespace heading_block {

bool block_parser::identifier(const std::string &block) const {
  const std::regex pattern(R"(^#\s+)");

  return std::regex_search(block, pattern);
}

std::string block_parser::parser(const std::string &block) const {
  int size = 0;

  for (const char c : block) {
    if (c == '#') {
      size++;
    }
  }

  const std::string size_str = std::to_string(size);
  const std::string text = block.substr(size + 1);

  return "<h" + size_str + ">" + text + "</h" + size_str + ">";
}

}