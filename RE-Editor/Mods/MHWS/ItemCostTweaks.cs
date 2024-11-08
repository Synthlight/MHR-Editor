using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class ItemCostTweaks : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name = "Item Cost Tweaks - Buy for 1z, sell for 99999z";

        var baseMod = new NexusMod {
            Version      = "1.0.0",
            NameAsBundle = name,
            Desc         = "Buy for 1z, sell for 99999z."
        };

        var mods = new List<INexusMod> {
            baseMod
                .SetName("Item Cost Tweaks - Buy Price Only")
                .SetFiles([PathHelper.ITEM_DATA_PATH])
                .SetAction(list => ItemCostTweak(list, Mode.BUY_PRICE)),
            baseMod
                .SetName("Item Cost Tweaks - Sell Price Only")
                .SetFiles([PathHelper.ITEM_DATA_PATH])
                .SetAction(list => ItemCostTweak(list, Mode.SELL_PRICE)),
            baseMod
                .SetName("Item Cost Tweaks - Both")
                .SetFiles([PathHelper.ITEM_DATA_PATH])
                .SetAction(list => ItemCostTweak(list, Mode.BUY_PRICE | Mode.SELL_PRICE))
        };

        ModMaker.WriteMods(mods, name, copyLooseToFluffy: true);
    }

    public static void ItemCostTweak(IList<RszObject> rszObjectData, Mode mode) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_user_data_ItemData_cData item:
                    if (mode.HasFlag(Mode.BUY_PRICE)) item.BuyPrice   = 1;
                    if (mode.HasFlag(Mode.SELL_PRICE)) item.SellPrice = 99999;
                    break;
            }
        }
    }

    [Flags]
    public enum Mode {
        BUY_PRICE,
        SELL_PRICE
    }
}