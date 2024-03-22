using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

/**
 * Currently does nothing.
 * Well, it modifies the files, they just have nmo affect on the game.
 */

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class DifficultyMultipliers : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "Difficulty Multipliers";
        const string description = "Multipliers on the difficulty. Higher multipliers do more damage.";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = version,
            NameAsBundle = bundleName,
            Desc         = description,
        };

        var mods = new[] {
            baseMod
                .SetName("Difficulty Multipliers: Half")
                .SetFiles([PathHelper.DIFFICULTY_SETTINGS_PATH])
                .SetAction(list => Jobs(list, DifficultyOptions.HALF)),
            baseMod
                .SetName("Difficulty Multipliers: Double")
                .SetFiles([PathHelper.DIFFICULTY_SETTINGS_PATH])
                .SetAction(list => Jobs(list, DifficultyOptions.DOUBLE)),
            baseMod
                .SetName("Difficulty Multipliers: x5")
                .SetFiles([PathHelper.DIFFICULTY_SETTINGS_PATH])
                .SetAction(list => Jobs(list, DifficultyOptions.DOUBLE)),
        };

        ModMaker.WriteMods(mods.ToList(), PathHelper.CHUNK_PATH, outPath, bundleName, true, makeIntoPak: true);
    }

    public static void Jobs(List<RszObject> rszObjectData, DifficultyOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_GameSystemUserData_DifficultySetting difficultySetting:
                    switch (option) {
                        case DifficultyOptions.HALF:
                            difficultySetting.DamageRate *= 0.5f;
                            break;
                        case DifficultyOptions.DOUBLE:
                            difficultySetting.DamageRate *= 2f;
                            break;
                        case DifficultyOptions.X5:
                            difficultySetting.DamageRate *= 5f;
                            break;
                    }
                    break;
            }
        }
    }

    public enum DifficultyOptions {
        HALF,
        DOUBLE,
        X5,
    }
}