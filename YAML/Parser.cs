﻿using System.Text;

namespace YAML;

public class Parser
{
    private static string ParseMultilineValue(StringReader reader, int indentLevel)
    {
        var val = new StringBuilder();
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            var currentIndent = line.TakeWhile(Char.IsWhiteSpace).Count();

            // Stop when there's less indendation
            if (currentIndent < indentLevel)
            {
                reader.ReadLine();
                break;
            }

            val.AppendLine(line.Trim());
        }

        // Remove the last newline
        if (val.Length > 0)
        {
            val.Remove(val.Length - 1, 1);
        }

        return val.ToString();
    }

    private static List<Node> ParseBlock(StringReader reader, int indentLevel)
    {
        var nodes = new List<Node>();
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            var currentIndent = line.TakeWhile(Char.IsWhiteSpace).Count();

            // Skip empty lines
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Skip comments
            if (line.Trim().StartsWith("#"))
            {
                continue;
            }

            // If the current line is less indented, it belongs to a previous block
            if (currentIndent < indentLevel)
            {
                reader.ReadLine();
                break;
            }

            line = line.Trim();
            if (line == string.Empty) continue;

            // List item (belongs to the previous node)
            if (line.StartsWith('-'))
            {
                nodes.Add(new Node
                {
                    Key = "",
                    Value = line.Substring(1).Trim()
                });

                continue;
            }

            var colonPos = line.IndexOf(':');

            if (colonPos == -1)
            {
                throw new Exception("Invalid line: " + line);
            }

            var key = line[..colonPos].Trim();
            var val = line[(colonPos + 1)..].Trim();
            var nextIndent = currentIndent + 1;

            // Nested block
            if (string.IsNullOrEmpty(val) && (reader.Peek() == ' ' || reader.Peek() == '\t'))
            {
                nodes.Add(new Node
                {
                    Key = key,
                    Value = ParseBlock(reader, nextIndent)
                });

                continue;
            }

            // Multiline string
            if (val == "|")
            {
                nodes.Add(new Node
                {
                    Key = key,
                    Value = ParseMultilineValue(reader, nextIndent)
                });

                continue;
            }

            // Single value
            if (val.StartsWith('"') && val.EndsWith('"'))
            {
               val = val[1..^1];
            }

            nodes.Add(new Node
            {
                Key = key,
                Value = val
            });
        }

        return nodes;
    }

    public static List<Node> Parse(string input)
    {
        using var reader = new StringReader(input);

        return ParseBlock(reader, 0);
    }

    public static Node? FindMaybeNode(List<Node> nodes, string key)
    {
        var stack = new Stack<List<Node>>();
        stack.Push(nodes);

        while (stack.Count > 0)
        {
            var currentNodes = stack.Pop();

            foreach (var node in currentNodes)
            {
                if (node.Key == key)
                {
                    return node;
                }

                if (node.Value is List<Node> list)
                {
                    stack.Push(list);
                }
            }
        }

        return null;
    }

    public static string FindMaybeStr(List<Node> nodes, string key)
    {
        var node = FindMaybeNode(nodes, key);

        return node?.Value as string ?? string.Empty;
    }
}