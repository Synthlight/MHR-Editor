using RE_Editor.Common.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;

namespace RE_Editor.Generated;

public static class Re4WeaponInstancer {
    public static Chainsaw_KeyItemInventoryItemSaveData NewKeyItem(RSZ rsz, uint type, int row, int col, int count) {
        var keyItem = Chainsaw_KeyItemInventoryItemSaveData.Create(rsz);
        keyItem.STRUCT_SlotIndex_Row      = row;
        keyItem.STRUCT_SlotIndex_Column   = col;
        keyItem.Item                      = new() {Chainsaw_Item.Create(rsz)};
        keyItem.Item[0].CurrentItemCount  = count;
        keyItem.Item[0].CurrentDurability = 1000;
        keyItem.Item[0].CurrentCondition  = Chainsaw_ItemConditionFlag.Default;
        keyItem.Item[0].ItemId            = (int) type;
        return keyItem;
    }

    public static Chainsaw_InventoryItemSaveData NewItem(RSZ rsz, uint type, int row, int col, int count, Chainsaw_ItemDirection rotation = Chainsaw_ItemDirection.Default) {
        var item = Chainsaw_InventoryItemSaveData.Create(rsz);
        item.STRUCT_SlotIndex_Row      = row;
        item.STRUCT_SlotIndex_Column   = col;
        item.CurrDirection             = rotation;
        item.Item                      = new() {Chainsaw_Item.Create(rsz)};
        item.Item[0].CurrentItemCount  = count;
        item.Item[0].CurrentDurability = 1000;
        item.Item[0].CurrentCondition  = Chainsaw_ItemConditionFlag.Default;
        item.Item[0].ItemId            = (int) type;
        return item;
    }

    public static Chainsaw_InventoryItemSaveData NewWeapon(RSZ rsz, uint type, uint ammoType, int row, int col, int ammoCount = 1, int itemCount = 1, Chainsaw_ItemDirection rotation = Chainsaw_ItemDirection.Default) {
        var weapon = Chainsaw_InventoryItemSaveData.Create(rsz);
        weapon.SlotType                = Chainsaw_InventorySlotType.Default;
        weapon.STRUCT_SlotIndex_Row    = row;
        weapon.STRUCT_SlotIndex_Column = col;
        weapon.CurrDirection           = rotation;

        var weaponItem = Chainsaw_WeaponItem.Create(rsz);
        weapon.Item                         = new() {weaponItem};
        weaponItem.CurrentItemCount         = itemCount;
        weaponItem.CurrentDurability        = 1000;
        weaponItem.CurrentCondition         = Chainsaw_ItemConditionFlag.Default;
        weaponItem.ItemId                   = (int) type;
        weaponItem.CurrentAmmo              = (int) ammoType;
        weaponItem.CurrentAmmoCount         = ammoCount;
        weaponItem.CurrentTacticalAmmoCount = 0;
        weaponItem.CurrentWeaponPartsCustom = new();
        weaponItem.LimitBreakCustomPattern  = Chainsaw_gui_LimitBreakCustomPattern.None;

        var upgradeContainer = Chainsaw_CustomLevelInWeapon.Create(rsz);
        weaponItem.CustomLevelInWeapon = new() {upgradeContainer};
        upgradeContainer.IsReflect     = true;

        CreateWeaponSpecificUpgrades(rsz, type, upgradeContainer);

        return weapon;
    }

    private static void CreateWeaponSpecificUpgrades(RSZ rsz, uint type, Chainsaw_CustomLevelInWeapon upgradeContainer) {
        // TODO: Instance these with the right levels and things which needs to be different for each weapon.
        // The current lists are not accurate for each weapon either.

        upgradeContainer.CommonLevelInWeapon     = new() {Chainsaw_CommonLevelInWeapon.Create(rsz), Chainsaw_CommonLevelInWeapon.Create(rsz)};
        upgradeContainer.IndividualLevelInWeapon = new() {Chainsaw_IndividualLevelInWeapon.Create(rsz), Chainsaw_IndividualLevelInWeapon.Create(rsz)};
        upgradeContainer.LimitBreakLevelInWeapon = new() {Chainsaw_LimitBreakLevelInWeapon.Create(rsz)};
    }
}