using System.Text.RegularExpressions;

namespace Markdown.Html.Parsers.Inline;

public partial class Link : IInlineParser
{
    [GeneratedRegex(@"(\!?\[(.*?)\]\((.*?)\))")]
    private static partial Regex MatchLink();

    [GeneratedRegex(@"(\[(?<label>.*?)\])")]
    private static partial Regex MatchLinkLabel();

    [GeneratedRegex(@"(\((?<url>.*?)(\s(\""|\')(?<alt>.*?)(\""|\'))?\))")]
    private static partial Regex MatchLinkURL();

    public string[] Matches(string block)
    {
        return MatchLink()
            .Matches(block)
            .Select(x => x.Value)
            .ToArray();
    }

    public string Parse(string match)
    {
        var urlGroups = MatchLinkURL().Match(match).Groups;
        var url = urlGroups["url"].Value;
        var alt = urlGroups.ContainsKey("alt") ? urlGroups["alt"].Value : "";
        var label = MatchLinkLabel().Match(match).Groups["label"].Value;

        if (match.StartsWith('!') && alt.Length > 0)
        {
            return $"<img src=\"{url}\" alt=\"{label}\" title=\"{alt}\" />";
        }

        if (match.StartsWith('!'))
        {
            return $"<img src=\"{url}\" alt=\"{label}\" />";
        }

        if (alt.Length > 0)
        {
            return $"<a href=\"{url}\" title=\"{alt}\">{label}</a>";
        }

        return $"<a href=\"{url}\">{label}</a>";
    }
}