using System.Text.RegularExpressions;
using System.Xml;

namespace Htmt.AttributeParsers;

public class InnerHtmlAttributeParser: IAttributeParser
{
    public string Name => "inner-html";
    
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

            var innerHtmlVal = n.GetAttribute("x:inner-html");
            if (string.IsNullOrEmpty(innerHtmlVal)) continue;
            
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(innerHtmlVal).Value;
            var keys = key.Split('.');
            
            if (parser.FindValueByKeys(keys) is not string strValue) continue;

            innerHtmlVal = wholeKeyRegex.Replace(innerHtmlVal, strValue);
            // convert innerHtmlVal to XML
            var xml = new XmlDocument();
            xml.LoadXml($"<root>{innerHtmlVal}</root>");
            n.InnerXml = xml.DocumentElement?.InnerXml ?? string.Empty;
            n.RemoveAttribute("x:inner-html");
        }
    }
}