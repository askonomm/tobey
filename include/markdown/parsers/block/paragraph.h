#ifndef PARAGRAPH_BLOCK_PARSER_H
#define PARAGRAPH_BLOCK_PARSER_H

#include "../../block_parser_interface.h"
#include <string>

namespace paragraph_block {

struct block_parser final : public block_parser_interface {
  bool identifier(const std::string &block) const override;
  std::string parser(const std::string &block) const override;
};

} // namespace paragraph_block

#endif