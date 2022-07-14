using System.Diagnostics;
using System.Security.Cryptography;
using MHR_Editor.Common.Models;

namespace Tests;

[TestClass]
public class TestFiles {
    private const string IN_PATH        = @"V:\MHR\re_chunk_000";
    private const string TEST_BASE_PATH = $@"{IN_PATH}\natives\STM\data\Define";
    private static readonly string[] TEST_PATHS = {
        $@"{IN_PATH}\natives\STM\data\Define",
        $@"{IN_PATH}\natives\STM\data\System",
        $@"{IN_PATH}\natives\STM\player\UserData",
    };

    private static IEnumerable<object[]> GetFilesToTest() {
        return from basePath in TEST_PATHS
               from file in Directory.EnumerateFiles(basePath, "*.user.2", SearchOption.AllDirectories)
               where File.Exists(file)
               select new object[] {file};
    }

    [DynamicData(nameof(GetFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestRead(string file) {
        try {
            ReDataFile.Read(file);
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }

    [DynamicData(nameof(GetFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestWrite(string file) {
        try {
            var       data   = ReDataFile.Read(file);
            using var writer = new BinaryWriter(new MemoryStream());
            data.Write(writer, true);

            var sourceLength = new FileInfo(file).Length;
            var destLength   = writer.BaseStream.Length;
            Debug.Assert(sourceLength == destLength, $"Length expected {sourceLength}, found {destLength}.");


            var fileHash = MD5.Create().ComputeHash(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read));
            var newHash  = MD5.Create().ComputeHash(writer.BaseStream);
            Debug.Assert(sourceLength == destLength, $"MD5 expected {fileHash}, found {newHash}.");
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }
}