using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Common;

public static partial class PathHelper {
    public const string CHUNK_PATH       = @"V:\DD2\re_chunk_000";
    public const string ENUM_HEADER_PATH = @"O:\SteamLibrary\steamapps\common\Dragons Dogma 2\Enums_Internal.hpp";
    public const string STRUCT_JSON_PATH = @"R:\Games\Monster Hunter Rise\RE_RSZ\rszdd2.json";
    public const string MODS_PATH        = @"R:\Games\Dragons Dogma 2\Mods";
    public const string FLUFFY_MODS_PATH = @"R:\Games\Dragons Dogma 2\Fluffy Mod Manager\Games\DragonsDogma2\Mods";

    public const string NEXUS_URL              = "";
    public const string JSON_VERSION_CHECK_URL = "";
    public const string WIKI_URL               = "";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly string[] TEST_PATHS = [
        @"\natives\STM\",
    ];

    public const string ARMOR_DATA_PATH                  = "natives/STM/AppSystem/Item/ItemData/ItemArmorData.user.2";
    public const string ARMOR_UPGRADE_DATA_PATH          = "natives/STM/AppSystem/Item/ItemData/ArmorEnhanceData.user.2";
    public const string ITEM_DATA_PATH                   = "natives/STM/AppSystem/Item/ItemData/ItemData.user.2";
    public const string ITEM_PARAMETERS_PATH             = "natives/STM/AppSystem/Item/ItemParameters/ItemParameters.user.2";
    public const string ITEM_SHOP_DATA_PATH              = "natives/STM/AppSystem/Item/ItemShopData/ItemShopData.user.2";
    public const string JOB_03_PARAM_PATH                = "natives/STM/AppSystem/ch/Common/Human/UserData/Parameter/Job03Parameter.user.2";
    public const string JOB_06_PARAM_PATH                = "natives/STM/AppSystem/ch/Common/Human/UserData/Parameter/Job06Parameter.user.2";
    public const string JOB_08_PARAM_PATH                = "natives/STM/AppSystem/ch/Common/Human/UserData/Parameter/Job08Parameter.user.2";
    public const string STAMINA_COMMON_ACTION_PARAM_PATH = "natives/STM/AppSystem/ch/Common/Human/UserData/Parameter/CommonActionStaminaParameter.user.2";
    public const string STAMINA_PARAM_PATH               = "natives/STM/AppSystem/ch/Common/Human/UserData/Parameter/StaminaParameter.user.2";
    public const string WEAPON_DATA_PATH                 = "natives/STM/AppSystem/Item/ItemData/ItemWeaponData.user.2";
    public const string WEAPON_UPGRADE_DATA_PATH         = "natives/STM/AppSystem/Item/ItemData/WeaponEnhanceData.user.2";
}