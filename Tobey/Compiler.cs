using System.Collections.Concurrent;
using Htmt;

namespace Tobey
{
    internal static class Compiler
    {
        public static void Compile(string path)
        {
            Console.WriteLine("Getting all content ...");
            var content = ComposeContent(path);

            Console.WriteLine("Attaching composer data ...");
            content = AttachComposerData(content);

            Console.WriteLine("Writing content ...");
            Write(path, content);
            
            Console.WriteLine("Moving assets ...");
            MoveAssets(path);
        }

        private static List<Dictionary<string, object>> ComposeContent(string path)
        {
            // get all .md files from path recursively
            var contentPath = Path.Combine(path, "content");
            var files = Directory.GetFiles(contentPath, "*.md", SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                Console.WriteLine("No content found.");

                return [];
            }

            // compose content of all files
            var content = new ConcurrentBag<Dictionary<string, object>>();

            Parallel.ForEach(files, file =>
            {
                Console.WriteLine($"Parsing {file}");

                var fm = FrontMatter.Parser.Parse(File.ReadAllText(file));

                fm.Add("full_path", file);
                content.Add(fm);
            });

            return [.. content];
        }

        private static List<Dictionary<string, object>> AttachComposerData(List<Dictionary<string, object>> content)
        {
            var newContent = content;

            Parallel.For(0, newContent.Count, i =>
            {
                var item = newContent[i];
                
                if (!item.TryGetValue("composer", out var composer) || composer is not Dictionary<string, object> composerDict)
                {
                    return;
                }
                
                var dc = new DataComposer(newContent);
                
                foreach (var key in composerDict.Keys)
                {
                    if (composerDict[key] is not Dictionary<string, object> v)
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

            return newContent;
        }

        private static void Write(string path, List<Dictionary<string, object>> content)
        {
            Parallel.ForEach(content, item =>
            {
                if (!item.TryGetValue("template", out var template) || template is not string templateStr)
                {
                    return;
                }
                
                if (!item.TryGetValue("path", out var output) || output is not string writeTo)
                {
                    return;
                }
                
                var templatePath = Path.Combine(path, "layouts", templateStr);
                
                if (!File.Exists(templatePath))
                {
                    Console.WriteLine($"Template {templateStr} not found.");
                    return;
                }
                
                var templateContent = File.ReadAllText(templatePath);
                var html = new Parser(templateContent).Parse(item);
                var outputPath = Path.Combine(path, "output", writeTo);
                var directoryPath = Path.GetDirectoryName(outputPath);
                
                if (directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(outputPath, html);
                Console.WriteLine($"Created {writeTo}");
            });
        }
        
        private static void MoveAssets(string path)
        {
            var assetsPath = Path.Combine(path, "assets");
            var outputAssetsPath = Path.Combine(path, "output", "assets");
            
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
}
