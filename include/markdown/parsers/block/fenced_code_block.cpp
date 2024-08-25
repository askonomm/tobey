#include "fenced_code_block.hpp"
#include "../../utils.hpp"

namespace fenced_code_block {
  bool block_parser::identifier(const std::string &block) const {
    return str_starts_with(block, "```") && str_ends_with(block, "```");
  }

  std::string block_parser::parser(const std::string &block) const {
    const std::string first_line = block.substr(0, block.find('\n'));
    const std::string language = str_trim(first_line.substr(3));

    // length of first line
    const size_t r_a = first_line.size() + 1; // 2 accounts for line break
    constexpr size_t r_z = 3 + 1;                 // 2 accounts for line break
    std::string code = block.substr(r_a, block.size() - r_a - r_z);

    // make code safe for HTML output
    code = str_replace(code, "&", "&amp;");
    code = str_replace(code, "<", "&lt;");
    code = str_replace(code, ">", "&gt;");
    code = str_replace(code, "\"", "&quot;");
    code = str_replace(code, "'", "&apos;");
    code = str_replace(code, "\n", "<br>");
    code = str_replace(code, "\t", "    ");
    code = str_replace(code, " ", "&nbsp;");

    if (!language.empty()) {
      return "<pre><code class=\"" + language + "\">" + code + "</code></pre>";
    }

    return "<pre><code>" + code + "</code></pre>";
  }
}