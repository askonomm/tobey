namespace Tobey;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Contains("--watch"))
        {
            Console.WriteLine("Watching...");
            Watcher.Watch(".", () => new Compiler { Dir = "." }.Compile());
        }
        else
        {
            new Compiler { Dir = "." }.Compile();
        }
    }
}