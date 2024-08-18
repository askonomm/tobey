#ifndef INLINE_CODE_INLINE_PARSER_H
#define INLINE_CODE_INLINE_PARSER_H

#include "../../inline_parser_interface.h"
#include <string>

namespace inline_code {

struct inline_parser final : public inline_parser_interface {
  [[nodiscard]] std::vector<std::string> matches(const std::string &block) const override;
  [[nodiscard]] std::string parser(const std::string &match) const override;
};

}

#endif