#include "strikethrough.h"
#include <regex>

namespace strikethrough {

std::vector<std::string>
inline_parser::matches(const std::string &block) const {
  std::vector<std::string> matches;
  std::regex p(R"(\~{1}(.*?)\~{1})");
  auto matches_begin = std::sregex_iterator(block.begin(), block.end(), p);
  auto matches_end = std::sregex_iterator();

  for (std::sregex_iterator i = matches_begin; i != matches_end; ++i) {
    std::smatch match = *i;
    matches.push_back(match.str());
  }

  return matches;
}

std::string inline_parser::parser(const std::string &match) const {
  return "<del>" + match.substr(1, match.size() - 2) + "</del>";
}

} // namespace strikethrough