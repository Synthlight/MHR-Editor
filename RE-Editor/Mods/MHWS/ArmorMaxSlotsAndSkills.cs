﻿using System.Collections.Generic;
using System.IO;
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
        const string name        = "Armor - Max Slots & Skills";
        const string description = "Armor - Max Slots & Skills.";
        const string version     = "1.0.0";

        var baseMod = new NexusMod {
            Version      = version,
            NameAsBundle = name,
            Desc         = description
        };

        var baseLuaMod = new VariousDataTweak {
            Version      = version,
            NameAsBundle = name,
            Desc         = description
        };

        var mods = new List<INexusMod> {
            baseMod
                .SetName("Armor - Max Slots Only (Natives)")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
                .SetAction(MaxSlots),
            baseLuaMod
                .SetName("Armor - Max Slots Only (REF)")
                .SetDefaultLuaName()
                .SetChanges([
                    new() {
                        Target = VariousDataTweak.Target.ARMOR_DATA,
                        Action = MaxSlotsRef
                    }
                ]),
            baseMod
                .SetName("Armor - Max Skills Only (Natives)")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
                .SetAction(MaxSkills),
            baseLuaMod
                .SetName("Armor - Max Skills Only (REF)")
                .SetDefaultLuaName()
                .SetChanges([
                    new() {
                        Target = VariousDataTweak.Target.ARMOR_DATA,
                        Action = MaxSkillsRef
                    }
                ]),
            baseMod
                .SetName("Armor - Max Slots & Skills (Natives)")
                .SetFiles([PathHelper.ARMOR_DATA_PATH])
                .SetAction(data => {
                    MaxSlots(data);
                    MaxSkills(data);
                })
        };

        ModMaker.WriteMods(mods, name, copyLooseToFluffy: true, noPakZip: true);
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

    public static void MaxSlotsRef(StreamWriter writer) {
        writer.WriteLine("    for _, slotLevel in pairs(entry._SlotLevel) do");
        writer.WriteLine("        slotLevel._Value = 3");
        writer.WriteLine("    end");
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

    public static void MaxSkillsRef(StreamWriter writer) {
        writer.WriteLine("    for skillIndex = 0, entry._SkillLevel:get_size() - 1 do");
        writer.WriteLine("        if (entry._SkillLevel[skillIndex].m_value >= 1) then");
        writer.WriteLine("            entry._SkillLevel[skillIndex] = 10");
        writer.WriteLine("        end");
        writer.WriteLine("    end");
    }
}