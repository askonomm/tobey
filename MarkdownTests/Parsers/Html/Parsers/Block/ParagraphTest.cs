namespace MarkdownTests.Parsers.Html.Parsers.Block;

[TestClass]
public class ParagraphTest
{
    [TestMethod]
    public void TestParsing()
    {
        var paragraph = new Markdown.Parsers.Html.Parsers.Block.Paragraph();
        var result = paragraph.Parse("This is a paragraph.");
        Assert.AreEqual("<p>This is a paragraph.</p>", result);
    }

    [TestMethod]
    public void TestParsingWithMultipleLines()
    {
        var paragraph = new Markdown.Parsers.Html.Parsers.Block.Paragraph();
        var result = paragraph.Parse("This is a paragraph.\nThis is another paragraph.");
        Assert.AreEqual("<p>This is a paragraph.\nThis is another paragraph.</p>", result);
    }
}
