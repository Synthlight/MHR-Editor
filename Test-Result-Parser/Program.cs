﻿using RE_Editor.Common;
using TrxFileParser;

namespace RE_Editor.Test_Result_Parser;

public static class Program {
    public const  string BASE_PROJ_PATH    = @"..\..\..";
    private const string TEST_RESULTS_PATH = $@"{BASE_PROJ_PATH}\TestResults\{PathHelper.CONFIG_NAME}\Results.trx";
    private const string ASSETS_DIR        = $@"{BASE_PROJ_PATH}\RE-Editor\Data\{PathHelper.CONFIG_NAME}\Assets";

    public static void Main() {
        var          results  = TrxDeserializer.Deserialize(TEST_RESULTS_PATH);
        const string testName = "TestWriteUserFile";

        var paths = from result in results.Results.UnitTestResults
                    where result.Outcome == "Passed"
                          && result.TestName.StartsWith(testName)
                    let name = result.TestName[$"{testName} (".Length..^1]
                                     .Replace($@"{PathHelper.CHUNK_PATH}\", "")
                                     .Replace('/', '\\')
                                     .ToLower()
                    orderby name
                    select name;

        using var assetFile  = File.CreateText($@"{ASSETS_DIR}\{PathHelper.SUPPORTED_FILES_NAME}");
        using var outputFile = File.CreateText(TEST_RESULTS_PATH.Replace("Results.trx", "Supported Files.txt"));

        foreach (var path in paths) {
            assetFile.WriteLine(path);
            outputFile.WriteLine(path);
        }
    }
}