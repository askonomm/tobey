namespace MarkdownTests.Html.Parsers.Inline;

[TestClass]
public class LinkTest
{
    [TestMethod]
    public void TestSingleMatch()
    {
        var link = new Markdown.Html.Parsers.Inline.Link();
        var matches = link.Matches("[link](https://example.com)");

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("[link](https://example.com)", matches[0]);
    }

    [TestMethod]
    public void TestMultipleMatch()
    {
        var link = new Markdown.Html.Parsers.Inline.Link();
        var matches = link.Matches("[link](https://example.com) and [another link](https://example.com)");

        Assert.AreEqual(2, matches.Length);
        Assert.AreEqual("[link](https://example.com)", matches[0]);
        Assert.AreEqual("[another link](https://example.com)", matches[1]);
    }

    [TestMethod]
    public void TestImageLinkMatch()
    {
        var link = new Markdown.Html.Parsers.Inline.Link();
        var matches = link.Matches("![image](https://example.com)");

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("![image](https://example.com)", matches[0]);
    }

    [TestMethod]
    public void TestImageLinkWithAltTextMatch()
    {
        var link = new Markdown.Html.Parsers.Inline.Link();
        var matches = link.Matches("![image](https://example.com 'alt text')");

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("![image](https://example.com 'alt text')", matches[0]);
    }

    [TestMethod]
    public void TestNoMatch()
    {
        var link = new Markdown.Html.Parsers.Inline.Link();
        var matches = link.Matches("no match");

        Assert.AreEqual(0, matches.Length);
    }

    [TestMethod]
    public void TestLinkWithNoUrlMatch()
    {
        var link = new Markdown.Html.Parsers.Inline.Link();
        var matches = link.Matches("[link]");

        Assert.AreEqual(0, matches.Length);
    }

    [TestMethod]
    public void TestLinkParsing()
    {
        var link = new Markdown.Html.Parsers.Inline.Link();
        var result = link.Parse("[link](https://example.com)");

        Assert.AreEqual("<a href=\"https://example.com\">link</a>", result);
    }

}