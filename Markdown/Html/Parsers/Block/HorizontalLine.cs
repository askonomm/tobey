namespace Markdown.Html.Parsers.Block;

public class HorizontalLineParser : IBlockParser
{
    public string Parse(string block)
    {
        return "<hr />";
    }
}