#include "image_link.hpp"
#include <regex>

namespace image_link {
  std::vector<std::string> inline_parser::matches(const std::string &block) const {
    std::vector<std::string> matches;
    const std::regex p(R"(\!?\[(.*?)\]\((.*?)\))");
    const auto matches_begin = std::sregex_iterator(block.begin(), block.end(), p);
    const auto matches_end = std::sregex_iterator();

    for (std::sregex_iterator i = matches_begin; i != matches_end; ++i) {
      const std::smatch& match = *i;
      matches.push_back(match.str());
    }

    return matches;
  }

  std::string inline_parser::parser(const std::string &match) const {
    const bool is_image = match[0] == '!';
    const std::regex p(R"(\!?\[(.*?)\]\((.*?)\))");
    // find url from match according to regex
    const std::string url = std::regex_replace(match, p, "$2");
    const std::string text = std::regex_replace(match, p, "$1");

    if (is_image) {
      return "<img src=\"" + url + "\" alt=\"" + text + "\">";
    }

    return "<a href=\"" + url + "\">" + text + "</a>";
  }
}