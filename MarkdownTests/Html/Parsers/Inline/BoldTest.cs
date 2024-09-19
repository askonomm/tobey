namespace MarkdownTests.Html.Parsers.Inline;

[TestClass]
public class BoldTest
{
    [TestMethod]
    public void TestSingleMatch()
    {
        var bold = new Markdown.Html.Parsers.Inline.Bold();
        var matches = bold.Matches("**bold**");

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("**bold**", matches[0]);
    }

    [TestMethod]
    public void TestMultipleMatches()
    {
        var bold = new Markdown.Html.Parsers.Inline.Bold();
        var matches = bold.Matches("**bold** and **another bold**");

        Assert.AreEqual(2, matches.Length);
        Assert.AreEqual("**bold**", matches[0]);
        Assert.AreEqual("**another bold**", matches[1]);
    }

    [TestMethod]
    public void TestDuplicateMatches()
    {
        var bold = new Markdown.Html.Parsers.Inline.Bold();
        var matches = bold.Matches("**bold** and **bold**");

        Assert.AreEqual(2, matches.Length);
        Assert.AreEqual("**bold**", matches[0]);
        Assert.AreEqual("**bold**", matches[1]);
    }

    [TestMethod]
    public void TestBoldMixedWithItalic()
    {
        var bold = new Markdown.Html.Parsers.Inline.Bold();
        var matches = bold.Matches("**bold** and *italic*");

        Assert.AreEqual(1, matches.Length);
        Assert.AreEqual("**bold**", matches[0]);
    }

    [TestMethod]
    public void TestNoMatch()
    {
        var bold = new Markdown.Html.Parsers.Inline.Bold();
        var matches = bold.Matches("no match");

        Assert.AreEqual(0, matches.Length);
    }
}