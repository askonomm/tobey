#include <regex>

#include "bold.hpp"
#include "../../utils.hpp"

namespace bold {
  std::vector<std::string> inline_parser::matches(const std::string &block) const {
    std::vector<std::string> matches;
    const std::regex p(R"(\*{2}(.*?)\*{2}|_{2}(.*?)_{2})");
    const auto matches_begin = std::sregex_iterator(block.begin(), block.end(), p);
    const auto matches_end = std::sregex_iterator();

    for (std::sregex_iterator i = matches_begin; i != matches_end; ++i) {
      const std::smatch& match = *i;

      if (const size_t match_start = match.position(0); !is_inside_backticks(block, match_start))
      {
        matches.push_back(match.str(0));;
      }
    }

    return matches;
  }

  std::string inline_parser::parser(const std::string &match) const {
    return "<strong>" + match.substr(2, match.size() - 4) + "</strong>";
  }
}