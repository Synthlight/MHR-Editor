﻿namespace RE_Editor.Common;

public static partial class PathHelper {
    public const string CONFIG_NAME      = "RE2";
    public const string CHUNK_PATH       = @"V:\RE2\re_chunk_000";
    public const string GAME_PATH        = @"O:\SteamLibrary\steamapps\common\RESIDENT EVIL 2  BIOHAZARD RE2";
    public const string EXE_PATH         = $@"{GAME_PATH}\re2_dump.exe";
    public const string IL2CPP_DUMP_PATH = $@"{GAME_PATH}\il2cpp_dump.json";
    public const string ENUM_HEADER_PATH = $@"{GAME_PATH}\Enums_Internal.hpp";
    public const string REFRAMEWORK_PATH = @"R:\Games\Monster Hunter Rise\REFramework";
    public const string MODS_PATH        = @"R:\Games\Resident Evil 2 Remake\Mods\_RTX";
    public const string FLUFFY_MODS_PATH = @"R:\Games\Resident Evil 2 Remake\Fluffy Manager 5000\Games\RE2R\Mods";
    public const string PYTHON38_PATH    = @"C:\Program Files\Python38\python.exe";

    public const string NEXUS_URL              = "";
    public const string JSON_VERSION_CHECK_URL = "";
    public const string WIKI_URL               = "";

    public static readonly string[] TEST_PATHS = [
        @"\natives\STM\"
    ];

    public const string WEAPON_BULLET_USER_DATA_PATH = "/natives/STM/SectionRoot/UserData/System/Inventory/WeaponBulletUserData.user.2";
}