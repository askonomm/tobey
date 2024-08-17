#include "image_link.h"
#include <regex>

namespace image_link {

std::vector<std::string>
inline_parser::matches(const std::string &block) const {
  std::vector<std::string> matches;
  std::regex p(R"(\!?\[(.*?)\]\((.*?)\))");
  auto matches_begin = std::sregex_iterator(block.begin(), block.end(), p);
  auto matches_end = std::sregex_iterator();

  for (std::sregex_iterator i = matches_begin; i != matches_end; ++i) {
    std::smatch match = *i;
    matches.push_back(match.str());
  }

  return matches;
}

std::string inline_parser::parser(const std::string &match) const {
  bool is_image = match[0] == '!';
  std::regex p(R"(\!?\[(.*?)\]\((.*?)\))");
  // find url from match according to regex
  std::string url = std::regex_replace(match, p, "$2");
  std::string text = std::regex_replace(match, p, "$1");

  if (is_image) {
    return "<img src=\"" + url + "\" alt=\"" + text + "\">";
  }

  return "<a href=\"" + url + "\">" + text + "</a>";
}

} // namespace image_link