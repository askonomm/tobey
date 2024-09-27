namespace Tobey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--watch"))
            {
                Console.WriteLine("Watching...");
            }
            else
            {
                Console.WriteLine("Not watching...");
            }
        }
    }
}