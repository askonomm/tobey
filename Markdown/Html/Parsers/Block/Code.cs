namespace Markdown.Html.Parsers.Block;

public class Code : IBlockParser
{
    public string Parse(string content)
    {
        // Code without first and last line
        var code = content
            .Split("\n")
            .Skip(1)
            .Take(content.Split("\n").Length - 2)
            .Aggregate("", (current, line) => current + line + "\n")
            .Trim();

        // Get language from the first line after backticks
        var language = content
            .Split("\n")
            .Take(1)
            .First()
            .Replace("```", "")
            .Trim();

        if (language.Length > 0)
        {
            return $"<pre class=\"language-{ language}\"><code>{code}</code></pre>";
        }

        return $"<pre><code>{code}</code></pre>";
    }
}
