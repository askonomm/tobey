using Markdown.Parsers.Html.Parsers.Inline;

namespace Markdown.Parsers.Html.Parsers.Block
{
    public class Paragraph : IBlockParser
    {
        public static List<IInlineParser> DefaultInlineParsers()
        {
            return [
                new Bold(),
                new Italic(),
                new Link(),
            ];
        }

        public string Parse(string block)
        {
            return Parse(block, DefaultInlineParsers());
        }

        public string Parse(string block, List<IInlineParser> inlineParsers)
        {
            return $"<p>{InlineParser.Parse(block, inlineParsers)}</p>";
        }
    }
}