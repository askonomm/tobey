using Htmt.AttributeParsers;

namespace Htmt;

public record ParserConfig
{
    public required string Template;
    
    public Dictionary<string, object> Data { get; init; } = new();
    
    public IAttributeParser[] AttributeParsers { get; init; } =
    [
        new ForAttributeParser(),
        new HrefAttributeParser(),
        new InnerTextAttributeParser(),
        new InnerHtmlAttributeParser(),
        new OuterTextAttributeParser(),
        //new OuterHtmlAttributeParser(),
        new IfAttributeParser(),
        new UnlessAttributeParser(),
    ];
}