using System.Xml;

namespace Htmt.AttributeParsers;

public class UnlessAttributeParser : IAttributeParser
{
    public string Name => "unless";
    
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

            var key = n.GetAttribute("x:unless");
            n.RemoveAttribute("x:unless");
            
            var value = parser.FindValueByKeys(key.Split('.'));
            
            if(value == null) continue;

            var removeNode = value switch
            {
                bool b => b,
                int i => i != 0,
                double d => d != 0,
                string s => !string.IsNullOrEmpty(s),
                _ => false
            };

            if (removeNode)
            {
                n.ParentNode?.RemoveChild(n);
            }
        }
    }
}