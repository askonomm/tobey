using System.Collections.Concurrent;
using Htmt;

namespace Tobey;
    
public class Compiler
{
    public string Dir { get; init; } = ".";
    
    private List<Dictionary<string, object?>> _content = [];
    
    public void Compile()
    {
        Console.WriteLine("Getting all content ...");
        ComposeContent();

        Console.WriteLine("Attaching composer data ...");
        AttachComposerData();
        
        Console.WriteLine("Attaching templates as partials ...");
        AttachTemplatePartials();

        Console.WriteLine("Writing content ...");
        Write();
        
        Console.WriteLine("Moving assets ...");
        MoveAssets();
    }

    private void ComposeContent()
    {
        // get all .md files from path recursively
        var contentPath = Path.Combine(Dir, "content");
        var files = Directory.GetFiles(contentPath, "*.md", SearchOption.AllDirectories);

        if (files.Length == 0)
        {
            Console.WriteLine("No content found.");

            return;
        }

        // compose content of all files
        var content = new ConcurrentBag<Dictionary<string, object?>>();

        Parallel.ForEach(files, file =>
        {
            Console.WriteLine($"Parsing {file}");

            var fm = FrontMatter.Parser.Parse(File.ReadAllText(file));

            fm.Add("full_path", file);
            content.Add(fm);
        });

        _content = [.. content];
    }

    private void AttachComposerData()
    {
        var newContent = _content;

        Parallel.For(0, newContent.Count, i =>
        {
            var item = newContent[i];
            
            if (!item.TryGetValue("composer", out var composer) || composer is not Dictionary<string, object?> composerDict)
            {
                return;
            }
            
            var dc = new DataComposer(newContent);
            
            foreach (var key in composerDict.Keys)
            {
                if (composerDict[key] is not Dictionary<string, object?> v)
                {
                    continue;
                }
                
                lock (item)
                {
                    item.Add(key, dc.Compose(v));
                }
            }

            newContent[i] = item;
        });

        _content = newContent;
    }

    private void AttachTemplatePartials()
    {
        var templatesPath = Path.Combine(Dir, "templates");
        
        if (!Directory.Exists(templatesPath))
        {
            Console.WriteLine("No templates found.");
            return;
        }
        
        var files = Directory.GetFiles(templatesPath, "*", SearchOption.AllDirectories);
        Dictionary<string, object?> partials = new();

        foreach (var file in files)
        {
            var contents = File.ReadAllText(file);
            var relativePath = Path.GetRelativePath(templatesPath, file);
            var partialName = relativePath.Replace(Path.GetExtension(file), "");
            
            // replace forward or backward slashes with a dot
            partialName = partialName.Replace("/", ".").Replace("\\", ".");
            
            var partialKeys = partialName.Split('.');
            
            // add to `partials` as nested object based on the `partialKeys`
            // e.g ['a', 'b', 'c'] would be { a: { b: { c: contents } } }
            var current = partials;
            
            for (var i = 0; i < partialKeys.Length; i++)
            {
                var key = partialKeys[i];
                
                if (i == partialKeys.Length - 1)
                {
                    current.Add(key, contents);
                }
                else
                {
                    if (!current.TryGetValue(key, out var value))
                    {
                        value = new Dictionary<string, object?>();
                        current.Add(key, value);
                    }
                    
                    if (value is Dictionary<string, object?> dict)
                    {
                        current = dict;
                    }
                }
            }
        }
        
        foreach (var item in _content)
        {
            foreach (var partial in partials)
            {
                item.Add(partial.Key, partial.Value);
            }
        }
    }

    private void Write()
    {
        Parallel.ForEach(_content, item =>
        {
            if (!item.TryGetValue("template", out var template) || template is not string templateStr)
            {
                return;
            }
            
            if (!item.TryGetValue("path", out var output) || output is not string writeTo)
            {
                return;
            }
            
            var templatePath = Path.Combine(Dir, "templates", templateStr);
            
            if (!File.Exists(templatePath))
            {
                Console.WriteLine($"Template {templateStr} not found.");
                return;
            }
            
            var itemCopy = new Dictionary<string, object?>(item);
            var templateContent = File.ReadAllText(templatePath);
            var html = new Parser { Template = templateContent, Data = itemCopy }.ToHtml();
            var outputPath = Path.Combine(Dir, "output", writeTo);
            var directoryPath = Path.GetDirectoryName(outputPath);
            
            if (directoryPath != null)
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(outputPath, html);
            Console.WriteLine($"Created {writeTo}");
        });
    }
    
    private void MoveAssets()
    {
        var assetsPath = Path.Combine(Dir, "assets");
        var outputAssetsPath = Path.Combine(Dir, "output", "assets");
        
        if (!Directory.Exists(assetsPath))
        {
            Console.WriteLine("No assets found.");
            return;
        }
        
        if (!Directory.Exists(outputAssetsPath))
        {
            Directory.CreateDirectory(outputAssetsPath);
        }
        
        var files = Directory.GetFiles(assetsPath, "*", SearchOption.AllDirectories);
        
        Parallel.ForEach(files, file =>
        {
            var relativePath = Path.GetRelativePath(assetsPath, file);
            var outputPath = Path.Combine(outputAssetsPath, relativePath);
            var directoryPath = Path.GetDirectoryName(outputPath);
            
            if (directoryPath != null)
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            File.Copy(file, outputPath, true);
            Console.WriteLine($"Copied {relativePath}");
        });
    }
}