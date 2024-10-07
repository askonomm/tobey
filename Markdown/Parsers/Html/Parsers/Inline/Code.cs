using System.Text.RegularExpressions;

namespace Markdown.Parsers.Html.Parsers.Inline
{
    public partial class Code : IInlineParser
    {
        [GeneratedRegex(@"(?<match>`([^`\n]+)`(?!`))")]
        private static partial Regex MatchCode();

        public string[] Matches(string block)
        {
            return MatchCode()
                .Matches(block)
                .Select(x => x.Groups["match"].Value)
                .ToArray();
        }

        public string Parse(string match)
        {
            return $"<code>{match[1..^1]}</code>";
        }
    }
}