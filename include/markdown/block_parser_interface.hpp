#ifndef BLOCK_PARSER_INTERFACE_H
#define BLOCK_PARSER_INTERFACE_H

#include <string>

class block_parser_interface {
public:
    virtual ~block_parser_interface() = default;
    [[nodiscard]] virtual bool identifier(const std::string &block) const = 0;
    [[nodiscard]] virtual std::string parser(const std::string &block) const = 0;
};

#endif