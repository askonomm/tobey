using System.Text.RegularExpressions;

namespace Markdown.Html.Parsers.Inline;

public partial class Italic : IInlineParser
{
    [GeneratedRegex(@"\*(.*?)\*")]
    private static partial Regex MatchItalic();
    
    public string[] Matches(string block)
    {
        return MatchItalic()
            .Matches(block)
            .Select(x => x.Value)
            .ToArray();
    }

    public string Parse(string match)
    {
        return $"<em>{match[1..^1]}</em>";
    }
}