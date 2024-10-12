namespace Tobey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--watch"))
            {
                Console.WriteLine("Watching...");
                Watcher.Watch(".", () => Compiler.Compile("."));
            }
            else
            {
                Compiler.Compile("C:\\Users\\askon\\Code\\faultd.com");
            }
        }
    }
}