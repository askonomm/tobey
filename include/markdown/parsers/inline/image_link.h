#ifndef IMAGE_LINK_PARSER_H
#define IMAGE_LINK_PARSER_H

#include "../../inline_parser_interface.h"
#include <string>

namespace image_link {

struct inline_parser final : public inline_parser_interface {
  std::vector<std::string> matches(const std::string &block) const override;
  std::string parser(const std::string &match) const override;
};

} // namespace image_link

#endif