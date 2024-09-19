using System.Text.RegularExpressions;

namespace Markdown.Html.Parsers.Inline;

public partial class Bold : IInlineParser
{
    [GeneratedRegex(@"(\*{2}(.*?)\*{2}|_{2}(.*?)_{2})")]
    private static partial Regex MatchBold();
    
    public string[] Matches(string block)
    {
        return MatchBold()
            .Matches(block)
            .Select(x => x.Value)
            .ToArray();
    }

    public string Parse(string match)
    {
        return $"<strong>{match[2..^2]}</strong>";
    }
}