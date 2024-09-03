#include <string>
#include <format>

#include "../libs/inja.hpp"

namespace templating {
  std::string format_date(const inja::Arguments &args) {
    const auto date = args.at(0)->get<std::string>();
    const auto date_format = args.at(1)->get<std::string>();

    std::tm tm = {};
    std::istringstream ss(date);
    ss >> std::get_time(&tm, "%Y-%m-%d");
    std::ostringstream oss;
    oss << std::put_time(&tm, date_format.c_str());

    return oss.str();
  }
}