using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class ArmorMaxSlotsAndSkills : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name = "Armor - Max Slots & Skills";

        var baseMod = new NexusMod {
            Version      = "1.0.0",
            NameAsBundle = name,
            Desc         = "Armor - Max Slots & Skills."
        };

        var mods = new List<INexusMod> {
            baseMod
                .SetName("Armor - Max Slots Only")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
                .SetAction(MaxSlots),
            baseMod
                .SetName("Armor - Max Skills Only")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
                .SetAction(MaxSkills),
            baseMod
                .SetName("Armor - Max Slots & Skills")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
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
                case App_user_data_ArmorData_cData armor:
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
                case App_user_data_ArmorData_cData armor:
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