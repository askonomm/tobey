namespace Markdown.Parsers.Html.Parsers.Block;

/// <summary>
/// Code block parser. Parses a Markdown code block to an HTML code block.
/// </summary>
public class Code : IBlockParser
{
    /// <summary>
    /// Parse a Markdown code block to an HTML code block.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public string Parse(string content)
    {
        // Code without first and last line
        var code = content
            .Split("\n")
            .Skip(1)
            .Take(content.Split("\n").Length - 2)
            .Aggregate("", (current, line) => current + line + "\n")
            .Trim();
        
        // HTML entities
        code = code
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
        
        // Get language from the first line after backticks
        var language = content
            .Split("\n")
            .Take(1)
            .First()
            .Replace("```", "")
            .Trim();

        if (language.Length > 0)
        {
            return $"<pre class=\"language-{language}\"><code>{code}</code></pre>";
        }

        return $"<pre><code>{code}</code></pre>";
    }
}