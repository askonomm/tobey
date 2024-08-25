#ifndef IMAGE_LINK_PARSER_H
#define IMAGE_LINK_PARSER_H

#include "../../inline_parser_interface.hpp"
#include <string>

namespace image_link {
  struct inline_parser final : public inline_parser_interface {
    [[nodiscard]] std::vector<std::string> matches(const std::string &block) const override;
    [[nodiscard]] std::string parser(const std::string &match) const override;
  };
}

#endif