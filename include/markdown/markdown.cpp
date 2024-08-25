#include <memory>
#include <string>
#include <vector>

#include "markdown.hpp"
#include "block_parser_interface.hpp"
#include "parsers/block/fenced_code_block.hpp"
#include "parsers/block/heading.hpp"
#include "parsers/block/line_break.hpp"
#include "parsers/block/paragraph.hpp"
#include "parsers/block/quote.hpp"
#include "stitchers/fenced_code_block.hpp"
#include "utils.hpp"

/**
 * Splits text by "\n\n".
 *
 * @param text The text to split
 * @return A vector of strings split by "\n\n"
 */
static std::vector<std::string> split_by_crlf(const std::string& text)
{
    std::vector<std::string> result;
    std::string::size_type start = 0;
    std::string::size_type end = 0;

    while ((end = text.find("\n\n", start)) != std::string::npos)
    {
        result.push_back(text.substr(start, end - start));
        start = end + 2; // move past \n\n
    }

    // add remaining
    result.push_back(text.substr(start));

    return result;
}

/**
 * Joins blocks by "\n\n".
 *
 * @param blocks Vector of blocks
 * @return A string with blocks joined by "\n\n"
 */
static std::string join_by_crlf(const std::vector<std::string>& blocks)
{
    std::string result;

    for (size_t i = 0; i < blocks.size(); ++i)
    {
        result += blocks[i];

        // unless we're in a last block, add newline
        if (i != blocks.size() - 1)
        {
            result += "\n\n";
        }
    }

    return result;
}

namespace markdown
{
    std::string parse(const std::string& input,
                      const std::vector<std::unique_ptr<block_parser_interface>>& block_parsers)
    {
        // make input into blocks
        const std::vector<std::string> raw_blocks = split_by_crlf(input);

        // stitch fenced code blocks
        const std::vector<std::string> stitched_blocks =
            stitch_fenced_code_blocks(raw_blocks);

        // parse blocks
        std::vector<std::string> parsed_blocks;

        for (const auto& block : stitched_blocks)
        {
            for (const auto& parser : block_parsers)
            {
                if (const std::string trimmed_block = str_trim(block); parser->identifier(trimmed_block))
                {
                    parsed_blocks.push_back(parser->parser(trimmed_block));
                    break;
                }
            }
        }

        // join by \n\n
        return join_by_crlf(parsed_blocks);
    }

    std::string parse(const std::string& input)
    {
        // block parsers
        std::vector<std::unique_ptr<block_parser_interface>> block_parsers;

        // paragraph block parser
        block_parsers.push_back(std::make_unique<paragraph_block::block_parser>());

        // heading block parser
        block_parsers.push_back(std::make_unique<heading_block::block_parser>());

        // line break block parser
        block_parsers.push_back(std::make_unique<line_break_block::block_parser>());

        // fenced code block parser
        block_parsers.push_back(std::make_unique<fenced_code_block::block_parser>());

        // quote block parser
        block_parsers.push_back(std::make_unique<quote_block::block_parser>());

        // make input into blocks
        const std::vector<std::string> raw_blocks = split_by_crlf(input);

        // stitch fenced code blocks
        const std::vector<std::string> stitched_blocks =
            stitch_fenced_code_blocks(raw_blocks);

        // parse blocks
        std::vector<std::string> parsed_blocks;

        for (const auto& block : stitched_blocks)
        {
            for (const auto& parser : block_parsers)
            {
                if (const std::string trimmed_block = str_trim(block); parser->identifier(trimmed_block))
                {
                    parsed_blocks.push_back(parser->parser(trimmed_block));
                    break;
                }
            }
        }

        // join by \n\n
        return join_by_crlf(parsed_blocks);
    }
}
