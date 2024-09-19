namespace Markdown.Identifiers.Block;

public class Code : IBlockIdentifier
{
    public string Name => "code";

    public bool Identifies(int currentBlock, string[] blocks)
    {
        // Does this block start with three backticks?
        if (blocks.ElementAt(currentBlock).StartsWith("```"))
        {
            // If it does, we identify it as a code block.
            return true;
        }

        // Does this block end with three backticks?
        if (blocks.ElementAt(currentBlock).EndsWith("```"))
        {
            // If it does, we identify it as a code block.
            return true;
        }

        // Is this block in the middle of blocks that start or end with three backticks?
        if (HasOpeningBlockBeforeIndex(currentBlock, blocks))
        {
            // If it is, we identify it as a code block.
            return true;
        }

        return false;
    }

    /**
     * This method checks if there is an opening code block before the current index.
     * That way we'll know if we're in the middle of a code block.
     */
    private static bool HasOpeningBlockBeforeIndex(int index, string[] blocks)
    {
        var open = false;

        for (var i = 0; i <= index; i++)
        {
            if (blocks.ElementAt(i).StartsWith("```"))
            {
                open = !open;
            }

            if (blocks.ElementAt(i).EndsWith("```"))
            {
                open = !open;
            }

            if (open && i == index)
            {
                return true;
            }
        }

        return false;
    }
}