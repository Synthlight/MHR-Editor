using RE_Editor.Generator;

namespace RE_Editor.Tests;

[TestClass]
public class TestGeneration {
    [TestMethod]
    public void TestWithWhitelist() {
        Program.Main(new[] {"useWhitelist", "dryRun"});
    }

    [TestMethod]
    public void TestWithoutWhitelist() {
        Program.Main(new[] {"dryRun"});
    }
}