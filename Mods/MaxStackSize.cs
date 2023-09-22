using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class MaxStackSize : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "Stack Size Changes";
        const string description = "Modifies the stack size of stackable items.";
        const string version     = "1.7.1";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = version,
            NameAsBundle = bundleName,
            Desc         = description,
            Files = new[] {
                PathHelper.ITEM_DATA_PATH,
                PathHelper.ITEM_DATA_PATH_AO,
                PathHelper.ITEM_DATA_PATH_AO_OVR,
                PathHelper.ITEM_DATA_PATH_MC,
                PathHelper.ITEM_DATA_PATH_MC_OVR,
            }
        };

        var mods = new[] {
            baseMod
                .SetName("Stack Size (All): 0999")
                .SetAction(list => MaxStacks(list, Target._999, Type.FULL)),
            baseMod
                .SetName("Stack Size (All): 9999")
                .SetAction(list => MaxStacks(list, Target._9999, Type.FULL)),
            baseMod
                .SetName("Stack Size (All): x02")
                .SetAction(list => MaxStacks(list, Target.X2, Type.FULL)),
            baseMod
                .SetName("Stack Size (All): x03")
                .SetAction(list => MaxStacks(list, Target.X3, Type.FULL)),
            baseMod
                .SetName("Stack Size (All): x04")
                .SetAction(list => MaxStacks(list, Target.X4, Type.FULL)),
            baseMod
                .SetName("Stack Size (All): x05")
                .SetAction(list => MaxStacks(list, Target.X5, Type.FULL)),
            baseMod
                .SetName("Stack Size (All): x10")
                .SetAction(list => MaxStacks(list, Target.X10, Type.FULL)),
            baseMod
                .SetName("Stack Size (Ammo Only): 0999")
                .SetAction(list => MaxStacks(list, Target._999, Type.AMMO_ONLY)),
            baseMod
                .SetName("Stack Size (Ammo Only): 9999")
                .SetAction(list => MaxStacks(list, Target._9999, Type.AMMO_ONLY)),
            baseMod
                .SetName("Stack Size (Ammo Only): x02")
                .SetAction(list => MaxStacks(list, Target.X2, Type.AMMO_ONLY)),
            baseMod
                .SetName("Stack Size (Ammo Only): x03")
                .SetAction(list => MaxStacks(list, Target.X3, Type.AMMO_ONLY)),
            baseMod
                .SetName("Stack Size (Ammo Only): x04")
                .SetAction(list => MaxStacks(list, Target.X4, Type.AMMO_ONLY)),
            baseMod
                .SetName("Stack Size (Ammo Only): x05")
                .SetAction(list => MaxStacks(list, Target.X5, Type.AMMO_ONLY)),
            baseMod
                .SetName("Stack Size (Ammo Only): x10")
                .SetAction(list => MaxStacks(list, Target.X10, Type.AMMO_ONLY)),
        };

        ModMaker.WriteMods(mods.ToList(), PathHelper.CHUNK_PATH, outPath, bundleName, true);
    }

    public static void MaxStacks(List<RszObject> rszObjectData, Target target, Type type) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Chainsaw_ItemDefinitionUserData_Data itemData:
                    var item = itemData.ItemDefineData[0];

                    if (type == Type.AMMO_ONLY) {
                        if (itemData.ItemId == ItemConstants.HANDGUN_AMMO
                            || itemData.ItemId == ItemConstants.SUBMACHINE_GUN_AMMO
                            || itemData.ItemId == ItemConstants.MAGNUM_AMMO
                            || itemData.ItemId == ItemConstants.SHOTGUN_SHELLS
                            || itemData.ItemId == ItemConstants.RIFLE_AMMO
                            || itemData.ItemId == ItemConstants.ATTACHABLE_MINES
                            || itemData.ItemId == ItemConstants.BOLTS) {
                            var newMax = GetNewMax(target, item);

                            if (item.StackMax < newMax) item.StackMax = newMax.Clamp(1, 9999);
                        }
                        break;
                    }

                    if ((item.StackMax > 1
                         || itemData.ItemId == ItemConstants.FIRST_AID_SPRAY
                         || itemData.ItemId == ItemConstants.GOLD_CHICKEN_EGG
                         || itemData.ItemId == ItemConstants.BROWN_CHICKEN_EGG
                         || itemData.ItemId == ItemConstants.CHICKEN_EGG
                         || itemData.ItemId == ItemConstants.HAND_GRENADE
                         || itemData.ItemId == ItemConstants.FLASH_GRENADE
                         || itemData.ItemId == ItemConstants.HEAVY_GRENADE
                         || itemData.ItemId == ItemConstants.RESOURCES_S
                         || itemData.ItemId == ItemConstants.RESOURCES_L
                         || itemData.ItemId == ItemConstants.VIPER
                         || itemData.ItemId == ItemConstants.BLACK_BASS
                         || itemData.ItemId == ItemConstants.BLACK_BASS_L
                         || itemData.ItemId == ItemConstants.LUNKER_BASS
                         || itemData.ItemId == ItemConstants.MIXED_HERB_G_PLUSG_PLUSG
                         || itemData.ItemId == ItemConstants.MIXED_HERB_G_PLUSG_PLUSY
                         || itemData.ItemId == ItemConstants.MIXED_HERB_G_PLUSR_PLUSY
                         || itemData.ItemId == ItemConstants.RHINOCEROS_BEETLE
                        )
                        && itemData.ItemId != ItemConstants.PESETAS
                       ) {
                        var newMax = GetNewMax(target, item);

                        if (item.StackMax < newMax) item.StackMax = newMax.Clamp(1, 9999);

                        if (itemData.ItemId == ItemConstants.HAND_GRENADE
                            || itemData.ItemId == ItemConstants.FLASH_GRENADE
                            || itemData.ItemId == ItemConstants.HEAVY_GRENADE
                            || itemData.ItemId == ItemConstants.GOLD_CHICKEN_EGG
                            || itemData.ItemId == ItemConstants.BROWN_CHICKEN_EGG
                            || itemData.ItemId == ItemConstants.CHICKEN_EGG
                           ) {
                            itemData.WeaponDefineData[0].StackMax = item.StackMax;
                        }
                    }
                    break;
            }
        }
    }

    private static int GetNewMax(Target target, Chainsaw_ItemDefiniition item) {
        var newMax = target switch {
            Target._999 => 999,
            Target._9999 => 9999,
            Target.X2 => item.StackMax * 2,
            Target.X3 => item.StackMax * 3,
            Target.X4 => item.StackMax * 4,
            Target.X5 => item.StackMax * 5,
            Target.X10 => item.StackMax * 10,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };
        return newMax;
    }

    public enum Target {
        _999,
        _9999,
        X2,
        X3,
        X4,
        X5,
        X10,
    }

    public enum Type {
        FULL,
        AMMO_ONLY,
    }
}