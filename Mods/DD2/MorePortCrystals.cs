using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class MorePortCrystals : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "Higher Portcrystal Limit";
        const string description = "Changes the limit from 10 to 256.";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = version,
            NameAsBundle = bundleName,
            Desc         = description,
            Files        = [PathHelper.ITEM_PARAMETERS_PATH]
        };

        var refModName = "Higher Portcrystal Limit: REF (MUST INSTALL BOTH)";

        var mods = new[] {
            baseMod
                .SetName(refModName)
                .SetFiles([])
                .SetAction(_ => Copy(bundleName, refModName.Replace(':', '-')))
                .SetMakeIntoPak(false),
            baseMod
                .SetName("Higher Portcrystal Limit: PAK (MUST INSTALL BOTH)")
                .SetFiles([PathHelper.ITEM_PARAMETERS_PATH])
                .SetAction(Mod)
                .SetMakeIntoPak(true),
        };

        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, bundleName, true);
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static void Copy(string bundleName, string refModName) {
        const string filename   = "MapIconSizeFixer.dll";
        var          sourceFile = $@"{PathHelper.MODS_PATH}\{bundleName}\{filename}";
        var          destDir    = $@"{PathHelper.MODS_PATH}\{bundleName}\{bundleName}\{refModName}\reframework\plugins";
        Directory.CreateDirectory(destDir);
        File.Copy(sourceFile, $@"{destDir}\{filename}", true);
    }

    public static void Mod(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemParameters itemParameters:
                    itemParameters.WarpPointPutEnableNum = 256;
                    break;
            }
        }
    }
}