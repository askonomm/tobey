namespace FrontMatter;

public class Parser
{
    public static Dictionary<string, object> Parse(string text)
    {
        if (!ConformsToStructure(text))
        {
            throw new Exception("The text does not conform to the structure of FrontMatter.");
        }

        var lines = text.Split("\r\n").Select(x => x).ToArray();
        var yamlStart = Array.IndexOf(lines, "---");
        var yamlEnd = Array.IndexOf(lines, "---", 1);
        var yaml = string.Join("\n", lines[(yamlStart + 1)..yamlEnd]).Trim();
        var markdown = string.Join("\n", lines[(yamlEnd + 1)..]).Trim();
        var data = YAML.Parser.Parse(yaml);
        var html = new Markdown.Parser(markdown).ParseWith(new Markdown.Parsers.Html.Parser());

        data.Add("content", html);

        return data;
    }

    public static bool ConformsToStructure(string text)
    {
        var lines = text.Split("\r\n").Select(x => x.Trim()).ToArray();
        var yamlStart = Array.IndexOf(lines, "---");
        var yamlEnd = Array.IndexOf(lines, "---", 1);

        return yamlStart != -1 && yamlEnd != -1;
    }
}
