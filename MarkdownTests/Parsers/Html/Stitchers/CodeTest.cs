namespace MarkdownTests.Parsers.Html.Stitchers;

[TestClass]
public class CodeTest
{
    [TestMethod]
    public void TestCodeStitching()
    {
        var code = new Markdown.Parsers.Html.Stitchers.Code();
        var blocks = new List<Markdown.Block>
        {
            new("code", "var x = 1;"),
            new("code", "var y = 2;"),
            new("code", "var z = 3;")
        };

        var result = code.Stitch(blocks);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void TestCodeStitchingBetweenOtherBlocks()
    {
        var code = new Markdown.Parsers.Html.Stitchers.Code();
        var blocks = new List<Markdown.Block>
        {
            new("paragraph", "This is a paragraph."),
            new("code", "var x = 1;"),
            new("code", "var y = 2;"),
            new("paragraph", "This is another paragraph.")
        };

        var result = code.Stitch(blocks);

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void TestCodeStitchingMultipleCodeRanges()
    {
        var code = new Markdown.Parsers.Html.Stitchers.Code();
        var blocks = new List<Markdown.Block>
        {
            new("code", "var x = 1;"),
            new("code", "var y = 2;"),
            new("paragraph", "This is a paragraph."),
            new("code", "var z = 3;"),
            new("code", "var a = 4;"),
            new("code", "var b = 5;")
        };

        var result = code.Stitch(blocks);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(2, result.Count(b => b.Name == "code"));
    }
}
