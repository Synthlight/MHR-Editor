using RE_Editor.Generator;

namespace RE_Editor.Tests;

[TestClass]
public class TestGeneration {
    [TestMethod]
    public void TestWhitelistGeneration() {
        new GenerateFiles().Go(new[] {"useWhitelist", "dryRun"});
    }

    [TestMethod]
    public void TestGreylistGeneration() {
        new GenerateFiles().Go(new[] {"useGreylist", "dryRun"});
    }

    [TestMethod]
    public void TestFullGeneration() {
        new GenerateFiles().Go(new[] {"dryRun"});
    }
}