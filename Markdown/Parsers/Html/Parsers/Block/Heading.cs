using Markdown.Parsers.Html.Parsers.Inline;

namespace Markdown.Parsers.Html.Parsers.Block;

/// <summary>
/// Heading block parser. Parses a Markdown heading to an HTML heading.
/// </summary>
public class Heading : IBlockParser
{
    /// <summary>
    /// Default inline parsers for the heading block.
    /// </summary>
    /// <returns></returns>
    public List<IInlineParser> DefaultInlineParsers()
    {
        return
        [
            new Italic(),
            new Link(),
        ];
    }

    /// <summary>
    /// Parse a Markdown heading block to an HTML heading.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public string Parse(string block)
    {
        return Parse(block, DefaultInlineParsers());
    }

    /// <summary>
    /// Parse a Markdown heading block to an HTML heading.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="inlineParsers"></param>
    /// <returns></returns>
    public string Parse(string block, List<IInlineParser> inlineParsers)
    {
        var level = block.TakeWhile(c => c == '#').Count();
        var text = block[level..].Trim();

        return $"<h{level}>{InlineParser.Parse(text, inlineParsers)}</h{level}>";
    }
}
