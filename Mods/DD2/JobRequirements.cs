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

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class JobRequirements : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "No Job Requirements";
        const string description = "Tweaks weapon & armor data to let you use any armor/weapon with any job.";
        const string version     = "1.1";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = version,
            NameAsBundle = bundleName,
            Desc         = description,
        };

        var mods = new[] {
            baseMod
                .SetName("No Job Requirements: Armors Only")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
                .SetAction(list => Jobs(list, JobOptions.ARMOR_ONLY)),
            baseMod
                .SetName("No Job Requirements: Armors Only (No Weight)")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
                .SetAction(list => {
                    Jobs(list, JobOptions.ARMOR_ONLY);
                    ItemTweaks.Weight(list, ItemTweaks.WeightOptions._0);
                }),
            baseMod
                .SetName("No Job Requirements: Weapons Only")
                .SetFiles([PathHelper.WEAPON_DATA_PATH])
                .SetAction(list => Jobs(list, JobOptions.WEAPONS_ONLY)),
            baseMod
                .SetName("No Job Requirements: Weapons Only (No Weight)")
                .SetFiles([PathHelper.WEAPON_DATA_PATH])
                .SetAction(list => {
                    Jobs(list, JobOptions.WEAPONS_ONLY);
                    ItemTweaks.Weight(list, ItemTweaks.WeightOptions._0);
                }),
        };

        ModMaker.WriteMods(mods.ToList(), PathHelper.CHUNK_PATH, outPath, bundleName, true, makeIntoPak: true);
    }

    public static void Jobs(List<RszObject> rszObjectData, JobOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemWeaponParam itemData:
                    switch (option) {
                        case JobOptions.ALL:
                        case JobOptions.WEAPONS_ONLY:
                            itemData.Job = ushort.MaxValue;
                            break;
                    }
                    break;
                case App_ItemArmorParam itemData:
                    switch (option) {
                        case JobOptions.ALL:
                        case JobOptions.ARMOR_ONLY:
                            itemData.Job = ushort.MaxValue;
                            break;
                    }
                    break;
            }
        }
    }

    public enum JobOptions {
        ALL,
        ARMOR_ONLY,
        WEAPONS_ONLY,
    }
}