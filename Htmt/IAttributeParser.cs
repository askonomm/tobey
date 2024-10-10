using System.Xml;

namespace Htmt;

public interface IAttributeParser
{
    public void Parse(Parser parser, XmlNodeList? nodes);
    public string Name { get; }
}