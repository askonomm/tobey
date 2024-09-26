namespace Markdown;

public class Markdown(string markdown)
{
    /**
     * Returns a list of default block identifiers.
     */
    public static List<IBlockIdentifier> DefaultBlockIdentifiers()
    {
        return [
            new Identifiers.Block.Code(),
            new Identifiers.Block.List(),
            new Identifiers.Block.Heading(),
            new Identifiers.Block.HorizontalLine(),
            new Identifiers.Block.Paragraph(),
        ];
    }
    
    /**
     * Parses the markdown with the given parser.
     */
    public T ParseWith<T>(IParser<T> parser)
    {
        return ParseWith(parser, DefaultBlockIdentifiers());
    }
    
    /**
     * Parses the markdown with the given parser and block identifiers.
     */
    public T ParseWith<T>(IParser<T> parser, List<IBlockIdentifier> blockIdentifiers)
    {
        // Split the markdown into blocks
        // TODO: Make this work on non-Windows, too.
        var blocks = markdown.Split("\r\n\r\n");

        // Create a list of identified blocks
        var identifiedBlocks = new List<Block>();

        for(int i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            var identifier = blockIdentifiers.FirstOrDefault(x => x.Identifies(i, blocks));
            
            if (identifier == null) continue;
            
            identifiedBlocks.Add(new Block(identifier.Name, block.Trim()));
        }

        // Parse the identified blocks
        return parser.Parse(identifiedBlocks);
    }
}