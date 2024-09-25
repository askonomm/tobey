namespace YAMLTests;

[TestClass]
public class ParserTest
{
    [TestMethod]
    public void TestPlainValue()
    {
        var text = "key: \"value\"";
        var result = YAML.Parser.Parse(text);

        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(string));
        Assert.AreEqual("value", result["key"]);
    }

    [TestMethod]
    public void TestStringValue()
    {
        var text = "key: \"value\"";
        var result = YAML.Parser.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(string));
        Assert.AreEqual("value", result["key"]);
    }

    [TestMethod]
    public void TestMultilineValue()
    {
        var text = "key: |\n  value";
        var result = YAML.Parser.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(string));
        Assert.AreEqual("value", result["key"]);
    }

    [TestMethod]
    public void TestIntegerValue()
    {
        var text = "key: 42";
        var result = YAML.Parser.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(int));
        Assert.AreEqual(42, result["key"]);
    }

    [TestMethod]
    public void TestDoubleValue()
    {
        var text = "key: 42.42";
        var result = YAML.Parser.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(double));
        Assert.AreEqual(42.42, result["key"]);
    }

    [TestMethod]
    public void TestBooleanValue()
    {
        var text = "key: true";
        var result = YAML.Parser.Parse(text);
        Assert.AreEqual(1, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(bool));
        Assert.AreEqual(true, result["key"]);
    }

    [TestMethod]
    public void TestMultipleValues()
    {
        var text = "key: \"value\"\nkey2: 42";
        var result = YAML.Parser.Parse(text);
        Assert.AreEqual(2, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(string));
        Assert.AreEqual("value", result["key"]);
        Assert.IsInstanceOfType(result["key2"], typeof(int));
        Assert.AreEqual(42, result["key2"]);
    }

    [TestMethod]
    public void TestNestedValues()
    {
        var text = "key: \"value\"\nkey2:\n  key3: 42";
        var result = YAML.Parser.Parse(text);
        Assert.AreEqual(2, result.Count);
        Assert.IsInstanceOfType(result["key"], typeof(string));
        Assert.AreEqual("value", result["key"]);
        Assert.IsInstanceOfType(result["key2"], typeof(Dictionary<string, object>));
        Assert.AreEqual(1, ((Dictionary<string, object>)result["key2"]).Count);
        Assert.IsInstanceOfType(((Dictionary<string, object>)result["key2"])["key3"], typeof(int));
        Assert.AreEqual(42, ((Dictionary<string, object>)result["key2"])["key3"]);
    }
}
