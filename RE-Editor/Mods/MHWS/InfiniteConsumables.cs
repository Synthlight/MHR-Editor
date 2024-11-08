using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class InfiniteConsumables : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name        = "Infinite Consumables";
        const string description = "Infinite Consumables.";
        const string version     = "1.0.0";

        var mod = new NexusMod {
            Name    = name,
            Version = version,
            Desc    = description,
            Files   = [PathHelper.ITEM_DATA_PATH],
            Action  = InfiniteConsumableItems
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true);
    }

    public static void InfiniteConsumableItems(IList<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_user_data_ItemData_cData item:
                    // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                    switch ((uint) item.ItemId) {
                        case ItemConstants.POTION:
                        case ItemConstants.MEGA_POTION:
                        case ItemConstants.ANTIDOTE:
                        case ItemConstants.MAX_POTION:
                        case ItemConstants.NULBERRY:
                        case ItemConstants.WELL_DONE_STEAK:
                        case ItemConstants.RARE_STEAK:
                        case ItemConstants.LIFEPOWDER:
                        case ItemConstants.BARREL_BOMB:
                        case ItemConstants.LARGE_BARREL_BOMB:
                        case ItemConstants.FLASH_POD:
                        case ItemConstants.PITFALL_TRAP:
                        case ItemConstants.SHOCK_TRAP:
                        case ItemConstants.ANTIDOTE_HERB:
                        case ItemConstants.LARGE_DUNG_POD:
                        case ItemConstants.LURING_POD:
                        case ItemConstants.SCREAMER_POD:
                        case ItemConstants.ARMORSKIN:
                        case ItemConstants.DEMONDRUG:
                        case ItemConstants.TRANQ_BOMB:
                        case ItemConstants.SMOKE_BOMB:
                        case ItemConstants.POISON_SMOKE_BOMB:
                        case ItemConstants.WHETFISH_FIN:
                        case ItemConstants.FARCASTER:
                            item.Infinit = true;
                            break;
                    }
                    break;
            }
        }
    }
}