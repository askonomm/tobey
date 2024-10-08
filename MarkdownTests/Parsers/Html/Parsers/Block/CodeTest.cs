﻿namespace MarkdownTests.Parsers.Html.Parsers.Block;

[TestClass]
public class CodeTest
{
    [TestMethod]
    public void TestWithLanguageParsing()
    {
        var code = new Markdown.Parsers.Html.Parsers.Block.Code();
        var result = code.Parse("```csharp\nvar x = 1;\n```");

        Assert.AreEqual("<pre class=\"language-csharp\"><code>var x = 1;</code></pre>", result);
    }

    [TestMethod]
    public void TestWithoutLanguageParsing()
    {
        var code = new Markdown.Parsers.Html.Parsers.Block.Code();
        var result = code.Parse("```\nvar x = 1;\n```");

        Assert.AreEqual("<pre><code>var x = 1;</code></pre>", result);
    }
}
