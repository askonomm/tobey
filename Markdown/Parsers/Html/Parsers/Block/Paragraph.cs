using Markdown.Parsers.Html.Parsers;

namespace Markdown.Parsers.Html.Parsers.Block
{
    public class Paragraph : IBlockParser
    {
        public static List<IInlineParser> DefaultInlineParsers()
        {
            return [
                new Inline.Bold(),
                new Inline.Italic(),
                new Inline.Link(),
                new Inline.Code()
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