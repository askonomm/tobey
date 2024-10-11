using System.Text.RegularExpressions;
using System.Xml;

namespace Htmt.AttributeParsers;

public partial class InnerTextAttributeParser : IAttributeParser
{
    public string Name => "inner-text";
    
    [GeneratedRegex(@"\{.*?\}")]
    private static partial Regex WholeKeyRegex();
    
    [GeneratedRegex(@"(?<=\{)(.*?)(?=\})")]
    private static partial Regex KeyRegex();
    
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

            var innerVal = n.GetAttribute("x:inner-text");
            if (string.IsNullOrEmpty(innerVal)) continue;
            
            var wholeKeyRegex = WholeKeyRegex();
            var keyRegex = KeyRegex();
            var key = keyRegex.Match(innerVal).Value;
            var keys = key.Split('.');
            
            if (Helper.FindValueByKeys(data, keys) is not string strValue) continue;

            innerVal = wholeKeyRegex.Replace(innerVal, strValue);
            n.InnerText = innerVal;
            n.RemoveAttribute("x:inner-text");
        }
    }
}