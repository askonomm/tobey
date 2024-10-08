using System.Text.RegularExpressions;

namespace Markdown.Parsers.Html.Parsers.Inline
{
    public partial class Bold : IInlineParser
    {
        [GeneratedRegex(@"(?<!`)(?<match>(?<!\*)\*\*([^*\n]+)\*\*(?!\*)|(?<!_)__([^_\n]+)__(?!_))(?!`)")]
        private static partial Regex MatchBold();

        public string[] Matches(string block)
        {
            return MatchBold()
                .Matches(block)
                .Select(x => x.Groups["match"].Value)
                .ToArray();
        }

        public string Parse(string match)
        {
            return $"<strong>{match[2..^2]}</strong>";
        }
    }
}