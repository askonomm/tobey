﻿namespace Markdown.Html.Parsers.Block;

public class HorizontalLine : IBlockParser
{
    public string Parse(string block)
    {
        return "<hr />";
    }
} 