#ifndef MARKDOWN_UTILS_H
#define MARKDOWN_UTILS_H

#include <memory>
#include <string>
#include <vector>

#include "inline_parser_interface.h"

std::string str_trim(const std::string &s);

bool str_starts_with(const std::string &s, const std::string &prefix);

bool str_ends_with(const std::string &s, const std::string &suffix);

std::vector<std::string> str_split(const std::string &s, char c);

std::string str_replace(const std::string &s, const std::string &match, const std::string &replacement);

std::string inline_parse(const std::vector<std::unique_ptr<inline_parser_interface>> &parsers, const std::string& text);

#endif