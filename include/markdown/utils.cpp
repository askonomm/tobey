#include "inline_parser_interface.h"
#include <memory>
#include <string>
#include <vector>

std::string str_trim(const std::string &s) {
    // start of string
    const size_t start = s.find_first_not_of(" \t\n\r");

    // if string is empty
    if (start == std::string::npos) {
        return s;
    }

    // end of string
    const size_t end = s.find_last_not_of(" \t\n\r");

    // return trimmed string
    return s.substr(start, end - start + 1);
}

bool str_starts_with(const std::string &s, const std::string &prefix) {
    return s.size() >= prefix.size() && s.compare(0, prefix.size(), prefix) == 0;
}

bool str_ends_with(const std::string &s, const std::string &suffix) {
    return s.size() >= suffix.size() &&
           s.compare(s.size() - suffix.size(), suffix.size(), suffix) == 0;
}

std::vector<std::string> str_split(const std::string &s, const char c) {
    std::vector<std::string> tokens;

    // walk the string char by char
    size_t start = 0;

    for (size_t i = 0; i < s.size(); ++i) {
        if (s[i] == c) {
            tokens.push_back(s.substr(start, i - start));
            start = i + 1;
        }
    }

    // add the last token
    tokens.push_back(s.substr(start));

    return tokens;
}

std::string str_replace(const std::string &s, const std::string &match,
                        const std::string &replacement) {
    std::string result = s;

    size_t pos = 0;

    while ((pos = result.find(match, pos)) != std::string::npos) {
        result.replace(pos, match.size(), replacement);
        pos += replacement.size();
    }

    return result;
}

std::string inline_parse(const std::vector<std::unique_ptr<inline_parser_interface>> &parsers, const std::string& text) {
    auto result = text;
    for (const auto &parser : parsers) {
        const std::vector<std::string> matches = parser->matches(result);

        for (const auto &match : matches) {
            result = str_replace(result, match, parser->parser(match));
        }
    }

    return result;
}