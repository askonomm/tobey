using Tobey.DataComposer;

namespace TobeyTests
{
    [TestClass]
    public sealed class DataComposerTest
    {
        [TestMethod]
        public void TestComposerStringFiltering()
        {
            var content = new List<Dictionary<string, object?>>
            {
                new() {
                    { "directory", "posts" }
                },
                new() {
                    { "directory", "pages" }
                }
            };

            var dsl = new Dictionary<string, object?>
            {
                { "directory", "posts" }
            };

            var result = new Composer(content).Compose(dsl);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("posts", result[0]["directory"]);
        }

        [TestMethod]
        public void TestComposerIntFiltering()
        {
            var content = new List<Dictionary<string, object?>>
            {
                new() {
                    { "n", 1 }
                },
                new() {
                    { "n", 2 }
                },
                new() {
                    { "n", 3 }
                }
            };

            var dsl = new Dictionary<string, object?>
            {
                { "n", 1 }
            };

            var result = new Composer(content).Compose(dsl);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0]["n"]);
        }

        [TestMethod]
        public void TestComposerDoubleFiltering()
        {
            var content = new List<Dictionary<string, object?>>
            {
                new() {
                    { "n", 1.0 }
                },
                new() {
                    { "n", 2.0 }
                },
                new() {
                    { "n", 3.0 }
                }
            };

            var dsl = new Dictionary<string, object?>
            {
                { "n", 1.0 }
            };

            var result = new Composer(content).Compose(dsl);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1.0, result[0]["n"]);
        }

        [TestMethod]
        public void TestComposerBoolFiltering()
        {
            var content = new List<Dictionary<string, object?>>
            {
                new() {
                    { "b", true }
                },
                new() {
                    { "b", false }
                }
            };

            var dsl = new Dictionary<string, object?>
            {
                { "b", true }
            };

            var result = new Composer(content).Compose(dsl);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result[0]["b"]);
        }

            [TestMethod]
        public void TestComposerLimiting()
        {
            var content = new List<Dictionary<string, object?>>
            {
                new() {
                    { "directory", "posts" }
                },
                new() {
                    { "directory", "pages" }
                }
            };

            var dsl = new Dictionary<string, object?>
            {
                { "limit", 1 }
            };

            var result = new Composer(content).Compose(dsl);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestComposerOffsetting()
        {
            var content = new List<Dictionary<string, object?>>
            {
                new() {
                    { "directory", "posts" }
                },
                new() {
                    { "directory", "pages" }
                }
            };

            var dsl = new Dictionary<string, object?>
            {
                { "offset", 1 }
            };

            var result = new Composer(content).Compose(dsl);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("pages", result[0]["directory"]);
        }

        [TestMethod]
        public void TestComposerSorting()
        {
            var content = new List<Dictionary<string, object?>>
            {
                new() {
                    { "directory", "posts" }
                },
                new() {
                    { "directory", "pages" }
                },
                new() {
                    { "directory", "photos" }
                },
                new() {
                    { "directory", "thoughts" }
                }
            };

            var dsl = new Dictionary<string, object?>
            {
                { "sort_by", "directory" }
            };

            var result = new Composer(content).Compose(dsl);

            Assert.AreEqual("pages", result[0]["directory"]);
        }
    }
}
