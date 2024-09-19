namespace Markdown.Html;

public static class InlineParser
{
    public static string Parse(string block, List<IInlineParser> parsers)
    {
        foreach (var parser in parsers)
        {
            var matches = parser.Matches(block);
            
            block = matches.Aggregate(block, (current, match) => current.Replace(match, parser.Parse(match)));
        }
        
        return block;
    }
}