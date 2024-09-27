namespace Markdown.Parsers.Html.Parsers.Block;

public class Heading : IBlockParser
{
    public List<IInlineParser> DefaultInlineParsers()
    {
        return
        [
            new Markdown.Html.Parsers.Inline.Italic(),
            new Markdown.Html.Parsers.Inline.Link(),
        ];
    }

    public string Parse(string block)
    {
        return Parse(block, DefaultInlineParsers());
    }

    public string Parse(string block, List<IInlineParser> inlineParsers)
    {
        var level = block.TakeWhile(c => c == '#').Count();
        var text = block[level..].Trim();

        return $"<h{level}>{InlineParser.Parse(text, inlineParsers)}</h{level}>";
    }
}