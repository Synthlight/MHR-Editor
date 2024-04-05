using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class MoreBoobJiggle : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name        = "More Boob Jiggle";
        const string description = "Because boobs need to jiggle.";
        const string version     = "1.0";

        var mod = new NexusMod {
            Version = version,
            Name    = name,
            Desc    = description,
            Files   = [PathHelper.SWAP_DATA_TOPS_PATH],
            Action  = Mod,
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true);
    }

    public static void Mod(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_TopsSwapItem param:
                    param.BreastSwaying *= 1000000;
                    break;
            }
        }
    }
}