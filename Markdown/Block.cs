namespace Markdown;

public struct Block(string Name, string Content)
{
    public string Name { get; set; }
    public string Content { get; set;  }
}