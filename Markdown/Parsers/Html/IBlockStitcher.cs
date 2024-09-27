namespace Markdown.Parsers.Html;

public interface IBlockStitcher
{
    List<Block> Stitch(List<Block> blocks);
}
