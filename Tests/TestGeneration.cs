using System.Diagnostics;
using RE_Editor.Common.Models;
using RE_Editor.Common;
using RE_Editor.Generator;

namespace RE_Editor.Tests;

[TestClass]
public class TestGeneration {
    [TestMethod]
    public void TestWhitelistGeneration() {
        new GenerateFiles().Go(["useWhitelist", "dryRun"]);
    }

    [TestMethod]
    public void TestGreylistGeneration() {
        new GenerateFiles().Go(["useGreylist", "dryRun"]);
    }

    [TestMethod]
    public void TestMsgCrcOverrideCounts() {
        var allMsgPaths = (from basePath in PathHelper.TEST_PATHS
                           let path = PathHelper.CHUNK_PATH + basePath.Replace("STM", "MSG")
                           where Directory.Exists(path)
                           select path).ToList();
        var allMsgUserFiles = PathHelper.GetCachedFileList(FileListCacheType.USER, msg: true);
        Debug.Assert(allMsgPaths.Count == 0 ? allMsgUserFiles.Count == 0 : allMsgUserFiles.Count > 0);
    }
}