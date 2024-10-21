namespace FrontMatter;

public class Parser
{
    public static Dictionary<string, object?> Parse(string text)
    {
        if (!ConformsToStructure(text))
        {
            throw new Exception("The text does not conform to the structure of FrontMatter.");
        }

        var lines = text.Split("\r\n").Select(x => x).ToArray();
        var blockStart = Array.IndexOf(lines, "---");
        var blockEnd = Array.IndexOf(lines, "---", 1);
        var dataLines = lines[(blockStart + 1)..blockEnd];
        var data = new Dictionary<string, object?>();
        
        foreach (var line in dataLines)
        {
            var parts = line.Trim().Split(":");
            var keys = parts[0].Trim().Split('.');
            var value = ParseValue(parts[1].Trim());
            var current = data;
            
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                if (i == keys.Length - 1)
                {
                    current[key] = value;
                }
                else
                {
                    if (!current.ContainsKey(key))
                    {
                        current[key] = new Dictionary<string, object>();
                    }
                    
                    if (current[key] is Dictionary<string, object?> dict)
                    {
                        current = dict;
                    }
                }
            }
        }
        
        var markdown = string.Join("\n", lines[(blockEnd + 1)..]).Trim();
        var html = new Markdown.Parser(markdown).ParseWith(new Markdown.Parsers.Html.Parser());
        
        data.Add("content", html);

        return data;
    }
    
    private static object ParseValue(string value)
    {
        if (value.StartsWith('"') && value.EndsWith('"'))
        {
            return value[1..^1];
        }

        if (value is "true" or "false")
        {
            return bool.Parse(value);
        }

        if (int.TryParse(value, out var intValue))
        {
            return intValue;
        }

        if (double.TryParse(value, out var doubleValue))
        {
            return doubleValue;
        }

        return value;
    }

    private static bool ConformsToStructure(string text)
    {
        var lines = text.Split("\r\n").Select(x => x.Trim()).ToArray();
        var yamlStart = Array.IndexOf(lines, "---");
        var yamlEnd = Array.IndexOf(lines, "---", 1);

        return yamlStart != -1 && yamlEnd != -1;
    }
}
