namespace Markdown
{
    public interface IParser<out T>
    {
        public T Parse(List<Block> blocks);
    }
}