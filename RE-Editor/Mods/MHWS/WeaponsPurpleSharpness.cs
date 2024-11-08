using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class WeaponsPurpleSharpness : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name = "Weapons - Max Purple Sharpness";

        var baseMod = new NexusMod {
            Version      = "1.0.0",
            NameAsBundle = name,
            Desc         = "Weapons - Max Purple Sharpness."
        };

        var mods = new List<INexusMod> {
            baseMod
                .SetName("Weapons - Max Purple Sharpness")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base))
                .SetAction(MaxSharpness)
        };

        ModMaker.WriteMods(mods, name, copyLooseToFluffy: true);
    }

    public static void MaxSharpness(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_user_data_WeaponData_cData weapon:
                    if (weapon.SharpnessValList[0].Value > 0) {
                        weapon.SharpnessValList[0].Value = 10;
                        weapon.SharpnessValList[1].Value = 10;
                        weapon.SharpnessValList[2].Value = 10;
                        weapon.SharpnessValList[3].Value = 10;
                        weapon.SharpnessValList[4].Value = 10;
                        weapon.SharpnessValList[5].Value = 10;
                        weapon.SharpnessValList[6].Value = 340;
                        foreach (var handicraft in weapon.TakumiValList) {
                            handicraft.Value = 0;
                        }
                    }
                    break;
            }
        }
    }
}