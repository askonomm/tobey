namespace Markdown.Parsers.Html
{
    public class Parser : IParser<string>
    {
        public string Parse(List<Block> blocks)
        {
            // Compose block parsers
            // TODO: make it configurable
            var blockParsers = new List<(string, IBlockParser)> {
            ("heading", new Parsers.Block.Heading()),
            ("horizontal_line", new Parsers.Block.HorizontalLine()),
            ("paragraph", new Parsers.Block.Paragraph()),
            ("code", new Parsers.Block.Code())
        };

            // Stitch blocks
            // TODO: make it configurable
            var blockStitchers = new List<IBlockStitcher> {
            new Stitchers.Code()
        };

            foreach (var stitcher in blockStitchers)
            {
                blocks = stitcher.Stitch(blocks);
            }

            // Parse blocks
            var html = "";

            foreach (var block in blocks)
            {
                foreach (var (name, parser) in blockParsers)
                {
                    if (block.Name != name) continue;

                    if (html != "") html += "\n\n";

                    html += parser.Parse(block.Content);
                    break;
                }
            }

            return html;
        }
    }
}