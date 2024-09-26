//var testFile = File.ReadAllText("C:\\Users\\askon\\Code\\Tobey\\Tobey\\test.md");
//var md = new Markdown.Parser(testFile);
//var html = md.ParseWith(new Markdown.Html.Parser());

var yaml = "key: \"value\"\r\nsomething:\r\n\thello: world";
var data = YAML.YAML.Parse(yaml);

Console.WriteLine(data.Count);