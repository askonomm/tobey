namespace Markdown.Parsers.Html.Parsers.Block;

/// <summary>
/// Horizontal line block parser. Parses a Markdown horizontal line to an HTML horizontal line.
/// </summary>
public class HorizontalLine : IBlockParser
{
    /// <summary>
    /// Parse a Markdown horizontal line block to an HTML horizontal line.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public string Parse(string block)
    {
        return "<hr />";
    }
}