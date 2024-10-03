using System.Collections.Concurrent;

namespace Tobey
{
    internal class Compiler
    {
        public static void Compile(string path)
        {
            Console.WriteLine("Getting all content ...");
            var content = ComposeContent(path);

            Console.WriteLine("Attaching composer data ...");
            content = AttachComposerData(content);

            Console.WriteLine("Writing content ...");
            Write(path, content);
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

                if (item.TryGetValue("composer", out var composer))
                {
                    if (composer is Dictionary<string, object> composerDict)
                    {
                        var dc = new DataComposer(newContent);

                        // each key in composerDict corresponds to one DSL item
                        foreach (var key in composerDict.Keys)
                        {
                            var val = composerDict[key];

                            if (val is Dictionary<string, object> v)
                            {
                                lock (item)
                                {
                                    item.Add(key, dc.Compose(v));
                                }
                            }
                        }

                        newContent[i] = item;
                    }
                }
            });

            return newContent;
        }

        private static void Write(string path, List<Dictionary<string, object>> content)
        {
            Parallel.ForEach(content, item =>
            {
                if (item.TryGetValue("template", out var template))
                {
                    if (template is string templateStr)
                    {
                        var templatePath = Path.Combine(path, "layouts", templateStr);
                        var templateContent = File.ReadAllText(templatePath);
                        var handlebars = HandlebarsDotNet.Handlebars.Compile(templateContent);
                        var html = handlebars(item);

                        if (item.TryGetValue("path", out var output))
                        {
                            if (output is string writeTo)
                            {
                                // Create parent directories if they don't exist
                                var outputPath = Path.Combine(path, "output", writeTo);
                                var directoryPath = Path.GetDirectoryName(outputPath);

                                if (directoryPath != null)
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.WriteAllText(outputPath, html);
                                Console.WriteLine($"Created {writeTo}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: {item["full_path"]} does not have \"path\" FrontMatter key set.");
                        }
                    }
                }
            });
        }
    }
}
