﻿using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Generated.Models;
using RE_Editor.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class CheatMod : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName        = "Weapon/Armor/Gem/Slot/Skill Cheats";
        const string variantBundleName = "All In One (Fluffy Selective Install)";
        const string outPath           = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = "1.7",
            NameAsBundle = bundleName,
            Desc         = "A cheat mod."
        };

        var bundleMods = new[] {
            baseMod
                .SetName("Armor With Max Slots Only")
                .SetFiles(new[] {PathHelper.ARMOR_BASE_PATH})
                .SetAction(MaxSlots),
            baseMod
                .SetName("Armor With Max Skills Only")
                .SetFiles(new[] {PathHelper.ARMOR_BASE_PATH})
                .SetAction(MaxSkills),
            baseMod
                .SetName("Armor With Max Slots & Skills")
                .SetFiles(new[] {PathHelper.ARMOR_BASE_PATH})
                .SetAction(data => {
                    MaxSlots(data);
                    MaxSkills(data);
                }),
            baseMod
                .SetName("Weapons With Max Slots Only")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base))
                .SetAction(MaxSlots),
            baseMod
                .SetName("Weapons With Max Sharpness Only")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base))
                .SetAction(MaxSharpness),
            baseMod
                .SetName("Weapons With Max Slots & Sharpness")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base))
                .SetAction(data => {
                    MaxSlots(data);
                    MaxSharpness(data);
                }),
            baseMod
                .SetName("One Gem to Max Skill")
                .SetFiles(new[] {PathHelper.DECORATION_PATH})
                .SetAction(MaxSkills),
            baseMod
                .SetName("GunLance With Lvl 8 Shelling Only")
                .SetFiles(new[] {PathHelper.GUN_LANCE_BASE_DATA_PATH})
                .SetAction(Lvl8Shelling),
            baseMod
                .SetName("GunLance With Lvl 8 Shelling & Max Slots")
                .SetFiles(new[] {PathHelper.GUN_LANCE_BASE_DATA_PATH})
                .SetAction(data => {
                    Lvl8Shelling(data);
                    MaxSlots(data);
                }),
            baseMod
                .SetName("GunLance With Lvl 8 Shelling & Max Sharpness")
                .SetFiles(new[] {PathHelper.GUN_LANCE_BASE_DATA_PATH})
                .SetAction(data => {
                    Lvl8Shelling(data);
                    MaxSharpness(data);
                }),
            baseMod
                .SetName("GunLance With Lvl 8 Shelling, Max Slots & Sharpness")
                .SetFiles(new[] {PathHelper.GUN_LANCE_BASE_DATA_PATH})
                .SetAction(data => {
                    Lvl8Shelling(data);
                    MaxSlots(data);
                    MaxSharpness(data);
                }),
        };

        ModMaker.WriteMods(bundleMods, PathHelper.CHUNK_PATH, outPath, variantBundleName, true);
    }

    public static void MaxSharpness(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case ISharpness weapon:
                    weapon.SharpnessValList[0].Value = 10;
                    weapon.SharpnessValList[1].Value = 10;
                    weapon.SharpnessValList[2].Value = 10;
                    weapon.SharpnessValList[3].Value = 10;
                    weapon.SharpnessValList[4].Value = 10;
                    weapon.SharpnessValList[5].Value = 10;
                    weapon.SharpnessValList[6].Value = 340;
                    foreach (var handicraft in weapon.HandicraftValList) {
                        handicraft.Value = 0;
                    }
                    break;
            }
        }
    }

    public static void MaxSlots(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Snow_data_ArmorBaseUserData_Param armor:
                    armor.DecorationsNumList[0].Value =
                        armor.DecorationsNumList[1].Value =
                            armor.DecorationsNumList[2].Value = 0;
                    armor.DecorationsNumList[3].Value = 3;
                    break;
            }
            if (obj is ISlots slots) {
                slots.SlotNumList[0].Value =
                    slots.SlotNumList[1].Value =
                        slots.SlotNumList[2].Value = 0;
                slots.SlotNumList[3].Value = 3;
            }
            if (obj is IRampageSlots rampageSlots) {
                rampageSlots.RampageSlotNumList[0].Value =
                    rampageSlots.RampageSlotNumList[1].Value = 0;
                rampageSlots.RampageSlotNumList[2].Value = 1;
            }
        }
    }

    public static void MaxSkills(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Snow_data_ArmorBaseUserData_Param armor:
                    foreach (var level in armor.SkillLvList) {
                        if (level.Value > 0) level.Value = 10;
                    }
                    break;
                case Snow_data_DecorationsBaseUserData_Param decoration:
                    if (decoration.SkillLvList[0].Value > 0) decoration.SkillLvList[0].Value = 10;
                    break;
            }
        }
    }

    public static void NoCost(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case ICraftingCost recipe:
                    foreach (var item in recipe.ItemIdList) {
                        item.Value = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    }
                    foreach (var itemNum in recipe.ItemNumList) {
                        itemNum.Value = 0;
                    }
                    break;
            }
            switch (obj) {
                case Snow_data_WeaponProductUserData_Param weaponProdData:
                    weaponProdData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.None;
                    weaponProdData.MaterialCategoryNum = 0;
                    break;
                case Snow_data_WeaponChangeUserData_Param weaponChangeData:
                    weaponChangeData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.None;
                    weaponChangeData.MaterialCategoryNum = 0;
                    break;
                case Snow_data_WeaponProcessUserData_Param weaponProcessData:
                    weaponProcessData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.None;
                    weaponProcessData.MaterialCategoryNum = 0;
                    break;
                case Snow_equip_PlOverwearProductUserData_Param layeredProdData:
                    // Settings the category to 'none' here breaks the game.
                    // If the player hasn't seen an item in the category the game breaks.
                    if (layeredProdData.MaterialCategory >= Snow_data_NormalItemData_MaterialCategory.Category_000) {
                        layeredProdData.MaterialCategory = Snow_data_NormalItemData_MaterialCategory.Category_000;
                    }
                    layeredProdData.MaterialCategoryNum = 0;
                    break;
                case Snow_data_HyakuryuDecoProductUserData_Param rampageDecoProdData:
                    if (rampageDecoProdData.Category >= Snow_data_NormalItemData_MaterialCategory.Category_000) {
                        rampageDecoProdData.Category = Snow_data_NormalItemData_MaterialCategory.Category_000;
                    }
                    rampageDecoProdData.Point = 0;
                    break;
                case Snow_data_CustomBuildupOpenUserData_Param augmentEnableData:
                    augmentEnableData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.Category_000;
                    augmentEnableData.MaterialCategoryNum = 0;
                    augmentEnableData.Price               = 0;
                    break;
                case Snow_data_CustomBuildupArmorMaterialUserData_Param augmentData:
                    augmentData.MaterialCategory          = Snow_data_NormalItemData_MaterialCategory.Category_000;
                    augmentData.MaterialCategoryNum       = 0;
                    augmentData.MaterialCategoryNum_Def   = 0;
                    augmentData.MaterialCategoryNum_Skill = 0;
                    augmentData.MaterialCategoryNum_Slot  = 0;
                    augmentData.Price                     = 0;
                    break;
                case Snow_data_CustomBuildupWeaponMaterialUserData_Param augmentData:
                    augmentData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.Category_000;
                    augmentData.MaterialCategoryNum = 0;
                    augmentData.Price               = 0;
                    break;
                case Snow_equip_OverwearWeaponProductUserData_Param wpLayeredProdData:
                    if (wpLayeredProdData.MaterialCategory >= Snow_data_NormalItemData_MaterialCategory.Category_000) {
                        wpLayeredProdData.MaterialCategory = Snow_data_NormalItemData_MaterialCategory.Category_000;
                    }
                    wpLayeredProdData.MaterialCategoryNum = 0;
                    wpLayeredProdData.Price               = 0;
                    break;
            }
        }
    }

    public static void NoUnlockFlag(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Snow_data_ArmorProductUserData_Param armorProdData:
                    armorProdData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    armorProdData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    armorProdData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
                case Snow_equip_PlOverwearProductUserData_Param armorProdData:
                    armorProdData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    armorProdData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    armorProdData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
                case Snow_data_DecorationsProductUserData_Param decoProdData:
                    decoProdData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    decoProdData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    decoProdData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
                case Snow_data_HyakuryuDecoProductUserData_Param decoProdData:
                    decoProdData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    decoProdData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    decoProdData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
                case Snow_data_WeaponProductUserData_Param weaponProdData:
                    weaponProdData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    weaponProdData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    weaponProdData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
                case Snow_data_WeaponChangeUserData_Param weaponChangeData:
                    weaponChangeData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    weaponChangeData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    weaponChangeData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
                case Snow_data_WeaponProcessUserData_Param weaponProcessData:
                    weaponProcessData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    weaponProcessData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    weaponProcessData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
                case Snow_equip_OverwearWeaponProductUserData_Param wpLayeredProdData:
                    wpLayeredProdData.ItemFlag     = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    wpLayeredProdData.EnemyFlag    = Snow_enemy_EnemyDef_EmTypes.EmTypeNoData;
                    wpLayeredProdData.ProgressFlag = Snow_data_DataDef_UnlockProgressTypes.None;
                    break;
            }
        }
    }

    public static void Lvl8Shelling(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Snow_equip_GunLanceBaseUserData_Param gLanceData:
                    gLanceData.GunLanceFireLv = Snow_data_GunLanceFireData_GunLanceFireLv.Lv8;
                    break;
            }
        }
    }
}