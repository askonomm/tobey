namespace Markdown.Identifiers.Block;

public class ParagraphBlock : IBlockIdentifier
{
    public string Name => "paragraph";
    
    public bool Identifies(string block)
    {
        // It may seem as if we always match the block, but we don't.
        // We only match the block if it doesn't match any other block, 
        // because the paragraph block is the default block, and it sits 
        // last in the list of block identifiers.
        return true;
    }
}