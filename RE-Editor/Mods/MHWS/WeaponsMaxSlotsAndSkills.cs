using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class WeaponsMaxSlotsAndSkills : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name = "Weapons - Max Slots & Skills";

        var baseMod = new NexusMod {
            Version      = "1.0.0",
            NameAsBundle = name,
            Desc         = "Weapons - Max Slots & Skills."
        };

        var mods = new List<INexusMod> {
            baseMod
                .SetName("Weapons - Max Slots Only")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base))
                .SetAction(MaxSlots),
            baseMod
                .SetName("Weapons - Max Skills Only")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base))
                .SetAction(MaxSkills),
            baseMod
                .SetName("Weapons - Max Slots & Skills")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base))
                .SetAction(data => {
                    MaxSlots(data);
                    MaxSkills(data);
                })
        };

        ModMaker.WriteMods(mods, name, copyLooseToFluffy: true);
    }

    public static void MaxSlots(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_user_data_WeaponData_cData armor:
                    foreach (var slotLevel in armor.SlotLevel) {
                        slotLevel.Value = 3;
                    }
                    break;
            }
        }
    }

    public static void MaxSkills(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_user_data_WeaponData_cData armor:
                    foreach (var skillLevel in armor.SkillLevel) {
                        if (skillLevel.Value > 0) {
                            skillLevel.Value = 10;
                        }
                    }
                    break;
            }
        }
    }
}