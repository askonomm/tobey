namespace HtmtTests;

[TestClass]
public class ParserTest
{
    [TestMethod]
    public void TestPrintNodeReplacement()
    {
        const string template = "<html><body><h2>Hello, <htmt:print key=\"name\" /></h2></body></html>";
        var data = new Dictionary<string, object> { { "name", "John Doe" } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<html><body><h2>Hello, John Doe</h2></body></html>", result);
    }

    [TestMethod]
    public void TestPrintNestedDataNodeReplacement()
    {
        const string template = "<html><body><h2>Hello, <htmt:print key=\"person.name\" /></h2></body></html>";
        var data = new Dictionary<string, object> { { "person", new { name = "John Doe" } } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<html><body><h2>Hello, John Doe</h2></body></html>", result);
    }

    [TestMethod]
    public void TestIfNodes()
    {
        const string template = "<html><body><htmt:if key=\"show\"><h2>Hello, <htmt:print key=\"name\" /></h2></htmt:if></body></html>";
        var data = new Dictionary<string, object> { { "name", "John Doe" }, { "show", true } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<html><body><h2>Hello, John Doe</h2></body></html>", result);
    }

    [TestMethod]
    public void TestIfNotNodes()
    {
        const string template = "<html><body><htmt:if key=\"show\"><h2>Hello, <htmt:print key=\"name\" /></h2></htmt:if></body></html>";
        var data = new Dictionary<string, object> { { "name", "John Doe" }, { "show", false } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<html><body></body></html>", result);
    }

    [TestMethod]
    public void TestUnlessNodes()
    {
        const string template = "<html><body><htmt:unless key=\"show\"><h2>Hello, <htmt:print key=\"name\" /></h2></htmt:unless></body></html>";
        var data = new Dictionary<string, object> { { "name", "John Doe" }, { "show", false } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<html><body><h2>Hello, John Doe</h2></body></html>", result);
    }

    [TestMethod]
    public void TestUnlessNotNodes()
    {
        const string template = "<html><body><htmt:unless key=\"show\"><h2>Hello, <htmt:print key=\"name\" /></h2></htmt:unless></body></html>";
        var data = new Dictionary<string, object> { { "name", "John Doe" }, { "show", true } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<html><body></body></html>", result);
    }

    [TestMethod]
    public void TestForNodes()
    {
        const string template = "<html><body><htmt:for key=\"people\" as=\"person\"><h2>Hello, <htmt:print key=\"person.name\" /></h2></htmt:for></body></html>";
        var data = new Dictionary<string, object> { { "people", new[] { new { name = "John Doe" }, new { name = "Matthew Doe" } } } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<html><body><h2>Hello, John Doe</h2><h2>Hello, Matthew Doe</h2></body></html>", result);
    }

    [TestMethod]
    public void TestHtml5Document()
    {
        const string template = "<!DOCTYPE html><html><head><title><htmt:print key=\"title\" /></title></head><body><h1><htmt:print key=\"heading\" /></h1></body></html>";
        var data = new Dictionary<string, object> { { "title", "Hello, World!" }, { "heading", "Welcome to the world!" } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<!DOCTYPE html><html><head><title>Hello, World!</title></head><body><h1>Welcome to the world!</h1></body></html>", result);
    }
    
    [TestMethod]
    public void TestHtml5DocumentWithComments()
    {
        const string template = "<!DOCTYPE html><!-- This is a comment --><html><head><title><htmt:print key=\"title\" /></title></head><body><h1><htmt:print key=\"heading\" /></h1></body></html>";
        var data = new Dictionary<string, object> { { "title", "Hello, World!" }, { "heading", "Welcome to the world!" } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<!DOCTYPE html><html><head><title>Hello, World!</title></head><body><h1>Welcome to the world!</h1></body></html>", result);
    }
    
    [TestMethod]
    public void TestHtml4Document()
    {
        const string template = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\"><html><head><title><htmt:print key=\"title\" /></title></head><body><h1><htmt:print key=\"heading\" /></h1></body></html>";
        var data = new Dictionary<string, object> { { "title", "Hello, World!" }, { "heading", "Welcome to the world!" } };
        var parser = new Htmt.Parser(template);
        var result = parser.Parse(data);
        
        Assert.AreEqual("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\"><html><head><title>Hello, World!</title></head><body><h1>Welcome to the world!</h1></body></html>", result);
    }
}