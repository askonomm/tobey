namespace MarkdownTests.Html.Parsers.Block;

[TestClass]
public class HorizontalLineTest
{
    [TestMethod]
    public void TestParsingDashes()
    {
        var horizontalLine = new Markdown.Parsers.Html.Parsers.Block.HorizontalLine();
        var result = horizontalLine.Parse("---");

        Assert.AreEqual("<hr />", result);
    }

    [TestMethod]
    public void TestParsingDashes2()
    {
        var horizontalLine = new Markdown.Parsers.Html.Parsers.Block.HorizontalLine();
        var result = horizontalLine.Parse("-----");

        Assert.AreEqual("<hr />", result);
    }

    [TestMethod]
    public void TestParsingAsterisks()
    {
        var horizontalLine = new Markdown.Parsers.Html.Parsers.Block.HorizontalLine();
        var result = horizontalLine.Parse("***");

        Assert.AreEqual("<hr />", result);
    }

    [TestMethod]
    public void TestParsingAsterisks2()
    {
        var horizontalLine = new Markdown.Parsers.Html.Parsers.Block.HorizontalLine();
        var result = horizontalLine.Parse("*****");

        Assert.AreEqual("<hr />", result);
    }

    [TestMethod]
    public void TestParsingUnderscores()
    {
        var horizontalLine = new Markdown.Parsers.Html.Parsers.Block.HorizontalLine();
        var result = horizontalLine.Parse("___");

        Assert.AreEqual("<hr />", result);
    }

    [TestMethod]
    public void TestParsingUnderscores2()
    {
        var horizontalLine = new Markdown.Parsers.Html.Parsers.Block.HorizontalLine();
        var result = horizontalLine.Parse("_____");

        Assert.AreEqual("<hr />", result);
    }
}
