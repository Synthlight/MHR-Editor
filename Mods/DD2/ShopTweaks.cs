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
        const string version     = "1.3";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var mod = new NexusMod {
            Version     = version,
            Name        = bundleName,
            Desc        = description,
            Files       = [PathHelper.ITEM_SHOP_DATA_PATH],
            Action      = ShopData,
            MakeIntoPak = true
        };

        ModMaker.WriteMods([mod], PathHelper.CHUNK_PATH, outPath, bundleName, true);
    }

    public static void ShopData(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_ItemShopParam shopData:
                    if (shopData.ShopId == 59) continue; // The `Seekers Token` shop which isn't really a shop.
                    if (shopData.BuyParams.Count > 0) {
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.ART_OF_METAMORPHOSIS, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.FERRYSTONE, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.PORTCRYSTAL, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.ELITE_CAMPING_KIT, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.EXPLORERS_CAMPING_KIT, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.ETERNAL_WAKESTONE, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.WAKESTONE, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.GIANT_HUNK_OF_RIFT_CRYSTAL, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.SEEKERS_TOKEN, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.FINDERS_TOKEN, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.UNMAKING_ARROW, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.GOLDEN_TROVE_BEETLE, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.SEALING_PHIAL, 255));
                        shopData.BuyParams.Add(App_ItemShopBuyParam.MakeNewBuyItem(shopData.rsz, (int) ItemConstants.WYRMSLIFE_CRYSTAL, 255));
                    }
                    break;
            }
        }
    }
}