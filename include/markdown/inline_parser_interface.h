#ifndef INLINE_PARSER_INTERFACE_H
#define INLINE_PARSER_INTERFACE_H

#include <string>
#include <vector>

class inline_parser_interface {
public:
    virtual ~inline_parser_interface() = default;
    [[nodiscard]] virtual std::vector<std::string> matches(const std::string &block) const = 0;
    [[nodiscard]] virtual std::string parser(const std::string &match) const = 0;
};

#endif