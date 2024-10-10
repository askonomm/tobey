using System.Text.RegularExpressions;
using System.Xml;

namespace Htmt.AttributeParsers;

public class OuterTextAttributeParser : IAttributeParser
{
    public string Name => "outer-text";
    
    public void Parse(Parser parser, XmlNodeList? nodes)
    {
        // No nodes found
        if (nodes == null || nodes.Count == 0)
        {
            return;
        }

        foreach (var node in nodes)
        {
            if (node is not XmlElement n) continue;

            var outerVal = n.GetAttribute("x:outer-text");
            
            if (string.IsNullOrEmpty(outerVal)) continue;
            
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(outerVal).Value;
            var keys = key.Split('.');
            
            if (parser.FindValueByKeys(keys) is not string strValue) continue;

            outerVal = wholeKeyRegex.Replace(outerVal, strValue);
            n.RemoveAttribute("x:outer-text");
            n.ParentNode?.ReplaceChild(parser.Xml.CreateTextNode(outerVal), n);
        }
    }
}