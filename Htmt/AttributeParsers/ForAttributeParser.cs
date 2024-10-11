using System.Xml;

namespace Htmt.AttributeParsers;

public class ForAttributeParser : IAttributeParser
{
    public string Name => "for";
    
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
            
            var collection = n.GetAttribute("x:for");
            var asVar = n.GetAttribute("x:as");
            
            n.RemoveAttribute("x:for");
            n.RemoveAttribute("x:as");
            
            var value = Helper.FindValueByKeys(data, collection.Split('.'));
            if (value is not IEnumerable<object> enumerable) continue;

            var fragment = xml.CreateDocumentFragment();

            foreach (var item in enumerable)
            {
                var iterationData = new Dictionary<string, object>(data);
                
                if (!string.IsNullOrEmpty(asVar))
                {
                    iterationData[asVar] = item;
                }
                
                var iterationParser = new Parser { Template = n.OuterXml, Data = iterationData};
                var itemXml = iterationParser.ToXml();
                var importedNode = xml.ImportNode(itemXml, true);
                
                fragment.AppendChild(importedNode);
            }
            
            n.ParentNode?.ReplaceChild(fragment, n);
        }
    }
}