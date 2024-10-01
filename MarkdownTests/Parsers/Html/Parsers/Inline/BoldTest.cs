namespace MarkdownTests.Parsers.Html.Parsers.Inline
{
    [TestClass]
    public class BoldTest
    {
        [TestMethod]
        public void TestSingleMatch()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("**bold**");

            Assert.AreEqual(1, matches.Length);
            Assert.AreEqual("**bold**", matches[0]);
        }

        [TestMethod]
        public void TestMultipleMatches()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("**bold** and **another bold**");

            Assert.AreEqual(2, matches.Length);
            Assert.AreEqual("**bold**", matches[0]);
            Assert.AreEqual("**another bold**", matches[1]);
        }

        [TestMethod]
        public void TestDuplicateMatches()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("**bold** and **bold**");

            Assert.AreEqual(2, matches.Length);
            Assert.AreEqual("**bold**", matches[0]);
            Assert.AreEqual("**bold**", matches[1]);
        }

        [TestMethod]
        public void TestBoldMixedWithItalic()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("**bold** and *italic*");

            Assert.AreEqual(1, matches.Length);
            Assert.AreEqual("**bold**", matches[0]);
        }

        [TestMethod]
        public void TestNoMatch()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("no match");

            Assert.AreEqual(0, matches.Length);
        }

        [TestMethod]
        public void TestBoldWithNoClosingMatch()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("**bold");

            Assert.AreEqual(0, matches.Length);
        }

        [TestMethod]
        public void TestBoldWithNoOpeningMatch()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("bold**");

            Assert.AreEqual(0, matches.Length);
        }

        [TestMethod]
        public void TestBoldParsing()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var html = bold.Parse("**bold**");

            Assert.AreEqual("<strong>bold</strong>", html);
        }

        [TestMethod]
        public void TestEnsureNoCrossOverWithItalicsMatches()
        {
            var bold = new Markdown.Parsers.Html.Parsers.Inline.Bold();
            var matches = bold.Matches("**bold** and __bold__ but not _____ and not ***** and not *italic* or _italic_");

            Assert.AreEqual(2, matches.Length);
            Assert.AreEqual("**bold**", matches[0]);
            Assert.AreEqual("__bold__", matches[1]);
        }
    }
}