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
public class OpMercenariesStart : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "OP Mercenaries Start";
        const string description = "Max <all things stackable>/case size for all mercenaries starter inventories.";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var mods = new NexusMod {
            Name    = bundleName,
            Version = version,
            Desc    = description,
            Files   = PathHelper.NEW_GAME_INVENTORY_MC_DATA_PATHS,
            Action  = MakeNewInventory
        };

        ModMaker.WriteMods(new List<NexusMod> {mods}, PathHelper.CHUNK_PATH, outPath, copyToFluffy: true);
    }

    public static void MakeNewInventory(IEnumerable<RszObject> rszObjectData) {
        foreach (var obj in new List<RszObject>(rszObjectData)) {
            if (obj is not Chainsaw_InventoryCatalogUserData inventoryData) continue;

            inventoryData.Datas[0].InventoryData[0].InventorySize[0].CurrInventorySize = Chainsaw_AttacheCaseSize.XXL;

            foreach (var item in inventoryData.Datas[0].InventoryData[0].InventoryItems) {
                item.Item[0].CurrentItemCount = 9999;
                if (item.Item[0] is Chainsaw_WeaponItem weapon) {
                    weapon.CurrentItemCount = 9999;
                }
            }
        }
    }
}