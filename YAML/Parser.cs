namespace YAML;

public class Parser
{
    private Dictionary<string, Data> parseBlock(string[] lines, int indent)
    {
        var data = new Dictionary<string, Data>();

        for (var i = 0; indent < lines.Length; i++)
        {
            var line = lines[i];
            var currentIndent = line.LastIndexOf('\t');
            var nextIndent = i + 1 < lines.Length ? lines[i + 1].LastIndexOf('\t') : 0;

            // Skip empty lines
            if (line.Trim() == "")
            {
                continue;
            }

            // If the current line is less indented, it belongs to a previous block
            if (currentIndent < indent)
            {
                break;
            }

            // List item - belongs to the previously created node
            if (line.Trim().StartsWith('-'))
            {
                var last_key = data.Last().Key;
                var last_val = data.Last().Value;
                var line_val = line.Trim().Substring(1).Trim();

                // If the value is already an array, append the new value to it
                if (last_val is Data.Array arrayVal)
                {
                    var array = arrayVal.Value;
                    array.Add(line_val);

                    data[last_key] = new Data.Array(last_key, array);
                }

                // Otherwise let's create a new array
                else
                {
                    data[last_key] = new Data.Array(last_key, [line_val]);
                }

                continue;
            }

            var key = line.Trim().Split(':')[0];
            var value = line.Trim().Split(':')[1].Trim();

            // Nested block
            if (nextIndent > indent && value.EndsWith('\t'))
            {
                var block = parseBlock(lines, nextIndent);

                data.Add(key, new Data.Object(key, block));
            }

        }

        return data;
    }

    public Dictionary<string, Data> Parse(string yaml)
    {
        var lines = yaml.Split("\r\n");

        return parseBlock(lines, 0);
    }
}
