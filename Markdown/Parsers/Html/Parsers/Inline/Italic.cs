using System.Text.RegularExpressions;

namespace Markdown.Parsers.Html.Parsers.Inline
{
    public partial class Italic : IInlineParser
    {
        [GeneratedRegex(@"(?<match>(?<!`)(?<!\*)\*([^*\n`]+)\*(?!\*)(?!`)|(?<!`)(?<!_)_([^_\n`]+)_(?!_)(?!`))(?![^`]*`)")]
        private static partial Regex MatchItalic();
        public string[] Matches(string block)
        {
            return MatchItalic()
                .Matches(block)
                .Select(x => x.Groups["match"].Value)
                .ToArray();
        }

        public string Parse(string match)
        {
            return $"<em>{match[1..^1]}</em>";
        }
    }
}