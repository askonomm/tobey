namespace Markdown.Identifiers.Block;

public class HeadingBlock : IBlockIdentifier
{
    public string Name => "heading";
    
    public bool Identifies(string block)
    {
        return block.StartsWith('#');
    }
}