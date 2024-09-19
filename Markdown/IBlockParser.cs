namespace Markdown;

public interface IBlockParser
{
    public bool Identifies(string block);
    public string Parse(string block);
}