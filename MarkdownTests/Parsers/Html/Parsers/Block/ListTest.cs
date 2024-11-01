namespace MarkdownTests.Parsers.Html.Parsers.Block;

[TestClass]
public class ListTest
{
    [TestMethod]
    public void TestSingleLevelList()
    {
        const string text = "- Item 1\n- Item 2\n- Item 3";
        var result = new Markdown.Parsers.Html.Parsers.Block.List().Parse(text);
        
        Assert.AreEqual("<ul>\n\t<li>Item 1</li>\n\t<li>Item 2</li>\n\t<li>Item 3</li>\n</ul>", result);
    }
    
    [TestMethod]
    public void TestSingleLevelListWithInline()
    {
        const string text = "- *Item* 1\n- [Item 2](https://example.com)\n- Item 3";
        var result = new Markdown.Parsers.Html.Parsers.Block.List().Parse(text); 
        
        Assert.AreEqual("<ul>\n\t<li><em>Item</em> 1</li>\n\t<li><a href=\"https://example.com\">Item 2</a></li>\n\t<li>Item 3</li>\n</ul>", result);
    }
    
    [TestMethod]
    public void TestMultiLevelList()
    {
        const string text = "- Item 1\n  - Item 1.1\n  - Item 1.2\n- Item 2\n- Item 3";
        var result = new Markdown.Parsers.Html.Parsers.Block.List().Parse(text); 
        
        Assert.AreEqual("<ul>\n\t<li>Item 1\n\t\t<ul>\n\t\t\t<li>Item 1.1</li>\n\t\t\t<li>Item 1.2</li>\n\t\t</ul>\n\t</li>\n\t<li>Item 2</li>\n\t<li>Item 3</li>\n</ul>", result);
    }
    
    [TestMethod]
    public void TestOrderedSingleLevelList()
    {
        const string text = "1. Item 1\n1. Item 2\n1. Item 3";
        var result = new Markdown.Parsers.Html.Parsers.Block.List().Parse(text); 
        
        Assert.AreEqual("<ol>\n\t<li>Item 1</li>\n\t<li>Item 2</li>\n\t<li>Item 3</li>\n</ol>", result);
    }
    
    [TestMethod]
    public void TestOrderedSingleLevelListWithInline()
    {
        const string text = "1. *Item* 1\n1. [Item 2](https://example.com)\n1. Item 3";
        var result = new Markdown.Parsers.Html.Parsers.Block.List().Parse(text); 
        
        Assert.AreEqual("<ol>\n\t<li><em>Item</em> 1</li>\n\t<li><a href=\"https://example.com\">Item 2</a></li>\n\t<li>Item 3</li>\n</ol>", result);
    }
    
    [TestMethod]
    public void TestOrderedMultiLevelList()
    {
        const string text = "1. Item 1\n  1. Item 1.1\n  1. Item 1.2\n1. Item 2\n1. Item 3";
        var result = new Markdown.Parsers.Html.Parsers.Block.List().Parse(text); 
        
        Assert.AreEqual("<ol>\n\t<li>Item 1\n\t\t<ol>\n\t\t\t<li>Item 1.1</li>\n\t\t\t<li>Item 1.2</li>\n\t\t</ol>\n\t</li>\n\t<li>Item 2</li>\n\t<li>Item 3</li>\n</ol>", result);
    }
    
    [TestMethod]
    public void TestMixedList()
    {
        const string text = "1. Item 1\n- Item 2\n  - Item 2.1\n  - Item 2.2\n2. Item 3";
        var result = new Markdown.Parsers.Html.Parsers.Block.List().Parse(text); 
        
        // assert equals but remember that sublists go inside the <li> tag
        Assert.AreEqual("<ol>\n\t<li>Item 1</li>\n\t<li>Item 2\n\t\t<ul>\n\t\t\t<li>Item 2.1</li>\n\t\t\t<li>Item 2.2</li>\n\t\t</ul>\n\t</li>\n\t<li>Item 3</li>\n</ol>", result);
    }
}