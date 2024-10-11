using System.Text.RegularExpressions;
using System.Xml;

namespace Htmt.AttributeParsers;

public class HrefAttributeParser : IAttributeParser
{
    public string Name => "href";

    public void Parse(XmlDocument xml, Dictionary<string, object> data, XmlNodeList? nodes)
    {
        // No nodes found
        if (nodes == null || nodes.Count == 0)
        {
            return;
        }
        
        foreach (var node in nodes)
        {
            if (node is not XmlElement n) continue;
            
            var val = n.GetAttribute("x:href");
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(val).Value;
            var keys = key.Split('.');
            
            var value = Helper.FindValueByKeys(data, keys);
            
            if (value is not string strValue)
            {
                strValue = value?.ToString() ?? string.Empty;
            }

            val = wholeKeyRegex.Replace(val, strValue);
            n.SetAttribute("href", val);
            n.RemoveAttribute("x:href");
        }
    }
}