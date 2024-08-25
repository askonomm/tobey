#ifndef QUOTE_BLOCK_PARSER_H
#define QUOTE_BLOCK_PARSER_H

#include "../../block_parser_interface.hpp"
#include <string>

namespace quote_block {
  struct block_parser final : public block_parser_interface {
    [[nodiscard]] bool identifier(const std::string &block) const override;
    [[nodiscard]] std::string parser(const std::string &block) const override;
  };
}

#endif