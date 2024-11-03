using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Generator;

public partial class GenerateFiles {
    public const string ROOT_STRUCT_NAMESPACE = "app";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    private static readonly List<string> WHITELIST = [
        "App_HunterDef_Skill_Fixed",
        "App_user_data_ArmorData",
        "App_user_data_ArmorData_cData",
        "App_user_data_ArmorSeriesData",
        "App_user_data_ArmorSeriesData_cData",
        "App_user_data_ItemData",
        "App_user_data_ItemData_cData",
        "App_user_data_cItemRecipe",
        "App_user_data_cItemRecipe_cData",
        "App_user_data_WeaponData",
        "App_user_data_WeaponData_cData",
        "App_ArmorDef_ARMOR_COLOR_TYPE_Serializable",
        "App_ArmorDef_ARMOR_PARTS",
        "App_ArmorDef_MODEL_VARIETY_Serializable",
        "App_ArmorDef_SERIES_Fixed",
        "App_WeaponDef_SERIES_Fixed",
    ];
}