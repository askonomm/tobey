using Markdown.Parsers.Html.Parsers;

namespace Markdown.Parsers.Html.Parsers.Block;

/// <summary>
/// List block parser. Supports both ordered and unordered lists,
/// as well as nested lists.
/// </summary>
public class List : IBlockParser
{
    /// <summary>
    /// Default inline parsers for the list block.
    /// </summary>
    /// <returns></returns>
    public List<IInlineParser> DefaultInlineParsers()
    {
        return
        [
            new Inline.Italic(),
            new Inline.Bold(),
            new Inline.Link(),
            new Inline.Code(),
        ];
    }

    /// <summary>
    /// Parse a Markdown list block to an HTML list.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public string Parse(string block)
    {
        return Parse(block, DefaultInlineParsers());
    }

    /// <summary>
    /// Parse a Markdown list block to an HTML list.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="inlineParsers"></param>
    /// <returns></returns>
    public string Parse(string block, List<IInlineParser> inlineParsers)
    {
        var lines = block.Split('\n');
        var list = LinesToList(lines);
        
        return ListToHtml(list, inlineParsers);
    }
    
    /// <summary>
    /// Recursively convert a list of tuples representing a markdown list to an HTML list.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="inlineParsers"></param>
    /// <returns></returns>
    private static string ListToHtml(List<(int, string)> list, List<IInlineParser> inlineParsers)
    {
        var html = "";
        
        for (var i = 0; i < list.Count; i++)
        {
            var (indentLevel, value) = list[i];
            var rootListType = value.StartsWith("- ") ? "ul" : "ol";
            var nextIndentLevel = i + 1 < list.Count ? list[i + 1].Item1 : 0;
            var tab = new string('\t', indentLevel * 2);

            if (i == 0)
            {
                html += $"{tab}<{rootListType}>\n";
            }

            html += $"{tab}\t<li>{ParseValue(value, inlineParsers)}";
            
            // If the next indent level is greater than the current one, then we have a nested list.
            if (nextIndentLevel > indentLevel)
            {
                // get all the items from next until the next item with the same indent level
                var sublist = FindItemsUntil(list.Skip(i + 1).ToList(), indentLevel);
 
                html += $"\n{ListToHtml(sublist, inlineParsers)}";
   
                // remove the sublist from the list
                i += sublist.Count;
                
                html += $"{tab}\t</li>\n";
            }
            else
            {
                html += "</li>\n";
            }
            
            // If final item and not the root list, close the list.
            if (i == list.Count - 1 && indentLevel > 0)
            {
                html += $"{tab}</{rootListType}>\n";
            }
            
            // If final item and the root list, close the list.
            if (i == list.Count - 1 && indentLevel == 0)
            {
                html += $"</{rootListType}>";
            }
        }
        
        return html;
    }
    
    /// <summary>
    /// Parse the value of a list item to HTML.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inlineParsers"></param>
    /// <returns></returns>
    private static string ParseValue(string value, List<IInlineParser> inlineParsers)
    {
        // Remove list markers by removing everything up to the first space.
        value = value[value.IndexOf(' ')..];
        
        return InlineParser.Parse(value, inlineParsers).Trim();
    }
    
    /// <summary>
    /// Finds all items in a list of tuples until the next item with the given indent level.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="indentLevel"></param>
    /// <returns></returns>
    private static List<(int, string)> FindItemsUntil(List<(int, string)> list, int indentLevel)
    {
        var sublist = new List<(int, string)>();
        
        foreach (var (level, value) in list)
        {
            if (level == indentLevel)
            {
                break;
            }
            
            sublist.Add((level, value));
        }
        
        return sublist;
    }

    /// <summary>
    /// For a list of lines, each representing a list markdown item, but ones
    /// that can potentially be nested by indentation, compose a tree structure.
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    private static List<(int, string)> LinesToList(string[] lines)
    {
        // Compose leveled list of tuples where the first element is the indent level, 
        // and the second element is the value of the list item.
        var list = new List<(int, string)>();

        foreach (var line in lines)
        {
            var lineIndentLevel = line.TakeWhile(c => c == ' ').Count() / 2;
            var value = line.Trim();
            
            list.Add((lineIndentLevel, value));
        }

        return list;
    }
}
        