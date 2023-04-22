using System;
using System.Collections.Generic;
using System.Linq;
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
public class OpGameStart : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "OP Game Start";
        const string description = "Gives OP weapon setups on new games.";
        const string version     = "1.1";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = version,
            NameAsBundle = bundleName,
            Desc         = description,
            Files        = new[] {PathHelper.NEW_GAME_INVENTORY_DATA_PATH}
        };

        var mods = new[] {
            baseMod
                .SetName($"{bundleName}: With Bonus Weapons")
                .SetAction(list => MakeNewInventory(list, Target.WITH_BONUS_WEAPONS)),
            baseMod
                .SetName($"{bundleName}: Without Bonus Weapons")
                .SetAction(list => MakeNewInventory(list, Target.WITHOUT_BONUS_WEAPONS)),
            baseMod
                .SetName($"{bundleName}: Just Max Case Size")
                .SetAction(list => MakeNewInventory(list, Target.JUST_MAX_CASE_SIZE)),
            baseMod
                .SetName($"{bundleName}: Just Small Keys")
                .SetAction(list => MakeNewInventory(list, Target.JUST_SMALL_KEYS)),
        };

        ModMaker.WriteMods(mods.ToList(), PathHelper.CHUNK_PATH, outPath, bundleName, true);
    }

    public static void MakeNewInventory(IEnumerable<RszObject> rszObjectData, Target target) {
        foreach (var obj in new List<RszObject>(rszObjectData)) {
            if (obj is not Chainsaw_InventoryCatalogUserData inventoryData) continue;

            switch (target) {
                case Target.JUST_MAX_CASE_SIZE:
                    inventoryData.Datas[0].InventoryData[0].InventorySize[0].CurrInventorySize = Chainsaw_AttacheCaseSize.XXL;
                    break;
                case Target.JUST_SMALL_KEYS:
                    // Needs to be in the key inventory.
                    inventoryData.Datas[0].KeyInventorySaveData[0].Items.Add(Re4WeaponInstancer.NewKeyItem(obj.rsz, ItemConstants.SMALL_KEY, 0, 0, 20));
                    break;
                case Target.WITH_BONUS_WEAPONS:
                case Target.WITHOUT_BONUS_WEAPONS:
                    AddWeapons(obj, inventoryData, target);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }

            break;
        }
    }

    private static void AddWeapons(RszObject obj, Chainsaw_InventoryCatalogUserData inventoryData, Target target) {
        var items = inventoryData.Datas[0].InventoryData[0].InventoryItems;
        var knife = items[2]; // Knife.

        const int count = 8888;

        knife.STRUCT_SlotIndex_Row    = 8;
        knife.STRUCT_SlotIndex_Column = 10;

        // Row 1-2
        items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.STINGRAY, ItemConstants.RIFLE_AMMO, 0, 3, ammoCount: 10));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.MIXED_HERB_G_PLUSR_PLUSY, 0, 10, count));
        items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.GOLD_CHICKEN_EGG, ItemConstants.GOLD_CHICKEN_EGG, 0, 12, itemCount: count)); // Row 1
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.HIGH_POWER_SCOPE, 1, 10, count)); // Row 2
        // Row 3-4
        items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.RED9, ItemConstants.HANDGUN_AMMO, 2, 8, ammoCount: 8));
        items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.HEAVY_GRENADE, ItemConstants.HEAVY_GRENADE, 2, 11, itemCount: count));
        items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.FLASH_GRENADE, ItemConstants.FLASH_GRENADE, 2, 12, itemCount: count));
        // Row 5-6
        items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.KILLER7, ItemConstants.MAGNUM_AMMO, 4, 8, ammoCount: 7));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.BIOSENSOR_SCOPE, 4, 11, 1));
        // Row 7-8
        items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.PUNISHER, ItemConstants.HANDGUN_AMMO, 6, 8, ammoCount: 12));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.HANDGUN_AMMO, 6, 11, count));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.RIFLE_AMMO, 7, 11, count));
        // Row 9
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.RED9_STOCK, 8, 6, 1));

        switch (target) {
            case Target.WITH_BONUS_WEAPONS:
                // Row 3-5
                items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.CHICAGO_SWEEPER, ItemConstants.SUBMACHINE_GUN_AMMO, 2, 0, ammoCount: 50));
                //items.Add(NewWeapon(obj.rsz, ItemConstants.PRIMAL_KNIFE, 0xFFFFFFFF, 2, 7, ammoCount: 0, rotation: Chainsaw_ItemDirection.Rot_090));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.LASER_SIGHT, 2, 7, 1));
                // Row 6-7
                items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.INFINITE_ROCKET_LAUNCHER, ItemConstants.INFINITE_ROCKET_LAUNCHER, 5, 0));
                // Row 8-9
                items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.HANDCANNON, ItemConstants.MAGNUM_AMMO, 7, 0, ammoCount: 5));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.SHOTGUN_SHELLS, 7, 4, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.SUBMACHINE_GUN_AMMO, 7, 6, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.MAGNUM_AMMO, 8, 4, count));
                break;
            case Target.WITHOUT_BONUS_WEAPONS:
                // Row 3-4
                items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.ROCKET_LAUNCHER, ItemConstants.ROCKET_LAUNCHER, 2, 0));
                // Row 5-6
                items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.ROCKET_LAUNCHER, ItemConstants.ROCKET_LAUNCHER, 4, 0));
                // Row 7-8
                items.Add(Re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants.ROCKET_LAUNCHER, ItemConstants.ROCKET_LAUNCHER, 6, 0));
                // Row 9
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.SHOTGUN_SHELLS, 8, 0, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.SUBMACHINE_GUN_AMMO, 8, 2, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants.MAGNUM_AMMO, 8, 4, count));
                break;
            default: throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
        return;
    }

    public enum Target {
        WITH_BONUS_WEAPONS,
        WITHOUT_BONUS_WEAPONS,
        JUST_MAX_CASE_SIZE,
        JUST_SMALL_KEYS,
    }
}