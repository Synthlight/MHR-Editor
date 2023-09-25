using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Generated;
using RE_Editor.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class OpSeparateWaysStart : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "OP Separate Ways Start";
        const string description = "Max <all things stackable>/case size for Separate Ways starter inventories.";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var mods = new NexusMod {
            Name    = bundleName,
            Version = version,
            Desc    = description,
            Files   = new[] {PathHelper.NEW_GAME_INVENTORY_AO_DATA_PATH},
            Action  = MakeNewInventory
        };

        ModMaker.WriteMods(new List<NexusMod> {mods}, PathHelper.CHUNK_PATH, outPath, copyToFluffy: true);
    }

    public static void MakeNewInventory(IEnumerable<RszObject> rszObjectData) {
        foreach (var obj in new List<RszObject>(rszObjectData)) {
            if (obj is not Chainsaw_InventoryCatalogUserData inventoryData) continue;

            foreach (var data in inventoryData.Datas) {
                data.InventoryData[0].InventorySize[0].CurrInventorySize = Chainsaw_AttacheCaseSize.XXL;
                data.KeyInventorySaveData[0].Items.Add(Re4WeaponInstancer.NewKeyItem(obj.rsz, ItemConstants_CH.SMALL_KEY, 0, 0, 20));

                foreach (var item in data.InventoryData[0].InventoryItems) {
                    item.Item[0].CurrentItemCount = 8999;
                    if (item.Item[0] is Chainsaw_WeaponItem weapon) {
                        weapon.CurrentItemCount = 8999;
                    }
                }
            }
        }
    }
}