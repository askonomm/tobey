namespace Tobey
{
    internal class Watcher
    {
        public static void Watch(string path, Action callback)
        {
            using var watcher = new FileSystemWatcher(path)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            watcher.Changed += (source, e) => OnChanged(source, e, callback);
            watcher.Created += (source, e) => OnCreated(source, e, callback);
            watcher.Deleted += (source, e) => OnDeleted(source, e, callback);
            watcher.Renamed += (source, e) => OnRenamed(source, e, callback);


            Console.ReadLine();
        }

        private static void OnChanged(object _, FileSystemEventArgs e, Action callback)
        {
            // Ignore output folder changes
            if (e.FullPath.Contains("output"))
            {
                return;
            }

            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

            callback();
        }

        private static void OnRenamed(object _, RenamedEventArgs e, Action callback)
        {
            // Ignore output folder changes
            if (e.FullPath.Contains("output"))
            {
                return;
            }

            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");

            callback();
        }

        private static void OnDeleted(object _, FileSystemEventArgs e, Action callback)
        {
            // Ignore output folder changes
            if (e.FullPath.Contains("output"))
            {
                return;
            }

            Console.WriteLine($"File: {e.FullPath} deleted");

            callback();
        }

        private static void OnCreated(object _, FileSystemEventArgs e, Action callback)
        {
            // Ignore output folder changes
            if (e.FullPath.Contains("output"))
            {
                return;
            }

            Console.WriteLine($"File: {e.FullPath} created");

            callback();
        }
    }
}
