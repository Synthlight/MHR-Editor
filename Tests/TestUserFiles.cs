using System.Diagnostics;
using System.Security.Cryptography;
using RE_Editor.Common;
using RE_Editor.Common.Models;

namespace RE_Editor.Tests;

[TestClass]
public class TestUserFiles {
    private static IEnumerable<object[]> GetFilesToTest() {
        return PathHelper.GetCachedFileList(FileListCacheType.USER).Select(s => new object[] {s});
    }

    [DynamicData(nameof(GetFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestReadUserFile(string file) {
        try {
            ReDataFile.Read(file);
        } catch (FileNotSupported) {
            if (Debugger.IsAttached) throw;
            Assert.Inconclusive();
        }
    }

    [DynamicData(nameof(GetFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestWriteUserFile(string file) {
        try {
            ReDataFile data;
            try {
                data = ReDataFile.Read(file);
            } catch (Exception e) {
                Assert.Inconclusive($"{e.Message}\n{e.StackTrace}");
                return;
            }

            try {
                using var writer = new BinaryWriter(new MemoryStream());
                data.Write(writer, testWritePosition: true, forGp: file.Contains("MSG"));

                var sourceLength = new FileInfo(file).Length;
                var destLength   = writer.BaseStream.Length;
                Debug.Assert(sourceLength == destLength, $"Length expected {sourceLength}, found {destLength}.");

                // To byte arrays since MD5 unbelievably takes steam **position** into account.
                var fileHash = MD5.Create().ComputeHash(File.ReadAllBytes(file));
                var newHash  = MD5.Create().ComputeHash(((MemoryStream) writer.BaseStream).ToArray());
                Debug.Assert(fileHash.SequenceEqual(newHash), $"MD5 expected {BitConverter.ToString(fileHash)}, found {BitConverter.ToString(newHash)}.");
            } catch (Exception) {
                if (Debugger.IsAttached) {
                    // Re-read before write because write can cause issues if it fails part way through.
                    ReDataFile.Read(file).Write(new BinaryWriter(File.OpenWrite($@"O:\Temp\{Path.GetFileName(file)}")), testWritePosition: true, forGp: file.Contains("MSG"));
                }
                throw;
            }
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }
}