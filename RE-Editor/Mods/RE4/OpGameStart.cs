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
        const string name        = "OP Game Start";
        const string description = "Gives OP weapon setups on new games.";
        const string version     = "1.1";

        var baseMod = new NexusMod {
            Version      = version,
            NameAsBundle = name,
            Desc         = description,
            Files        = [PathHelper.NEW_GAME_INVENTORY_DATA_PATH]
        };

        var mods = new[] {
            baseMod
                .SetName($"{name}: With Bonus Weapons")
                .SetAction(list => MakeNewInventory(list, Target.WITH_BONUS_WEAPONS)),
            baseMod
                .SetName($"{name}: Without Bonus Weapons")
                .SetAction(list => MakeNewInventory(list, Target.WITHOUT_BONUS_WEAPONS)),
            baseMod
                .SetName($"{name}: Just Max Case Size")
                .SetAction(list => MakeNewInventory(list, Target.JUST_MAX_CASE_SIZE)),
            baseMod
                .SetName($"{name}: Just Small Keys")
                .SetAction(list => MakeNewInventory(list, Target.JUST_SMALL_KEYS)),
        };

        ModMaker.WriteMods(mods, name, copyLooseToFluffy: true);
    }

    public static void MakeNewInventory(IEnumerable<RszObject> rszObjectData, Target target) {
        var re4WeaponInstancer = new Re4WeaponInstancer($@"{PathHelper.CHUNK_PATH}\natives\STM\_Chainsaw\AppSystem\WeaponCustom\WeaponCustomUserdata.user.2");

        foreach (var obj in new List<RszObject>(rszObjectData)) {
            if (obj is not Chainsaw_InventoryCatalogUserData inventoryData) continue;

            if (target is Target.JUST_MAX_CASE_SIZE or Target.WITH_BONUS_WEAPONS or Target.WITHOUT_BONUS_WEAPONS) {
                inventoryData.Datas[0].InventoryData[0].InventorySize[0].CurrInventorySize = Chainsaw_AttacheCaseSize.XXL;
            }
            if (target is Target.JUST_SMALL_KEYS or Target.WITH_BONUS_WEAPONS or Target.WITHOUT_BONUS_WEAPONS) {
                // Needs to be in the key inventory.
                inventoryData.Datas[0].KeyInventorySaveData[0].Items.Add(Re4WeaponInstancer.NewKeyItem(obj.rsz, ItemConstants_CH.SMALL_KEY, 0, 0, 20));
            }
            if (target is Target.WITH_BONUS_WEAPONS or Target.WITHOUT_BONUS_WEAPONS) {
                AddWeapons(obj, inventoryData, target, re4WeaponInstancer);
            }

            break;
        }
    }

    private static void AddWeapons(RszObject obj, Chainsaw_InventoryCatalogUserData inventoryData, Target target, Re4WeaponInstancer re4WeaponInstancer) {
        var items = inventoryData.Datas[0].InventoryData[0].InventoryItems;
        var knife = items[2]; // Knife.

        const int count = 8888;

        knife.STRUCT_SlotIndex_Row    = 8;
        knife.STRUCT_SlotIndex_Column = 10;

        items.RemoveAt(0); // Remove the first aid spray.

        // Row 1-2
        items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.STINGRAY, WeaponConstants_CH.STINGRAY, ItemConstants_CH.RIFLE_AMMO, 0, 3, ammoCount: 10));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.MIXED_HERB_G_PLUSR_PLUSY, 0, 10, count, Chainsaw_ItemDirection.Rot_090));
        items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.GOLD_CHICKEN_EGG, 0, ItemConstants_CH.GOLD_CHICKEN_EGG, 0, 12, itemCount: count)); // Row 1
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.HIGH_POWER_SCOPE, 1, 10, count)); // Row 2
        // Row 3-4
        items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.RED9, WeaponConstants_CH.RED9, ItemConstants_CH.HANDGUN_AMMO, 2, 8, ammoCount: 8));
        items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.HEAVY_GRENADE, 01, ItemConstants_CH.HEAVY_GRENADE, 2, 11, itemCount: count));
        items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.FLASH_GRENADE, 0, ItemConstants_CH.FLASH_GRENADE, 2, 12, itemCount: count));
        // Row 5-6
        items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.KILLER7, WeaponConstants_CH.KILLER7, ItemConstants_CH.MAGNUM_AMMO, 4, 8, ammoCount: 7));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.BIOSENSOR_SCOPE, 4, 11, 1));
        // Row 7-8
        items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.PUNISHER, WeaponConstants_CH.PUNISHER, ItemConstants_CH.HANDGUN_AMMO, 6, 8, ammoCount: 12));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.HANDGUN_AMMO, 6, 11, count));
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.RIFLE_AMMO, 7, 11, count));
        // Row 9
        items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.RED9_STOCK, 8, 6, 1));

        switch (target) {
            case Target.WITH_BONUS_WEAPONS:
                // Row 3-5
                items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.CHICAGO_SWEEPER, WeaponConstants_CH.CHICAGO_SWEEPER, ItemConstants_CH.SUBMACHINE_GUN_AMMO, 2, 0, ammoCount: 50));
                //items.Add(NewWeapon(obj.rsz, ItemConstants_CH.PRIMAL_KNIFE, 0xFFFFFFFF, 2, 7, ammoCount: 0, rotation: Chainsaw_ItemDirection.Rot_090));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.LASER_SIGHT, 2, 7, 1));
                // Row 6-7
                items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.INFINITE_ROCKET_LAUNCHER, WeaponConstants_CH.INFINITE_ROCKET_LAUNCHER, ItemConstants_CH.INFINITE_ROCKET_LAUNCHER, 5, 0));
                // Row 8-9
                items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.HANDCANNON, WeaponConstants_CH.HANDCANNON, ItemConstants_CH.MAGNUM_AMMO, 7, 0, ammoCount: 5));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.SHOTGUN_SHELLS, 7, 4, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.SUBMACHINE_GUN_AMMO, 7, 6, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.MAGNUM_AMMO, 8, 4, count));
                break;
            case Target.WITHOUT_BONUS_WEAPONS:
                // Row 3-4
                items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.ROCKET_LAUNCHER, 0, ItemConstants_CH.ROCKET_LAUNCHER, 2, 0));
                // Row 5-6
                items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.ROCKET_LAUNCHER, 0, ItemConstants_CH.ROCKET_LAUNCHER, 4, 0));
                // Row 7-8
                items.Add(re4WeaponInstancer.NewWeapon(obj.rsz, ItemConstants_CH.ROCKET_LAUNCHER, 0, ItemConstants_CH.ROCKET_LAUNCHER, 6, 0));
                // Row 9
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.SHOTGUN_SHELLS, 8, 0, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.SUBMACHINE_GUN_AMMO, 8, 2, count));
                items.Add(Re4WeaponInstancer.NewItem(obj.rsz, ItemConstants_CH.MAGNUM_AMMO, 8, 4, count));
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