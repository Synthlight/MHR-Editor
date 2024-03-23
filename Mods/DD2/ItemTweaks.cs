using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class ItemTweaks : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "Item Tweaks";
        const string description = "Item/Equipment weight and cost changes.";
        const string version     = "1.7";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var itemDataFiles = new List<string> {
            PathHelper.ARMOR_DATA_PATH,
            PathHelper.ITEM_DATA_PATH,
            PathHelper.WEAPON_DATA_PATH,
        };

        var baseMod = new NexusModVariant {
            Version      = version,
            NameAsBundle = bundleName,
            Desc         = description,
            Files        = itemDataFiles
        };

        var mods = new[] {
            baseMod
                .SetName("No Item Decay")
                .SetAction(list => Decay(list, DecayOptions._0)),
            baseMod
                .SetName("Stack Size: 9999")
                .SetAction(list => Stack(list, StackOptions._9999)),
            baseMod
                .SetName("Cost: 1 Gold")
                .SetAction(list => GoldCost(list, GoldOptions._1)),
            baseMod
                .SetName("Cost: 1 Gold, x10 Sell Price")
                .SetAction(list => {
                    GoldCost(list, GoldOptions._1);
                    SellPrice(list, SellOptions.X10);
                }),
            baseMod
                .SetName("Cost: 1 Gold, Weight: 0")
                .SetAction(list => {
                    GoldCost(list, GoldOptions._1);
                    Weight(list, WeightOptions._0);
                }),
            baseMod
                .SetName("Cost: 1 Gold, Weight: 0 (Ferrystone Only)")
                .SetAction(list => {
                    GoldCost(list, GoldOptions._1_Ferry_Only);
                    Weight(list, WeightOptions._0_Ferry_Only);
                }),
            baseMod
                .SetName("Weight: 0")
                .SetAction(list => Weight(list, WeightOptions._0)),
            baseMod
                .SetName("Weight: 0, x10 Sell Price")
                .SetAction(list => {
                    Weight(list, WeightOptions._0);
                    SellPrice(list, SellOptions.X10);
                }),
            baseMod
                .SetName("Weight: 0 (All Items Only)")
                .SetFiles([PathHelper.ITEM_DATA_PATH])
                .SetAction(list => Weight(list, WeightOptions._0)),
            baseMod
                .SetName("Weight: 0 (Materials Only)")
                .SetAction(list => Weight(list, WeightOptions._0_Materials)),
            baseMod
                .SetName("x10 Sell Price")
                .SetAction(list => SellPrice(list, SellOptions.X10)),
            baseMod
                .SetName("All-in-One (Item Tweaks)")
                .SetAction(list => {
                    GoldCost(list, GoldOptions._1);
                    SellPrice(list, SellOptions.X10);
                    Weight(list, WeightOptions._0);
                    Decay(list, DecayOptions._0);
                    Stack(list, StackOptions._9999);
                }),
        };

        ModMaker.WriteMods(mods.ToList(), PathHelper.CHUNK_PATH, outPath, bundleName, true, makeIntoPak: true);
    }

    public static void GoldCost(List<RszObject> rszObjectData, GoldOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemDataParam itemData:
                    switch (option) {
                        case GoldOptions._1:
                            itemData.BuyPrice = 1;
                            break;
                        case GoldOptions._1_Ferry_Only:
                            if (itemData.Id == ItemConstants.FERRYSTONE) {
                                itemData.BuyPrice = 1;
                            }
                            break;
                    }
                    break;
                case App_ItemWeaponParam itemData:
                    switch (option) {
                        case GoldOptions._1:
                            itemData.BuyPrice = 1;
                            break;
                    }
                    break;
                case App_ItemArmorParam itemData:
                    switch (option) {
                        case GoldOptions._1:
                            itemData.BuyPrice = 1;
                            break;
                    }
                    break;
            }
        }
    }

    public static void SellPrice(List<RszObject> rszObjectData, SellOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemDataParam itemData:
                    switch (option) {
                        case SellOptions.X10:
                            itemData.SellPrice *= 10;
                            break;
                    }
                    break;
                case App_ItemWeaponParam itemData:
                    switch (option) {
                        case SellOptions.X10:
                            itemData.SellPrice *= 10;
                            break;
                    }
                    break;
                case App_ItemArmorParam itemData:
                    switch (option) {
                        case SellOptions.X10:
                            itemData.SellPrice *= 10;
                            break;
                    }
                    break;
            }
        }
    }

    public static void Weight(List<RszObject> rszObjectData, WeightOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemDataParam itemData:
                    switch (option) {
                        case WeightOptions._0:
                            itemData.Weight = 0;
                            break;
                        case WeightOptions._0_Ferry_Only:
                            if (itemData.Id == ItemConstants.FERRYSTONE) {
                                itemData.Weight = 0;
                            }
                            break;
                        case WeightOptions._0_Materials:
                            if (itemData.Category == App_ItemCategory.Material
                                || itemData.SubCategory == App_ItemSubCategory.Material) {
                                itemData.Weight = 0;
                            }
                            break;
                    }
                    break;
                case App_ItemWeaponParam itemData:
                    switch (option) {
                        case WeightOptions._0:
                            itemData.Weight = 0;
                            break;
                    }
                    break;
                case App_ItemArmorParam itemData:
                    switch (option) {
                        case WeightOptions._0:
                            itemData.Weight = 0;
                            break;
                    }
                    break;
            }
        }
    }

    public static void Decay(List<RszObject> rszObjectData, DecayOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemDataParam itemData:
                    switch (option) {
                        case DecayOptions._0:
                            itemData.Decay         = 0;
                            itemData.DecayedItemId = 0;
                            break;
                    }
                    break;
            }
        }
    }

    public static void Stack(List<RszObject> rszObjectData, StackOptions option) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemDataParam itemData:
                    switch (option) {
                        case StackOptions.X10:
                            if (itemData.StackNum > 1) itemData.StackNum *= 10;
                            break;
                        case StackOptions._9999:
                            if (itemData.StackNum > 1) itemData.StackNum = 0;
                            break;
                    }
                    break;
            }
        }
    }

    public enum GoldOptions {
        NORMAL,
        _1,
        _1_Ferry_Only,
    }

    public enum WeightOptions {
        NORMAL,
        _0,
        _0_Ferry_Only,
        _0_Materials,
    }

    public enum SellOptions {
        NORMAL,
        X10,
    }

    public enum DecayOptions {
        NORMAL,
        _0,
    }

    public enum StackOptions {
        NORMAL,
        X10,
        _9999,
    }
}