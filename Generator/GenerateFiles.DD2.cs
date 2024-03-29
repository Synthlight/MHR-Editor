using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Generator;

public partial class GenerateFiles {
    public const string ROOT_STRUCT_NAMESPACE = "app";
    public const string CONFIG_NAME           = "DD2";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    private static readonly List<string> WHITELIST = [
        "App_ArmorEnhanceParam",
        "App_DropPartsUserData",
        "App_GameSystemUserData",
        "App_HumanSpeedParameter",
        "App_HumanStaminaParameter",
        "App_HumanStaminaParameterAdditional",
        "App_HumanStaminaParameterAdditionalData",
        "App_ItemArmorData",
        "App_ItemData",
        "App_ItemParameters",
        "App_ItemShopData",
        "App_ItemWeaponData",
        "App_Job03Parameter",
        "App_Job04Parameter",
        "App_Job07Parameter",
        "App_WeaponCatalogData",
        "App_WeaponEnhanceParam",
        "App_Job04Parameter_CuttingWindParameter_LevelParameter",
    ];
}