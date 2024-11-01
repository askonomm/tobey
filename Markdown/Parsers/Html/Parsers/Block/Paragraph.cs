namespace Markdown.Parsers.Html.Parsers.Block;

/// <summary>
/// Paragraph block parser. Parses a Markdown paragraph to an HTML paragraph.
/// </summary>
public class Paragraph : IBlockParser
{
    /// <summary>
    /// Default inline parsers for the paragraph block.
    /// </summary>
    /// <returns></returns>
    public static List<IInlineParser> DefaultInlineParsers()
    {
        return [
            new Inline.Bold(),
            new Inline.Italic(),
            new Inline.Link(),
            new Inline.Code()
        ];
    }

    /// <summary>
    /// Parse a Markdown paragraph block to an HTML paragraph.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public string Parse(string block)
    {
        return Parse(block, DefaultInlineParsers());
    }

    /// <summary>
    /// Parse a Markdown paragraph block to an HTML paragraph.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="inlineParsers"></param>
    /// <returns></returns>
    public string Parse(string block, List<IInlineParser> inlineParsers)
    {
        return $"<p>{InlineParser.Parse(block, inlineParsers)}</p>";
    }
}
