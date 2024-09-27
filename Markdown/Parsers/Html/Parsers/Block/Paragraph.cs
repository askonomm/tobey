using Markdown.Html.Parsers.Inline;
using Markdown.Parsers.Html;

namespace Markdown.Parsers.Html.Parsers.Block;

public class Paragraph : IBlockParser
{
    public List<IInlineParser> DefaultInlineParsers()
    {
        return [
            new Bold(),
            new Italic(),
            new Link(),
        ];
    }

    public string Parse(string block)
    {
        return Parse(block, DefaultInlineParsers());
    }

    public string Parse(string block, List<IInlineParser> inlineParsers)
    {
        return $"<p>{InlineParser.Parse(block, inlineParsers)}</p>";
    }
}