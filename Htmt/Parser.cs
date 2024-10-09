using System.Diagnostics.CodeAnalysis;
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
    
    private const string HtmtNamespace = "http://www.w3.org/1999/xhtml";
    
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
        _nsManager.AddNamespace("x", HtmtNamespace);
        
        var templateStr = $"<root xmlns:x=\"{HtmtNamespace}\">{template}</root>";
        using var reader = XmlReader.Create(new StringReader(templateStr), _settings);
        _xml.Load(reader);
        
        // Traverse the XML tree and add unique ID to each node,
        // this is used to identify nodes in the template
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
        _nsManager.AddNamespace("x", HtmtNamespace);
        
        var templateStr = $"<root xmlns:x=\"{HtmtNamespace}\">{template}</root>";
        using var reader = XmlReader.Create(new StringReader(templateStr), _settings);
        _xml.Load(reader);
        
        // Traverse the XML tree and add unique ID to each node,
        // this is used to identify nodes in the template
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
        // Parse x:href attributes
        ParseHrefAttributes(data);
        
        // Parse x:inner-text attributes
        ParseInnerTextAttributes(data);
        
        // Parse x:inner-html attributes
        ParseInnerHtmlAttributes(data);
        
        // Parse x:outer-text attributes
        ParseOuterTextAttributes(data);
        
        // Parse x:if attributes
        ParseIfAttributes(data);
        
        // Parse x:unless attributes
        ParseUnlessAttributes(data);
        
        // Parse x:for attributes
        ParseForAttributes(data);
        
        // Parse Remove Outer nodes
        //ParseRemoveOuterAttributes();
    }

    private void ParseHrefAttributes(Dictionary<string, object> data)
    {
        // Add href attribute to all elements with htmt-href="{key}" attribute
        var selectedNodes = _doc?.SelectNodes("//*[@x:href]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }
        
        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;
            
            var val = n.GetAttribute("x:href");
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
            n.RemoveAttribute("x:href");
        }
    }
    
    private void ParseInnerTextAttributes(Dictionary<string, object> data)
    {
        // Add text attribute to all elements with htmt-text="{key}" attribute
        var selectedNodes = _doc?.SelectNodes("//*[@x:inner-text]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }

        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;

            var innerVal = n.GetAttribute("x:inner-text");
            if (string.IsNullOrEmpty(innerVal)) continue;
            
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(innerVal).Value;
            var keys = key.Split('.');
            
            if (FindValueByKeys(data, keys) is not string strValue) continue;

            innerVal = wholeKeyRegex.Replace(innerVal, strValue);
            n.InnerText = innerVal;
            n.RemoveAttribute("x:inner-text");
        }
    }
    
    private void ParseInnerHtmlAttributes(Dictionary<string, object> data)
    {
        // Add text attribute to all elements with htmt-text="{key}" attribute
        var selectedNodes = _doc?.SelectNodes("//*[@x:inner-html]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }

        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;

            var innerHtmlVal = n.GetAttribute("x:inner-html");
            if (string.IsNullOrEmpty(innerHtmlVal)) continue;
            
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(innerHtmlVal).Value;
            var keys = key.Split('.');
            
            if (FindValueByKeys(data, keys) is not string strValue) continue;

            innerHtmlVal = wholeKeyRegex.Replace(innerHtmlVal, strValue);
            // convert innerHtmlVal to XML
            var xml = new XmlDocument();
            xml.LoadXml($"<root>{innerHtmlVal}</root>");
            n.InnerXml = xml.DocumentElement?.InnerXml ?? string.Empty;
            n.RemoveAttribute("x:inner-html");
        }
    }
    
    public void ParseOuterTextAttributes(Dictionary<string, object> data)
    {
        // Add text attribute to all elements with htmt-text="{key}" attribute
        var selectedNodes = _doc?.SelectNodes("//*[@x:outer-text]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }

        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;

            var outerVal = n.GetAttribute("x:outer-text");
            if (string.IsNullOrEmpty(outerVal)) continue;
            
            var wholeKeyRegex = new Regex(@"\{.*?\}");
            var keyRegex = new Regex(@"(?<=\{)(.*?)(?=\})");
            var key = keyRegex.Match(outerVal).Value;
            var keys = key.Split('.');
            
            if (FindValueByKeys(data, keys) is not string strValue) continue;

            outerVal = wholeKeyRegex.Replace(outerVal, strValue);
            n.RemoveAttribute("x:outer-text");
            ReplaceNode(n, _xml.CreateTextNode(outerVal));
        }
    }

    private static object? FindValueByKeys(Dictionary<string, object> data, string[] keys)
    {
        while (true)
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
                case Dictionary<string, object> dict:
                {
                    var newKeys = keys.Skip(1).ToArray();

                    data = dict;
                    keys = newKeys;
                    continue;
                }
                case Dictionary<string, string> dict:
                {
                    var newKeys = keys.Skip(1).ToArray();

                    data = dict.ToDictionary(x => x.Key, x => (object)x.Value);
                    keys = newKeys;
                    continue;
                }
                case Dictionary<string, int> dict:
                {
                    var newKeys = keys.Skip(1).ToArray();

                    data = dict.ToDictionary(x => x.Key, x => (object)x.Value);
                    keys = newKeys;
                    continue;
                }
                case Dictionary<string, bool> dict:
                {
                    var newKeys = keys.Skip(1).ToArray();

                    data = dict.ToDictionary(x => x.Key, x => (object)x.Value);
                    keys = newKeys;
                    continue;
                }
                
                default:
                    return null;
            }
        }
    }

    private void ParseIfAttributes(Dictionary<string, object> data)
    {
        var selectedNodes = _doc?.SelectNodes("//*[@x:if]", _nsManager);

        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }

        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;

            var key = n.GetAttribute("x:if");
            n.RemoveAttribute("x:if");

            if (!data.TryGetValue(key, out var value)) continue;
            
            var removeNode = value switch
            {
                bool b => !b,
                int i => i == 0,
                string s => string.IsNullOrEmpty(s),
                _ => true
            };
            
            if (removeNode)
            {
                n.ParentNode?.RemoveChild(n);
            }
        }
    }

    private void ParseUnlessAttributes(Dictionary<string, object> data)
    {
        var selectedNodes = _doc?.SelectNodes("//*[@x:unless]", _nsManager);

        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }

        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            if (InsideForNode(n)) continue;

            var key = n.GetAttribute("x:unless");
            n.RemoveAttribute("x:unless");

            if (!data.TryGetValue(key, out var value)) continue;
            
            var removeNode = value switch
            {
                bool b => b,
                int i => i != 0,
                string s => !string.IsNullOrEmpty(s),
                _ => false
            };
            
            if (removeNode)
            {
                n.ParentNode?.RemoveChild(n);
            }
        }
    }

    private void ParseForAttributes(Dictionary<string, object> data)
    {
        var selectedNodes = _doc?.SelectNodes("//*[@x:for]", _nsManager);
        
        // No nodes found
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return;
        }
        
        foreach (var node in selectedNodes)
        {
            if (node is not XmlElement n) continue;
            
            var collection = n.GetAttribute("x:for");
            var asVar = n.GetAttribute("x:as");
            
            n.RemoveAttribute("x:for");
            n.RemoveAttribute("x:as");
            
            var value = FindValueByKeys(data, collection.Split('.'));
            if (value is not IEnumerable<object> enumerable) continue;

            // Create document fragment
            var fragment = _xml.CreateDocumentFragment();
            
            // Loop through value, and add data where asVar is the key
            foreach (var item in enumerable)
            {
                if (!string.IsNullOrEmpty(asVar))
                {
                    data[asVar] = item;
                }

                var parser = new Parser(n.OuterXml, _xml);
                var result = parser.ParseToXml(data);
                
                if (result == null) continue;
                
                fragment.AppendChild(result);
            }
            
            // Replace n child nodes with updatedNode child nodes
            ReplaceNode(n, fragment);
            
            // Remove the asVar key from the data dictionary
            data.Remove(asVar);
        }
    }

    private static bool InsideForNode(XmlElement node)
    {
        var parent = node.ParentNode;
        
        while (parent != null)
        {
            // if the parent is a node with the x:for attribute, return true
            if (parent is XmlElement e && e.HasAttribute("x:for"))
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