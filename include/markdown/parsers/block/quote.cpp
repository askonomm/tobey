#include "quote.h"
#include "../../utils.h"
#include "../inline/bold.h"
#include <string>
#include <vector>

namespace quote_block {

bool block_parser::identifier(const std::string &block) const {
  // a quote block is a block where every line starts with >
  for (const std::vector<std::string> lines = str_split(block, '\n'); const std::string &line : lines) {
    if (!str_trim(line).empty() && !str_starts_with(line, ">")) {
      return false;
    }
  }

  return true;
}

std::string block_parser::parser(const std::string &block) const {
  const std::vector<std::string> lines = str_split(block, '\n');
  std::string result;

  for (const std::string &line : lines) {
    if (str_trim(line).size() > 1) {
      result += "<p>" + str_trim(line.substr(1)) + "</p>";

      // if not the last line, add line break
      if (line != lines.back()) {
        result += "\n\n";
      }
    }
  }

  // inline parsers
  std::vector<std::unique_ptr<inline_parser_interface>> inline_parsers;

  // bold inline parser
  inline_parsers.push_back(std::make_unique<bold::inline_parser>());

  // parse inline
  result = inline_parse(inline_parsers, result);

  return "<blockquote>" + result + "</blockquote>";
}

} // namespace quote_block