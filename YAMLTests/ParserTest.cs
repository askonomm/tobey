namespace YAMLTests;

[TestClass]
public class ParserTest
{
    [TestMethod]
    public void TestMethod1()
    {
        var text = "key: \"value\"";
        var result = YAML.Parser.Parse(text);

        Assert.AreEqual(1, result.Count);
    }
}
