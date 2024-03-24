using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Common;

public static class PathHelper {
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
        //@"\natives\STM\AppSystem\",
        @"\natives\STM\AppSystem\catalog\maincontents\userdata\",
        @"\natives\STM\AppSystem\item\",
        @"\natives\STM\AppSystem\userdata\",
        @"\natives\stm\appsystem\ch\common\userdata\",
        @"\natives\stm\appsystem\ch\common\human\userdata\",
    ];

    public const string ARMOR_DATA_PATH      = "natives/STM/AppSystem/Item/ItemData/ItemArmorData.user.2";
    public const string ITEM_DATA_PATH       = "natives/STM/AppSystem/Item/ItemData/ItemData.user.2";
    public const string ITEM_PARAMETERS_PATH = "natives/STM/AppSystem/Item/ItemParameters/ItemParameters.user.2";
    public const string ITEM_SHOP_DATA_PATH  = "natives/STM/AppSystem/Item/ItemShopData/ItemShopData.user.2";
    public const string STAMINA_PARAM_PATH   = "natives/STM/AppSystem/ch/Common/Human/UserData/Parameter/StaminaParameter.user.2";
    public const string WEAPON_DATA_PATH     = "natives/STM/AppSystem/Item/ItemData/ItemWeaponData.user.2";
}