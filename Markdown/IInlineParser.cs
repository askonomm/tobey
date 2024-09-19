namespace Markdown;

public interface IInlineParser
{
    public string[] Matches(string block);
    
    public string Parse(string match);
}