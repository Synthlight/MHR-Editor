using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class InfiniteAmmo : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name        = "Infinite Ammo and Coatings";
        const string description = "Infinite Ammo and Coatings.";
        const string version     = "1.0.0";

        var mod = new NexusMod {
            Name    = name,
            Version = version,
            Desc    = description,
            Files   = [PathHelper.ITEM_DATA_PATH],
            Action  = InfiniteAmmoAndCoatings
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true);
    }

    public static void InfiniteAmmoAndCoatings(IList<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_user_data_ItemData_cData item:
                    if (item.Type is App_ItemDef_TYPE_Fixed.SHELL or App_ItemDef_TYPE_Fixed.BOTTLE) {
                        item.Infinit = true;
                    }
                    break;
            }
        }
    }
}