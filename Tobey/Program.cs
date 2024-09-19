var testFile = File.ReadAllText("C:\\Users\\askon\\Code\\Tobey\\Tobey\\test.md");
var md = new Markdown.Parser(testFile);
var html = md.ParseWith(new Markdown.Html.Parser());

Console.WriteLine(html);