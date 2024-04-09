using RE_Editor.Common;
using RE_Editor.Common.Models;

namespace RE_Editor.Tests;

[TestClass]
public class TestMsgFiles {
    private static IEnumerable<object[]> GetFilesToTest() {
        return PathHelper.GetCachedFileList(FileListCacheType.MSG).Select(s => new object[] {s});
    }

    [DynamicData(nameof(GetFilesToTest), DynamicDataSourceType.Method)]
    [DataTestMethod]
    public void TestReadTextFile(string file) {
        try {
            MSG.Read(file);
        } catch (FileNotSupported) {
            Assert.Inconclusive();
        }
    }
}