#ifndef FENCED_CODE_BLOCK_PARSER_H
#define FENCED_CODE_BLOCK_PARSER_H

#include "../../block_parser_interface.h"
#include <string>

namespace fenced_code_block {

struct block_parser final : public block_parser_interface {
  [[nodiscard]] bool identifier(const std::string &block) const override;
  [[nodiscard]] std::string parser(const std::string &block) const override;
};

}

#endif