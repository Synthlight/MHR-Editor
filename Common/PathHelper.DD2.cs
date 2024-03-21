using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Common;

public static class PathHelper {
    public const string CHUNK_PATH       = @"V:\DD2\re_chunk_000";
    public const string ENUM_HEADER_PATH = @"O:\SteamLibrary\steamapps\common\Dragons Dogma 2 Character Creator & Storage\Enums_Internal.hpp";
    public const string STRUCT_JSON_PATH = @"R:\Games\Monster Hunter Rise\RE_RSZ\rszdd2.json";
    public const string MODS_PATH        = @"R:\Games\Dragons Dogma 2\Mods";
    public const string FLUFFY_MODS_PATH = @"R:\Games\Dragons Dogma 2\Fluffy Mod Manager\Games\DD2\Mods";

    public const string NEXUS_URL              = "";
    public const string JSON_VERSION_CHECK_URL = "";
    public const string WIKI_URL               = "";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly string[] TEST_PATHS = [
        @"\natives\STM\AppSystem\"
    ];
}