namespace Tobey;

public class Watcher
{
    private static Timer? _debounceTimer;
    
    public static void Watch(string path, Action callback)
    {
        using var watcher = new FileSystemWatcher(path);
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        watcher.Changed += (source, e) => OnChanged(path, source, e, callback);
        watcher.Created += (source, e) => OnCreated(path, source, e, callback);
        watcher.Deleted += (source, e) => OnDeleted(path, source, e, callback);
        watcher.Renamed += (source, e) => OnRenamed(path, source, e, callback);
        
        Console.ReadLine();
    }

    private static bool IgnoreFile(string path, string fullPath, string? name)
    {
        if (!path.EndsWith(Path.DirectorySeparatorChar))
        {
            path += Path.DirectorySeparatorChar;
        }
        
        var fullPathDir = Path.GetDirectoryName(fullPath) ?? "";
        var pathDir = Path.GetDirectoryName(path) ?? "";
        var relativeDir = fullPathDir?.Replace(pathDir, "").TrimStart(Path.DirectorySeparatorChar);

        return name?.StartsWith('.') == true || relativeDir?.StartsWith("output") == true;
    }

    private static void OnChanged(string path, object _, FileSystemEventArgs e, Action callback)
    {
        if (IgnoreFile(path, e.FullPath, e.Name)) return;

        Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ => callback(), null, 500, Timeout.Infinite);
    }

    private static void OnRenamed(string path, object _, RenamedEventArgs e, Action callback)
    {
        if (IgnoreFile(path, e.FullPath, e.Name)) return;

        Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");

        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ => callback(), null, 500, Timeout.Infinite);
    }

    private static void OnDeleted(string path, object _, FileSystemEventArgs e, Action callback)
    {
        if (IgnoreFile(path, e.FullPath, e.Name)) return;

        Console.WriteLine($"File: {e.FullPath} deleted");

        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ => callback(), null, 500, Timeout.Infinite);
    }

    private static void OnCreated(string path, object _, FileSystemEventArgs e, Action callback)
    {
        if (IgnoreFile(path, e.FullPath, e.Name)) return;

        Console.WriteLine($"File: {e.FullPath} created");

        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ => callback(), null, 500, Timeout.Infinite);
    }
}