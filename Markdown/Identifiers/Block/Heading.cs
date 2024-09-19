namespace Markdown.Identifiers.Block;

public class Heading : IBlockIdentifier
{
    public string Name => "heading";
    
    public bool Identifies(int currentBlock, string[] blocks)
    {
        return blocks.ElementAt(currentBlock).StartsWith("# ");
    }
}