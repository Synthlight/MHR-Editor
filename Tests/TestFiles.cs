using System.Diagnostics;
using System.Security.Cryptography;
using RE_Editor.Common;
using RE_Editor.Common.Models;

namespace RE_Editor.Tests;

[TestClass]
public class TestFiles {
    private static IEnumerable<object[]> GetUserFilesToTest() {
        return from basePath in PathHelper.TEST_PATHS
               from file in Directory.EnumerateFiles(PathHelper.CHUNK_PATH + basePath, "*.user.2", SearchOption.AllDirectories)
               where File.Exists(file)
               select new object[] {file};
    }

    private static IEnumerable<object[]> GetTextFilesToTest() {
        return from basePath in PathHelper.TEST_PATHS
               from file in Directory.EnumerateFiles(PathHelper.CHUNK_PATH + basePath, "*.msg.17", SearchOption.AllDirectories)
               where File.Exists(file)
               select new object[] {file};
    }

    [DynamicData(nameof(GetUserFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestReadUserFile(string file) {
        try {
            ReDataFile.Read(file);
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }

    [DynamicData(nameof(GetUserFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestWriteUserFile(string file) {
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

    [DynamicData(nameof(GetTextFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestReadTextFile(string file) {
        try {
            MSG.Read(file);
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }
}