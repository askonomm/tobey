namespace Markdown.Parsers.Html
{
    public static class InlineParser
    {
        public static string Parse(string block, List<IInlineParser> parsers)
        {
            foreach (var parser in parsers)
            {
                var matches = parser.Matches(block).Distinct().ToArray();

                foreach (var match in matches)
                {
                    block = block.Replace(match, parser.Parse(match));
                }
            }

            return block;
        }
    }
}