using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
public class NoStamina : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "No Stamina Consumed For Job Skills";
        const string description = "Changes job skills' stamina use.";
        const string version     = "1.1";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var dataFiles = new List<string>();
        for (var i = 1; i <= 10; i++) {
            dataFiles.Add($@"natives\stm\AppSystem\ch\common\human\userdata\parameter\Job{i:00}StaminaParameter.user.2");
        }

        var baseMod = new NexusModVariant {
            Version      = version,
            NameAsBundle = bundleName,
            Desc         = description,
            Files        = dataFiles,
            Action       = list => Stamina(list, StaminaOptions._0)
        };

        var mods = new[] {
            baseMod
                .SetName("Stamina Use: None")
                .SetAction(list => Stamina(list, StaminaOptions._0)),
            baseMod
                .SetName("Stamina Use: Half")
                .SetAction(list => Stamina(list, StaminaOptions.HALF)),
            baseMod
                .SetName("Stamina Use: x2")
                .SetAction(list => Stamina(list, StaminaOptions.X2)),
            baseMod
                .SetName("Stamina Use: x5")
                .SetAction(list => Stamina(list, StaminaOptions.X5)),
        };

        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, bundleName, true, makeIntoPak: true);
    }

    public static void Stamina(List<RszObject> rszObjectData, StaminaOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_HumanStaminaParameterAdditional itemData:
                    switch (option) {
                        case StaminaOptions._0:
                            itemData.Value = 0f;
                            break;
                        case StaminaOptions.HALF:
                            itemData.Value *= 0.5f;
                            break;
                        case StaminaOptions.X2:
                            itemData.Value *= 2f;
                            break;
                        case StaminaOptions.X5:
                            itemData.Value *= 5f;
                            break;
                    }
                    break;
            }
        }
    }

    public enum StaminaOptions {
        _0,
        HALF,
        X2,
        X5,
    }
}