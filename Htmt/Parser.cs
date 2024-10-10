using System.Text.RegularExpressions;
using System.Xml;
using Htmt.AttributeParsers;

namespace Htmt;

public class Parser
{
    public XmlDocument Xml { get; } = new();

    public string Template { get; init; } = string.Empty;

    public Dictionary<string, object> Data { get; init; } = new();

    public IAttributeParser[]? AttributeParsers { get; init; }
    
    private XmlNamespaceManager _nsManager = null!;
    
    private bool _isHtml;
    
    private string _docType = string.Empty;
    
    private readonly XmlReaderSettings _xmlSettings = new()
    {
        IgnoreWhitespace = true,
        IgnoreComments = true,
        DtdProcessing = DtdProcessing.Ignore,
        ValidationType = ValidationType.None,
        XmlResolver = null
    };
    
    private const string HtmtNamespace = "http://www.w3.org/1999/xhtml";

    private void Parse()
    {
        _nsManager = new XmlNamespaceManager(Xml.NameTable);
        _nsManager.AddNamespace("x", HtmtNamespace);
        
        if (IsHtml(Template))
        {
            _isHtml = true;
            _docType = GetDoctype(Template ?? string.Empty);
        }
        
        var templateWithoutDoctype = RemoveDoctype(Template ?? string.Empty);
        var templateStr = $"<root xmlns:x=\"{HtmtNamespace}\">{templateWithoutDoctype}</root>";
        using var reader = XmlReader.Create(new StringReader(templateStr), _xmlSettings);
        Xml.Load(reader);
        
        AddIdentifierToNodes();
        RunAttributeParsers();
        RemoveIdentifierFromNodes();
    }

    public static IAttributeParser[] DefaultAttributeParsers()
    {
        return
        [
            new InnerTextAttributeParser(),
            new OuterTextAttributeParser(),
            new InnerHtmlAttributeParser(),
            new HrefAttributeParser(),
            new IfAttributeParser(),
            new UnlessAttributeParser(),
            new ForAttributeParser()
        ];
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
    
    public string ToHtml()
    {
        Parse();
        
        if (Xml.DocumentElement == null) return string.Empty;
        
        if (_isHtml)
        {
            return $"{_docType}{Xml.DocumentElement.FirstChild?.OuterXml}";
        }
        
        return Xml.DocumentElement.FirstChild?.OuterXml ?? string.Empty;
    }
    
    public XmlNode? ToXml()
    {
        Parse();
        
        return Xml.DocumentElement?.FirstChild ?? Xml.CreateElement("root");
    }
    
    private void RunAttributeParsers()
    {
        var parsers = AttributeParsers;
        
        if (parsers == null || parsers.Length == 0)
        {
            parsers = DefaultAttributeParsers();
        }
        
        foreach(var parser in parsers)
        {
            var nodes = Xml.DocumentElement?.SelectNodes($"//*[@x:{parser.Name}]", _nsManager);
            
            parser.Parse(this, nodes);
        }
    }
    
    public object? FindValueByKeys(string[] keys)
    {
        var data = new Dictionary<string, object>(Data);
        
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

    private void AddIdentifierToNodes()
    {
        if (Xml.DocumentElement == null) return;
        
        var nodesToProcess = new Queue<XmlNode>(Xml.DocumentElement.ChildNodes.Cast<XmlNode>());

        while (nodesToProcess.Count > 0)
        {
            var node = nodesToProcess.Dequeue();
            if (node is not XmlElement element) continue;

            var uuid = Guid.NewGuid().ToString();
            element.SetAttribute("data-htmt-id", uuid);

            foreach (XmlNode child in element.ChildNodes)
            {
                nodesToProcess.Enqueue(child);
            }
        }
    }

    private void RemoveIdentifierFromNodes()
    {
        // null check
        if (Xml.DocumentElement == null) return;

        var nodesToProcess = new Queue<XmlNode>(Xml.DocumentElement.ChildNodes.Cast<XmlNode>());

        while (nodesToProcess.Count > 0)
        {
            var node = nodesToProcess.Dequeue();
            if (node is not XmlElement element) continue;

            element.RemoveAttribute("data-htmt-id");

            foreach (XmlNode child in element.ChildNodes)
            {
                nodesToProcess.Enqueue(child);
            }
        }
    }
}