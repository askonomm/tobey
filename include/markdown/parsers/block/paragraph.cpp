#include "paragraph.h"
#include "../../utils.h"
#include "../inline/bold.h"
#include "../inline/image_link.h"
#include "../inline/inline_code.h"
#include "../inline/italic.h"
#include "../inline/strikethrough.h"
#include <regex>

namespace paragraph_block {

bool block_parser::identifier(const std::string &block) const {
  const std::regex pattern(R"(^\w+)");

  return std::regex_search(block, pattern);
}

std::string block_parser::parser(const std::string &block) const {
  auto result = block;

  // inline parsers
  std::vector<std::unique_ptr<inline_parser_interface>> inline_parsers;

  // bold inline parser
  inline_parsers.push_back(std::make_unique<bold::inline_parser>());

  // italic inline parser
  inline_parsers.push_back(std::make_unique<italic::inline_parser>());

  // inline code inline parser
  inline_parsers.push_back(std::make_unique<inline_code::inline_parser>());

  // strikethrough inline parser
  inline_parsers.push_back(std::make_unique<strikethrough::inline_parser>());

  // image link inline parser
  inline_parsers.push_back(std::make_unique<image_link::inline_parser>());

  // parse inline
  result = inline_parse(inline_parsers, result);

  return "<p>" + str_trim(result) + "</p>";
}

}