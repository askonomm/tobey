namespace FrontMatterTests;

using FrontMatter;

[TestClass]
public sealed class FrontMatterTest
{
    [TestMethod]
    public void TestFull()
    {
        var text = "---\nkey: \"value\"\n---\n\n# Title\r\n\r\nContent";
        var result = FrontMatter.Parse(text);
        Assert.AreEqual(2, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(string));
        Assert.AreEqual("value", result["key"]);
        Assert.IsInstanceOfType(result["content"], typeof(string));
        Assert.AreEqual("<h1>Title</h1>\n\n<p>Content</p>", result["content"]);
    }

    [TestMethod]
    public void TestEmpty()
    {
        var text = "---\n---\n# Title\r\n\r\nContent";
        var result = FrontMatter.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["content"], typeof(string));
        Assert.AreEqual("<h1>Title</h1>\n\n<p>Content</p>", result["content"]);
    }

    [TestMethod]
    public void TestNoFrontMatter()
    {
        var text = "# Title\r\n\r\nContent";
        var result = FrontMatter.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["content"], typeof(string));
        Assert.AreEqual("<h1>Title</h1>\n\n<p>Content</p>", result["content"]);
    }
}
