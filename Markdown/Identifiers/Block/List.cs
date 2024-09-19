namespace Markdown.Identifiers.Block;

class List : IBlockIdentifier
{
    public string Name => "list";

    public bool Identifies(int currentBlock, string[] blocks)
    {
        var content = blocks[currentBlock];
        var firstLine = content.Split('\n')[0];
        var ordered = firstLine.StartsWith("1. ") || firstLine.StartsWith("1) ");
        var unordered = firstLine.StartsWith("- ") || firstLine.StartsWith("* ") || firstLine.StartsWith("+ ");

        return ordered || unordered;
    }
}