using RE_Editor.Generator;

namespace RE_Editor.Tests;

[TestClass]
public class TestGeneration {
    [TestMethod]
    public void TestWhitelistGeneration() {
        Program.Main(new[] {"useWhitelist", "dryRun"});
    }

    [TestMethod]
    public void TestGreylistGeneration() {
        Program.Main(new[] {"useGreylist", "dryRun"});
    }

    [TestMethod]
    public void TestFullGeneration() {
        Program.Main(new[] {"dryRun"});
    }
}