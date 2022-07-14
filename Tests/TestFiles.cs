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
            ReDataFile data;
            try {
                data = ReDataFile.Read(file);
            } catch (Exception) {
                Assert.Inconclusive();
                return;
            }

            using var writer = new BinaryWriter(new MemoryStream());
            data.Write(writer, true);

            var sourceLength = new FileInfo(file).Length;
            var destLength   = writer.BaseStream.Length;
            Debug.Assert(sourceLength == destLength, $"Length expected {sourceLength}, found {destLength}.");

            // To byte arrays since MD5 unbelievably takes steam **position** into account.
            var fileHash = MD5.Create().ComputeHash(File.ReadAllBytes(file));
            var newHash  = MD5.Create().ComputeHash(((MemoryStream) writer.BaseStream).ToArray());
            Debug.Assert(fileHash.SequenceEqual(newHash), $"MD5 expected {BitConverter.ToString(fileHash)}, found {BitConverter.ToString(newHash)}.");
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }
}