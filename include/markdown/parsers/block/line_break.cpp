#include "line_break.h"
#include <algorithm>

static int count_chars(const std::string &str) {
  int count = 0;

  for ([[maybe_unused]] auto &c : str) {
    count++;
  }

  return count;
}

static int count_occurences_of(const std::string &str, char input) {
  int count = 0;

  for (auto &c : str) {
    if (c == input) {
      count++;
    }
  }

  return count;
}

namespace line_break_block {

bool block_parser::identifier(const std::string &block) const {
  const int lines = std::count(block.begin(), block.end(), '\n');
  const int char_count = count_chars(block);
  const int asterisk_char_occurences = count_occurences_of(block, '*');
  const int underscore_char_occurences = count_occurences_of(block, '_');
  const int dash_char_occurences = count_occurences_of(block, '-');
  bool identifies = false;

  // asterisk?
  if (asterisk_char_occurences == char_count) {
    identifies = true;
  }

  // underscore?
  if (underscore_char_occurences == char_count) {
    identifies = true;
  }

  // dash?
  if (dash_char_occurences == char_count) {
    identifies = true;
  }

  return identifies && lines == 0 && char_count >= 3;
}

std::string
block_parser::parser([[maybe_unused]] const std::string &block) const {
  return "<hr>";
}

} // namespace line_break_block