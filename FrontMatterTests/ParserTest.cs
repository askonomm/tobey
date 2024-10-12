namespace FrontMatterTests;

[TestClass]
public sealed class ParserTest
{
    [TestMethod]
    public void TestFull()
    {
        const string text = "---\r\nkey: \"value\"\r\n---\r\n\r\n# Title\r\n\r\nContent";
        var result = FrontMatter.Parser.Parse(text);
        Assert.AreEqual(2, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(string));
        Assert.AreEqual("value", result["key"]);
        Assert.IsInstanceOfType(result["content"], typeof(string));
        Assert.AreEqual("<h1>Title</h1>\n\n<p>Content</p>", result["content"]);
    }

    [TestMethod]
    public void TestEmpty()
    {
        const string text = "---\r\n---\r\n\r\n# Title\r\n\r\nContent";
        var result = FrontMatter.Parser.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["content"], typeof(string));
        Assert.AreEqual("<h1>Title</h1>\n\n<p>Content</p>", result["content"]);
    }

    [TestMethod]
    public void TestDeepKeys()
    {
        const string text = "---\r\nkey.subkey: \"value\"\r\n---\r\n\r\n# Title\r\n\r\nContent";
        var result = FrontMatter.Parser.Parse(text);
        
        Assert.AreEqual(2, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(Dictionary<string, object>));
        Assert.AreEqual(1, ((Dictionary<string, object>) result["key"]).Count);
        Assert.IsInstanceOfType(((Dictionary<string, object>) result["key"])["subkey"], typeof(string));
        Assert.AreEqual("value", ((Dictionary<string, object>) result["key"])["subkey"]);
        Assert.IsInstanceOfType(result["content"], typeof(string));
        Assert.AreEqual("<h1>Title</h1>\n\n<p>Content</p>", result["content"]);
    }

    /** should throw an error */
    [TestMethod]
    public void TestNoFrontMatter()
    {
        const string text = "# Title\r\n\r\nContent";

        Assert.ThrowsException<Exception>(() => FrontMatter.Parser.Parse(text));
    }
}
