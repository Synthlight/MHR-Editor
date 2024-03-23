﻿using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Generator;

public partial class GenerateFiles {
    public const string ROOT_STRUCT_NAMESPACE = "snow";
    public const string CONFIG_NAME           = "MHR";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    private static readonly List<string> WHITELIST = [
        "Snow_data_ArmorBaseUserData",
        "Snow_data_ContentsIdSystem_ItemId",
        "Snow_data_ContentsIdSystem_SubCategoryType",
        "Snow_data_DangoBaseUserData",
        "Snow_data_DataDef_PlEquipSkillId",
        "Snow_data_DecorationsBaseUserData",
        "Snow_data_ItemUserData",
        "Snow_data_NormalLvBuffCageBaseUserData",
        "Snow_data_OtAirouArmorBaseUserData",
        "Snow_data_OtDogArmorBaseUserData",
        "Snow_data_OtDogWeaponBaseUserData",
        "Snow_data_OtWeaponBaseUserData",
        "Snow_data_PlEquipSkillBaseUserData",
        "Snow_data_PlHyakuryuSkillBaseUserData",
        "Snow_enemy_em134_Em134_00UniqueData", // Nested generics.
        "Snow_envCreature_Ec019Trajectory_TimeEffectSetting", // Nested generics.
        "Snow_equip_BowBaseUserData",
        "Snow_equip_ChargeAxeBaseUserData",
        "Snow_equip_DualBladesBaseUserData",
        "Snow_equip_GreatSwordBaseUserData",
        "Snow_equip_GunLanceBaseUserData",
        "Snow_equip_HammerBaseUserData",
        "Snow_equip_HeavyBowgunBaseUserData",
        "Snow_equip_HornBaseUserData",
        "Snow_equip_InsectBaseUserData",
        "Snow_equip_InsectGlaiveBaseUserData",
        "Snow_equip_LanceBaseUserData",
        "Snow_equip_LightBowgunBaseUserData",
        "Snow_equip_LongSwordBaseUserData",
        "Snow_equip_OtOverwearBaseUserData_Param",
        "Snow_equip_OtOverwearRecipeUserData_Param",
        "Snow_equip_PlOverwearBaseUserData",
        "Snow_equip_ShortSwordBaseUserData",
        "Snow_equip_SlashAxeBaseUserData",
        "Snow_fallingObject_FallingObjectPlayerHeavyBowgunExtraCartridgeUserData",
        "Snow_npc_fsm_action_NpcFsmAction_StopNavigation", // Via_vec3? This one skipped the via type checking.
        "Snow_player_PlayerUserDataBow",
        "Snow_player_PlayerUserDataIG_Insect"
    ];
}