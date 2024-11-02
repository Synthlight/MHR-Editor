using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Common;

public static partial class PathHelper {
    public const string CONFIG_NAME      = "MHWS";
    public const string CHUNK_PATH       = @"V:\MHWS\re_chunk_000";
    public const string GAME_PATH        = @"O:\SteamLibrary\steamapps\common\MonsterHunterWildsBetatest";
    public const string EXE_PATH         = $@"{GAME_PATH}\MonsterHunterWildsBeta_dump.exe";
    public const string IL2CPP_DUMP_PATH = $@"{GAME_PATH}\il2cpp_dump.json";
    public const string ENUM_HEADER_PATH = $@"{GAME_PATH}\Enums_Internal.hpp";
    public const string REFRAMEWORK_PATH = @"R:\Games\Monster Hunter Rise\REFramework";
    public const string MODS_PATH        = @"R:\Games\Monster Hunter Wilds\Mods";
    public const string FLUFFY_MODS_PATH = @"R:\Games\Monster Hunter Wilds\FMM\Games\MHRS\Mods";
    public const string PYTHON38_PATH    = @"C:\Program Files\Python38\python.exe";

    public const string NEXUS_URL              = "";
    public const string JSON_VERSION_CHECK_URL = $"http://brutsches.com/{CONFIG_NAME}-Editor.version.json";
    public const string WIKI_URL               = "https://github.com/Synthlight/MHR-Editor/wiki";

    public static readonly string[] TEST_PATHS = [
        @"\natives\STM"
    ];

    public const string ARMOR_DATA_PATH = @"\natives\STM\GameDesign\Common\Equip\ArmorData.user.3";
    public const string ITEM_DATA_PATH  = @"\natives\STM\GameDesign\Common\Item\itemData.user.3";

    public static IEnumerable<string> GetAllWeaponFilePaths(WeaponDataType type, string platform = "STM") {
        var postfix                                = "";
        if (type == WeaponDataType.Recipe) postfix = "Recipe";

        return Global.WEAPON_TYPES.Select(s => @$"\natives\{platform}\GameDesign\Common\Weapon\{s}{postfix}.user.3");
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum WeaponDataType {
        Base,
        Recipe
    }
}