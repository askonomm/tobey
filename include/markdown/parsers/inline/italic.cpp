#include "italic.hpp"
#include <regex>

namespace italic {
  std::vector<std::string> inline_parser::matches(const std::string &block) const {
    std::vector<std::string> matches;
    const std::regex p(R"(\*{1}(.*?)\*{1}|_{1}(.*?)_{1})");
    const auto matches_begin = std::sregex_iterator(block.begin(), block.end(), p);
    const auto matches_end = std::sregex_iterator();

    for (std::sregex_iterator i = matches_begin; i != matches_end; ++i) {
      const std::smatch& match = *i;
      matches.push_back(match.str());
    }

    return matches;
  }

  std::string inline_parser::parser(const std::string &match) const {
    return "<em>" + match.substr(1, match.size() - 2) + "</em>";
  }
}