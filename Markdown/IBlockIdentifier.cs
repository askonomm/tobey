namespace Markdown
{
    public interface IBlockIdentifier
    {
        public string Name { get; }

        public bool Identifies(int currentBlock, string[] blocks);
    }
}