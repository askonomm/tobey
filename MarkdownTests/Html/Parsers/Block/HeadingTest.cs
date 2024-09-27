namespace MarkdownTests.Html.Parsers.Block;

[TestClass]
public class HeadingTest
{
    [TestMethod]
    public void TestH1Parsing()
    {
        var heading = new Markdown.Parsers.Html.Parsers.Block.Heading();
        var result = heading.Parse("# Heading 1");

        Assert.AreEqual("<h1>Heading 1</h1>", result);
    }

    [TestMethod]
    public void TestH2Parsing()
    {
        var heading = new Markdown.Parsers.Html.Parsers.Block.Heading();
        var result = heading.Parse("## Heading 2");

        Assert.AreEqual("<h2>Heading 2</h2>", result);
    }

    [TestMethod]
    public void TestH3Parsing()
    {
        var heading = new Markdown.Parsers.Html.Parsers.Block.Heading();
        var result = heading.Parse("### Heading 3");

        Assert.AreEqual("<h3>Heading 3</h3>", result);
    }

    [TestMethod]
    public void TestH4Parsing()
    {
        var heading = new Markdown.Parsers.Html.Parsers.Block.Heading();
        var result = heading.Parse("#### Heading 4");

        Assert.AreEqual("<h4>Heading 4</h4>", result);
    }

    [TestMethod]
    public void TestH5Parsing()
    {
        var heading = new Markdown.Parsers.Html.Parsers.Block.Heading();
        var result = heading.Parse("##### Heading 5");

        Assert.AreEqual("<h5>Heading 5</h5>", result);
    }
}
