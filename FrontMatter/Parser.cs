namespace FrontMatter;

public class Parser
{
    public static Dictionary<string, object> Parse(string text)
    {        
        var lines = text.Split("\n");
        var yamlStart = Array.IndexOf(lines, "---");
        var yamlEnd = Array.IndexOf(lines, "---", 1);

        // There is no YAML, just markdown
        if (yamlStart == -1)
        {
            return new Dictionary<string, object>
            {
                { "content", new Markdown.Parser(text).ParseWith(new Markdown.Html.Parser()) }
            };
        }

        var yaml = string.Join("\n", lines[(yamlStart + 1)..yamlEnd]).Trim();
        var markdown = string.Join("\n", lines[(yamlEnd + 1)..]).Trim();
        var data = YAML.Parser.Parse(yaml);
        var html = new Markdown.Parser(markdown).ParseWith(new Markdown.Html.Parser());

        data.Add("content", html);

        return data;
    }
}