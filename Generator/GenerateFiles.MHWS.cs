using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Generator;

public partial class GenerateFiles {
    public const string ROOT_STRUCT_NAMESPACE = "app";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    private static readonly List<string> WHITELIST = [
        "App_user_data_ItemData_cData",
        "App_user_data_ItemData",
        "App_user_data_ArmorData_cData",
        "App_user_data_ArmorData",
        "App_user_data_WeaponData_cData",
        "App_user_data_WeaponData",
    ];
}