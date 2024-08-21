#ifndef BOLD_INLINE_PARSER_H
#define BOLD_INLINE_PARSER_H

#include "../../inline_parser_interface.h"
#include <string>

namespace bold {
  struct inline_parser final : public inline_parser_interface {
    [[nodiscard]] std::vector<std::string> matches(const std::string &block) const override;
    [[nodiscard]] std::string parser(const std::string &match) const override;
  };
}

#endif