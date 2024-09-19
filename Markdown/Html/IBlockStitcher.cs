namespace Markdown.Html;

public interface IBlockStitcher
{
    List<Block> Stitch(List<Block> blocks);
}
