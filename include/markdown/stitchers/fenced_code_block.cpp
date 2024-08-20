#include "../utils.h"
#include <string>
#include <vector>

std::vector<std::string> stitch_fenced_code_blocks(const std::vector<std::string> &blocks) {
  std::vector<std::string> ublocks = blocks;
  for (size_t i = 0; i < ublocks.size(); ++i) {
    if (str_starts_with(ublocks[i], "```") &&
        !str_ends_with(ublocks[i], "```")) {
      size_t j = i + 1;

      while (j < ublocks.size() && !str_ends_with(ublocks[j], "```")) {
        ublocks.at(i) = ublocks[i] + "\r\n" + ublocks[j];
        ++j;
      }

      // join the last block
      ublocks.at(i) = ublocks[i] + "\r\n" + ublocks[j];

      // remove the blocks that were stitched
      ublocks.erase(ublocks.begin() + i + 1, ublocks.begin() + j + 1);
    }
  }

  return ublocks;
}