using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class ShopTweaks : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "Crazy & Metamorphosis Merged and in Every Shop";
        const string description = "Adds the items they both offer, but in every single shop in the game.";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var mod = new NexusMod {
            Version = version,
            Name    = bundleName,
            Desc    = description,
            Files   = [PathHelper.ITEM_SHOP_DATA_PATH],
            Action  = ShopData
        };

        ModMaker.WriteMods([mod], PathHelper.CHUNK_PATH, outPath, bundleName, true, makeIntoPak: true);
    }

    public static void ShopData(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemShopParam shopData:
                    if (shopData.BuyParams.Count > 0) {
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.ART_OF_METAMORPHOSIS, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.FERRYSTONE, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.PORTCRYSTAL, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.ELITE_CAMPING_KIT, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.EXPLORERS_CAMPING_KIT, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.ETERNAL_WAKESTONE, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.WAKESTONE, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.GIANT_HUNK_OF_RIFT_CRYSTAL, 255));
                    }
                    break;
            }
        }
    }
}