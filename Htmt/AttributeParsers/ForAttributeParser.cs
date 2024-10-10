using System.Xml;

namespace Htmt.AttributeParsers;

public class ForAttributeParser : IAttributeParser
{
    public string Name => "for";
    
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
            
            var collection = n.GetAttribute("x:for");
            var asVar = n.GetAttribute("x:as");
            
            n.RemoveAttribute("x:for");
            n.RemoveAttribute("x:as");
            
            var value = parser.FindValueByKeys(collection.Split('.'));
            if (value is not IEnumerable<object> enumerable) continue;

            var fragment = parser.Xml.CreateDocumentFragment();
            var outer = n.OuterXml;

            foreach (var item in enumerable)
            {
                var data = new Dictionary<string, object>(parser.Data);
                
                if (!string.IsNullOrEmpty(asVar))
                {
                    data[asVar] = item;
                }
                
                var iterationParser = new Parser { Template = outer, Data = data};
                var itemXml = iterationParser.ToXml();
                
                if (itemXml == null) continue;
                
                var importedNode = parser.Xml.ImportNode(itemXml, true);
                fragment.AppendChild(importedNode);
            }
            
            n.ParentNode?.ReplaceChild(fragment, n);
        }
    }
}