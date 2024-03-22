using RE_Editor.Common.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;

namespace RE_Editor.Generated;

public class Re4WeaponInstancer(string weaponCustomUserDataPath) {
    private Chainsaw_WeaponCustomUserdata weaponCustomUserData;

    public static Chainsaw_KeyItemInventoryItemSaveData NewKeyItem(RSZ rsz, uint type, int row, int col, int count) {
        var keyItem = Chainsaw_KeyItemInventoryItemSaveData.Create(rsz);
        keyItem.STRUCT_SlotIndex_Row      = row;
        keyItem.STRUCT_SlotIndex_Column   = col;
        keyItem.Item                      = [Chainsaw_Item.Create(rsz)];
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
        item.Item                      = [Chainsaw_Item.Create(rsz)];
        item.Item[0].CurrentItemCount  = count;
        item.Item[0].CurrentDurability = 1000;
        item.Item[0].CurrentCondition  = Chainsaw_ItemConditionFlag.Default;
        item.Item[0].ItemId            = (int) type;
        return item;
    }

    public Chainsaw_InventoryItemSaveData NewWeapon(RSZ rsz, uint type, uint weaponId, uint ammoType, int row, int col, int ammoCount = 1, int itemCount = 1, Chainsaw_ItemDirection rotation = Chainsaw_ItemDirection.Default) {
        var weapon = Chainsaw_InventoryItemSaveData.Create(rsz);
        weapon.SlotType                = Chainsaw_InventorySlotType.Default;
        weapon.STRUCT_SlotIndex_Row    = row;
        weapon.STRUCT_SlotIndex_Column = col;
        weapon.CurrDirection           = rotation;

        var weaponItem = Chainsaw_WeaponItem.Create(rsz);
        weapon.Item                         = [weaponItem];
        weaponItem.CurrentItemCount         = itemCount;
        weaponItem.CurrentDurability        = 1000;
        weaponItem.CurrentCondition         = Chainsaw_ItemConditionFlag.Default;
        weaponItem.ItemId                   = (int) type;
        weaponItem.CurrentAmmo              = (int) ammoType;
        weaponItem.CurrentAmmoCount         = ammoCount;
        weaponItem.CurrentTacticalAmmoCount = 0;
        weaponItem.CurrentWeaponPartsCustom = [];
        weaponItem.LimitBreakCustomPattern  = Chainsaw_gui_LimitBreakCustomPattern.None;

        var upgradeContainer = Chainsaw_CustomLevelInWeapon.Create(rsz);
        weaponItem.CurrentWeaponPartsCustom          = [Chainsaw_WeaponPartsCustom.Create(rsz)]; // Vanilla file just has an empty list.
        weaponItem.CurrentWeaponPartsCustom[0].Datas = [];
        weaponItem.CustomLevelInWeapon               = [upgradeContainer];
        upgradeContainer.IsReflect                   = true;

        CreateWeaponSpecificUpgrades(rsz, weaponId, upgradeContainer);

        return weapon;
    }

    private void CreateWeaponSpecificUpgrades(RSZ rsz, uint weaponId, Chainsaw_CustomLevelInWeapon upgradeContainer) {
        weaponCustomUserData ??= ReDataFile.Read(weaponCustomUserDataPath)
                                           .rsz
                                           .GetEntryObject<Chainsaw_WeaponCustomUserdata>();

        upgradeContainer.CommonLevelInWeapon     = [];
        upgradeContainer.IndividualLevelInWeapon = [];
        upgradeContainer.LimitBreakLevelInWeapon = [];

        var stage = weaponCustomUserData.WeaponStages.FirstOrDefault(stage => stage.WeaponID == weaponId)?.WeaponCustom[0];
        if (stage == null) return;

        foreach (var data in stage.Commons) {
            var commonLevelInWeapon = Chainsaw_CommonLevelInWeapon.Create(rsz);
            commonLevelInWeapon.CommonCustomCategory = data.CommonCustomCategory;
            upgradeContainer.CommonLevelInWeapon.Add(commonLevelInWeapon);
        }

        foreach (var data in stage.Individuals) {
            var commonLevelInWeapon = Chainsaw_IndividualLevelInWeapon.Create(rsz);
            commonLevelInWeapon.IndividualCustomCategory = data.IndividualCustomCategory;
            upgradeContainer.IndividualLevelInWeapon.Add(commonLevelInWeapon);
        }

        foreach (var data in stage.LimitBreak) {
            var commonLevelInWeapon = Chainsaw_LimitBreakLevelInWeapon.Create(rsz);
            commonLevelInWeapon.LimitBreakCustomCategory = data.LimitBreakCustomCategory;
            upgradeContainer.LimitBreakLevelInWeapon.Add(commonLevelInWeapon);
        }
    }
}