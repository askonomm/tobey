namespace Markdown.Parsers.Html.Stitchers;

public class Code : IBlockStitcher
{
    public List<Block> Stitch(List<Block> blocks)
    {
        var newBlocks = new List<Block>();

        // Merge all consecutive "code" blocks together into one block
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks.ElementAt(i).Name != "code")
            {
                newBlocks.Add(blocks.ElementAt(i));
                continue;
            }

            var codeBlock = blocks.ElementAt(i);
            var codeContent = codeBlock.Content;

            for (int j = i + 1; j < blocks.Count; j++)
            {
                if (blocks.ElementAt(j).Name != "code")
                {
                    break;
                }

                codeContent += "\n\n";
                codeContent += blocks.ElementAt(j).Content;
                i++;
            }

            newBlocks.Add(new Block("code", codeContent));
        }

        return newBlocks;
    }
}
