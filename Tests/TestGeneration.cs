using MHR_Editor.Generator;

namespace Tests;

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