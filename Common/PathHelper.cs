using System.IO;
using Newtonsoft.Json;
using RE_Editor.Common.Models;

namespace RE_Editor.Common;

public static partial class PathHelper {
    public static List<string> GetCachedFileList(FileListCacheType cacheType) {
        var fileType = cacheType switch {
            FileListCacheType.MSG => $"msg.{Global.MSG_VERSION}",
            FileListCacheType.USER => "user.2",
            _ => throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null)
        };
        var          userFileCache = $@"{CHUNK_PATH}\{cacheType}_FILE_LIST_CACHE.json";
        List<string> allUserFiles;
        if (File.Exists(userFileCache)) {
            var userFileCacheJson = File.ReadAllText(userFileCache);
            allUserFiles = JsonConvert.DeserializeObject<List<string>>(userFileCacheJson)!;
        } else {
            allUserFiles = (from basePath in TEST_PATHS
                            from file in Directory.EnumerateFiles(CHUNK_PATH + basePath, $"*.{fileType}", SearchOption.AllDirectories)
                            where File.Exists(file)
                            select file).ToList();
            var userFileCacheJson = JsonConvert.SerializeObject(allUserFiles);
            File.WriteAllText(userFileCache, userFileCacheJson);
        }
        return allUserFiles;
    }
}