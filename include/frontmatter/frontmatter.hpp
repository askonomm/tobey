#ifndef FRONTMATTER_H
#define FRONTMATTER_H

#include <string>
#include "../yaml/yaml.h"

namespace frontmatter {
     std::tuple<std::vector<yaml::Node>, std::string> parse(const std::string &input);
}

#endif //FRONTMATTER_H
