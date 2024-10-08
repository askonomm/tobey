using System.Text.RegularExpressions;
using System.Xml;

namespace Htmt;

public class Parser
{
    private readonly XmlDocument _xml;
    private XmlElement? _doc;
    private readonly XmlNamespaceManager _nsManager;
    private readonly bool _isHtml;
    private readonly string _docType = string.Empty;
    
    private readonly XmlReaderSettings _settings = new()
    {
        IgnoreWhitespace = true,
        IgnoreComments = true,
        DtdProcessing = DtdProcessing.Ignore,
        ValidationType = ValidationType.None,
        XmlResolver = null
    };
    
    private const string HtmtNamespace = "http://htmt";
    
    public Parser(string template)
    {
        if (IsHtml(template))
        {
            _isHtml = true;
            _docType = GetDoctype(template);
            template = RemoveDoctype(template);
        }
        
        // Parse Template to XML representation
        _xml = new XmlDocument();
        _nsManager = new XmlNamespaceManager(_xml.NameTable);
        _nsManager.AddNamespace("htmt", "http://htmt");
        
        var templateStr = $"<root xmlns:htmt=\"{HtmtNamespace}\">{template}</root>";
        using var reader = XmlReader.Create(new StringReader(templateStr), _settings);
        _xml.Load(reader);
        
        // Traverse the XML tree and add unique ID to each node,
        // this is used to identify nodes in the template
        // and replace them with the correct data
        if (_xml.DocumentElement != null)
        {
            _doc = AddIdentifierToNodes(_xml.DocumentElement);
        }
    }
    
    private Parser(string template, XmlDocument xml)
    {
        if (IsHtml(template))
        {
            _isHtml = true;
            _docType = GetDoctype(template);
            template = RemoveDoctype(template);
        }
        
        // Parse Template to XML representation
        _xml = xml;
        _nsManager = new XmlNamespaceManager(_xml.NameTable);
        _nsManager.AddNamespace("htmt", "http://htmt");
        
        var templateStr = $"<root xmlns:htmt=\"{HtmtNamespace}\">{template}</root>";
        using var reader = XmlReader.Create(new StringReader(templateStr), _settings);
        _xml.Load(reader);
        
        // Traverse the XML tree and add unique ID to each node,
        // this is used to identify nodes in the template
        // and replace them with the correct data
        if (_xml.DocumentElement != null)
        {
            _doc = AddIdentifierToNodes(_xml.DocumentElement);
        }
    }
    
    private static bool IsHtml(string template)
    {
        // Any document that start with <!DOCTYPE*
        return template.Trim().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase);
    }
    
    private static string RemoveDoctype(string template)
    {
        var doctypeRegex = new Regex(@"<!DOCTYPE[^>]*>", RegexOptions.IgnoreCase);
        return doctypeRegex.Replace(template, string.Empty);
    }
    
    private static string GetDoctype(string template)
    {
        var doctypeRegex = new Regex(@"<!DOCTYPE[^>]*>", RegexOptions.IgnoreCase);
        var match = doctypeRegex.Match(template);
        
        return match.Success ? match.Value : string.Empty;
    }
    
    public string Parse(Dictionary<string, object> data)
    {
        if (_doc == null) return string.Empty;
        
        // Run mutations
        RunMutations(data);
        
        // Remove identifiers from nodes
        _doc = RemoveIdentifierFromNodes(_doc);
        
        // If we're in a HTML5 document, add back the doctype
        if (_isHtml)
        {
            return $"{_docType}{_doc.FirstChild?.OuterXml}";
        }
        
        // Return as HTML
        return _doc.FirstChild?.OuterXml ?? string.Empty;
    }
    
    private XmlNode? ParseToXml(Dictionary<string, object> data)
    {
        if (_doc == null) return _xml.CreateElement("root");
        
        // Run mutations
        RunMutations(data);
        
        // Remove identifiers from nodes
        _doc = RemoveIdentifierFromNodes(_doc);

        return _doc.FirstChild;
    }

    private void RunMutations(Dictionary<string, object> data)
    {
        // Parse Print nodes
        ParsePrintNodes(data);
        
        // Parse htmt-href attributes
        ParseHrefAttributes(data);
        
        // Parse htmt-text attributes
        ParseTextAttributes(data);
        
        // Parse If nodes
        ParseIfNodes(data);
        
        // Parse Unless nodes
        ParseUnlessNodes(data);
        
        // Parse For nodes
        ParseForNodes(data);
    }
    
    private void ParsePrintNodes(Dictionary<string, object> data)
    {
        // Replace all elements with a name of <htmt:print name="{key}" /> with the value of the key in the data dictionary
        // If the element is inside a loop (<htmt:for collection="{key}" as="{var}"></htmt:for>, do not replace it. 
        var selectedNodes = _doc?.SelectNodes("//htmt:print", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }
        
        foreach(var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;
            
            var key = n.GetAttribute("key");
            var keys = key.Split('.');
            
            // Traverse the data dictionary to find the correct value
            var value = FindValueByKeys(data, keys);
            
            switch (value)
            {
                case string strValue:
                    ReplaceNode(n, CreateTextNode(strValue));
                    break;
                case int intValue:
                    ReplaceNode(n, CreateTextNode(intValue.ToString()));
                    break;
            }
        }
    }

    private void ParseHrefAttributes(Dictionary<string, object> data)
    {
        // Add href attribute to all elements with htmt-href="{key}" attribute
        var selectedNodes = _doc?.SelectNodes("//*[@htmt-href]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }
        
        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;
            
            var val = n.GetAttribute("htmt-href");
            // get str between { and } using regex
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(val).Value;
            var keys = key.Split('.');
            
            // Traverse the data dictionary to find the correct value
            var value = FindValueByKeys(data, keys);

            if (value is not string strValue) continue;

            // replace {...} with strValue (including brackets) in val
            val = wholeKeyRegex.Replace(val, strValue);
            n.SetAttribute("href", val);
            n.RemoveAttribute("htmt-href");
        }
    }
    
    private void ParseTextAttributes(Dictionary<string, object> data)
    {
        // Add text attribute to all elements with htmt-text="{key}" attribute
        var selectedNodes = _doc?.SelectNodes("//*[@htmt-text]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }
        
        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;
            
            var val = n.GetAttribute("htmt-text");
            // get str between { and } using regex
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(val).Value;
            var keys = key.Split('.');
            
            // Traverse the data dictionary to find the correct value
            var value = FindValueByKeys(data, keys);

            if (value is not string strValue) continue;

            // replace {...} with strValue (including brackets) in val
            val = wholeKeyRegex.Replace(val, strValue);
            n.InnerText = val;
            n.RemoveAttribute("htmt-text");
        }
    }
    
    private static object? FindValueByKeys(Dictionary<string, object> data, string[] keys)
    {
        if (!data.TryGetValue(keys.First(), out var v))
        {
            return null;
        }
        
        if (keys.Length == 1)
        {
            return v;
        }

        switch (v)
        {
            // if is of type AnonymousType[], convert to Dictionary<string, object>
            case Dictionary<string, object> dict:
            {
                var newKeys = keys.Skip(1).ToArray();

                return FindValueByKeys(dict, newKeys);
            }
            case object[] arr:
            {
                var dict = new Dictionary<string, object>();
                for (var i = 0; i < arr.Length; i++)
                {
                    if (arr[i] is not Dictionary<string, object> d) continue;
                    
                    foreach (var (k, value) in d)
                    {
                        dict[$"{i}.{k}"] = value;
                    }
                }

                var newKeys = keys.Skip(1).ToArray();

                return FindValueByKeys(dict, newKeys);
            }
            case not null:
            {
                // if is of type AnonymousType, convert to Dictionary<string, object>
                if (v.GetType().Name.Contains("AnonymousType"))
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var property in v.GetType().GetProperties())
                    {
                        dict[property.Name] = property.GetValue(v) ?? string.Empty;
                    }

                    var newKeys = keys.Skip(1).ToArray();

                    return FindValueByKeys(dict, newKeys);
                }

                break;
            }
            default:
                return null;
        }
        
        return null;
    }

    private void ParseIfNodes(Dictionary<string, object> data)
    {
        var selectedNodes = _doc?.SelectNodes("//htmt:if", _nsManager);

        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }

        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;

            var key = n.GetAttribute("key");

            if (!data.TryGetValue(key, out var value)) continue;
            
            var isTrue = value switch
            {
                bool b => b,
                int i => i != 0,
                string s => !string.IsNullOrEmpty(s),
                _ => false
            };
            
            // Replace node with its children if the condition is true
            if (isTrue)
            {
                ReplaceNodeWithChildren(n);
            }
            else
            {
                // Remove node if the condition is false
                n.ParentNode?.RemoveChild(n);
            }

        }
    }

    private void ParseUnlessNodes(Dictionary<string, object> data)
    {
        var selectedNodes = _doc?.SelectNodes("//htmt:unless", _nsManager);

        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }

        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;

            var key = n.GetAttribute("key");

            if (!data.TryGetValue(key, out var value)) continue;
            
            var isFalse = value switch
            {
                bool b => !b,
                int i => i == 0,
                string s => string.IsNullOrEmpty(s),
                _ => false
            };
            
            // Replace node with its children if the condition is false
            if (isFalse)
            {
                ReplaceNodeWithChildren(n);
            }
            else
            {
                // Remove node if the condition is true
                n.ParentNode?.RemoveChild(n);
            }

        }
    }

    private void ParseForNodes(Dictionary<string, object> data)
    {
        var selectedNodes = _doc?.SelectNodes("//*[@htmt-for]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }
        
        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            
            var collection = n.GetAttribute("htmt-for");
            var asVar = n.GetAttribute("htmt-as");
            var value = FindValueByKeys(data, collection.Split('.'));
            
            if (value is not IEnumerable<object> enumerable) continue;
            
            var children = n.ChildNodes;
            
            // No point in continuing if there are no children
            if (children.Count == 0)
            {
                return;
            }

            // Create document fragment
            var updatedNode = _xml.CreateDocumentFragment();
            
            // Loop through value, and add data where asVar is the key
            foreach (var item in enumerable)
            {
                data[asVar] = item;
                
                // Create a new node that contains all the children
                var childrenContainer = _xml.CreateDocumentFragment();
                
                foreach (XmlNode child in children)
                {
                    childrenContainer.AppendChild(child.CloneNode(true));
                }
                
                var parser = new Parser(childrenContainer.OuterXml, _xml);
                var result = parser.ParseToXml(data);
                
                if (result == null) continue;
                
                updatedNode.AppendChild(result);
            }
            
            // Replace n child nodes with updatedNode child nodes
            SwapChildren(n, updatedNode);
            
            // Remove htmt-for and htmt-as attributes
            n.RemoveAttribute("htmt-for");
            n.RemoveAttribute("htmt-as");
            
            // Remove the asVar key from the data dictionary
            data.Remove(asVar);
        }
    }
    
    private static void SwapChildren(XmlElement node, XmlNode replacement)
    {
        var children = node.ChildNodes;
        var replacementChildren = replacement.ChildNodes;
        
        // Remove all children from node
        for (var i = children.Count - 1; i >= 0; i--)
        {
            node.RemoveChild(children[i] ?? node.OwnerDocument.CreateTextNode(""));
        }
        
        // Add all children from replacement to node
        for (var i = 0; i < replacementChildren.Count; i++)
        {
            node.AppendChild(replacementChildren[i] ?? node.OwnerDocument.CreateTextNode(""));
        }
    }

    private static bool InsideForNode(XmlElement node)
    {
        var parent = node.ParentNode;
        
        while (parent != null)
        {
            // of the parent is a node with the htmt-for attribute, return true
            if (parent is XmlElement e && e.HasAttribute("htmt-for"))
            {
                return true;
            }
            
            parent = parent.ParentNode;
        }
        
        return false;
    }

    private static XmlElement AddIdentifierToNodes(XmlElement doc)
    {
        foreach (XmlNode node in doc.ChildNodes)
        {
            if (node is not XmlElement element) continue;
            
            var uuid = Guid.NewGuid().ToString();
            element.SetAttribute("data-htmt-id", uuid);
            
            if (element.HasChildNodes)
            {
                AddIdentifierToNodes(element);
            }
        }
        
        return doc;
    }

    private static XmlElement RemoveIdentifierFromNodes(XmlElement doc)
    {
        foreach (XmlNode node in doc.ChildNodes)
        {
            if (node is not XmlElement element) continue;
            
            element.RemoveAttribute("data-htmt-id");
            
            if (element.HasChildNodes)
            {
                RemoveIdentifierFromNodes(element);
            }
        }
        
        return doc;
    }
    
    private XmlText CreateTextNode(string value)
    {
        var textNode = _xml.CreateTextNode(value);
        
        return textNode;
    }
    
    private void ReplaceNode(XmlElement node, XmlNode replacement)
    {
        // find node in this.doc with the same htmt-id as node, and replace it with value
        var id = node.GetAttribute("data-htmt-id");
        var target = _doc?.SelectSingleNode($"//*[@data-htmt-id='{id}']", _nsManager);

        target?.ParentNode?.ReplaceChild(replacement, target);
    }
    
    private void ReplaceNodeWithChildren(XmlElement node)
    {
        var children = node.ChildNodes;
        var parent = node.ParentNode;

        if (children.Count == 0)
        {
            return;
        }

        for (var i = 0; i < children.Count; i++)
        {
            parent?.InsertBefore(children[i] ?? _xml.CreateTextNode(""), node);
        }

        parent?.RemoveChild(node);
    }
}