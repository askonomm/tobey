namespace Markdown.Identifiers.Block;

public class HorizontalLine : IBlockIdentifier
{
    public string Name => "horizontal_line";
    
    public bool Identifies(string block)
    {
        var isDash = block.Trim().All(c => c == '-');
        var isAsterisk = block.Trim().All(c => c == '*');
        var isUnderscore = block.Trim().All(c => c == '_');
        
        return isDash || isAsterisk || isUnderscore;
    }
}