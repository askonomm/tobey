#ifndef PARAGRAPH_BLOCK_PARSER_H
#define PARAGRAPH_BLOCK_PARSER_H

#include "../../block_parser_interface.h"
#include <string>

namespace paragraph_block {
  struct block_parser final : public block_parser_interface {
    [[nodiscard]] bool identifier(const std::string &block) const override;
    [[nodiscard]] std::string parser(const std::string &block) const override;
  };
}

#endif