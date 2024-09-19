namespace Markdown.Identifiers.Block;

public class HorizontalLine : IBlockIdentifier
{
    public string Name => "horizontal_line";
    
    public bool Identifies(int currentBlock, string[] blocks)
    {
        var content = blocks.ElementAt(currentBlock);
        var isDash = content.All(c => c == '-');
        var isAsterisk = content.All(c => c == '*');
        var isUnderscore = content.All(c => c == '_');
        
        return isDash || isAsterisk || isUnderscore;
    }
}