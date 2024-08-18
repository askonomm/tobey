#include "line_break.h"
#include <algorithm>

static int count_chars(const std::string &str) {
  int count = 0;

  for ([[maybe_unused]] auto &c : str) {
    count++;
  }

  return count;
}

static int count_occurrences_of(const std::string &str, const char input) {
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
  const size_t lines = std::ranges::count(block, '\n');
  const int char_count = count_chars(block);
  const int asterisk_char_occurrences = count_occurrences_of(block, '*');
  const int underscore_char_occurrences = count_occurrences_of(block, '_');
  const int dash_char_occurrences = count_occurrences_of(block, '-');
  bool identifies = false;

  // asterisk?
  if (asterisk_char_occurrences == char_count) {
    identifies = true;
  }

  // underscore?
  if (underscore_char_occurrences == char_count) {
    identifies = true;
  }

  // dash?
  if (dash_char_occurrences == char_count) {
    identifies = true;
  }

  return identifies && lines == 0 && char_count >= 3;
}

std::string block_parser::parser([[maybe_unused]] const std::string &block) const {
  return "<hr>";
}

}