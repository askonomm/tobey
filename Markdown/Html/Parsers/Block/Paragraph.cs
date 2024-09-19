﻿using Markdown.Html.Parsers.Inline;

namespace Markdown.Html.Parsers.Block;

public class Paragraph : IBlockParser
{
    public List<IInlineParser> DefaultInlineParsers()
    {
        return new List<IInlineParser>
        {
            new BoldParser(),
            new ItalicParser(),
        };
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