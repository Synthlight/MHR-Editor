using MHR_Editor.Common.Models;
using MHR_Editor.Data;

namespace Tests;

[TestClass]
public class TestFiles {
    private const string IN_PATH        = @"V:\MHR\re_chunk_000";
    private const string TEST_BASE_PATH = $@"{IN_PATH}\natives\STM\data\Define";
    private static readonly string[] TEST_PATHS = {
        $@"{TEST_BASE_PATH}\Player",
        $@"{TEST_BASE_PATH}\Otomo",
    };

    private static IEnumerable<object[]> GetFilesToTest() {
        foreach (var basePath in TEST_PATHS) {
            var files = Directory.EnumerateFiles(basePath, "*.user.2", SearchOption.AllDirectories);
            foreach (var file in files) {
                if (!File.Exists(file)) continue;
                yield return new object[] {file};
            }
        }
    }

    [TestInitialize]
    public void Init() {
        DataInit.Init();
    }

    [DynamicData(nameof(GetFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void Test(string file) {
        try {
            ReDataFile.Read(file);
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }
}