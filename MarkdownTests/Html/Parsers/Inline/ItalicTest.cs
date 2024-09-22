namespace MarkdownTests.Html.Parsers.Inline;

[TestClass]
public class ItalicTest
{
    [TestMethod]
    public void TestSingleMatch()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("*italic*");

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("*italic*", matches[0]);
    }

    [TestMethod]
    public void TestMultipleMatches()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("*italic* and *another italic*");

        Assert.AreEqual(2, matches.Length);
        Assert.AreEqual("*italic*", matches[0]);
        Assert.AreEqual("*another italic*", matches[1]);
    }

    [TestMethod]
    public void TestDuplicateMatches()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("*italic* and *italic*");

        Assert.AreEqual(2, matches.Length);
        Assert.AreEqual("*italic*", matches[0]);
        Assert.AreEqual("*italic*", matches[1]);
    }

    [TestMethod]
    public void TestItalicMixedWithBold()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("*italic* and **bold**");

        
        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("*italic*", matches[0]);
    }

    [TestMethod]
    public void TestAlternativeItalic()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("_italic_");

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("_italic_", matches[0]);
    }

    [TestMethod]
    public void TestNoMatch()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("no match");

        Assert.AreEqual(0, matches.Length);
    }

    [TestMethod]
    public void TestItalicWithNoClosingMatch()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("*italic");

        Assert.AreEqual(0, matches.Length);
    }

    [TestMethod]
    public void TestItalicWithNoOpeningMatch()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("italic*");

        Assert.AreEqual(0, matches.Length);
    }

    [TestMethod]
    public void TestItalicParsing()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var html = italic.Parse("*italic*");

        Assert.AreEqual("<em>italic</em>", html);
    }

    [TestMethod]
    public void TestAlternativeItalicParsing()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var html = italic.Parse("_italic_");

        Assert.AreEqual("<em>italic</em>", html);
    }

    [TestMethod]
    public void TestEnsureNoCrossOverWithBoldMatches()
    {
        var italic = new Markdown.Html.Parsers.Inline.Italic();
        var matches = italic.Matches("Match _italic_ and *italic* but not ** and not __ and not **bold** and not __bold__. ");

        Assert.AreEqual(2, matches.Length);
        Assert.AreEqual("_italic_", matches[0]);
        Assert.AreEqual("*italic*", matches[1]);
    }
}