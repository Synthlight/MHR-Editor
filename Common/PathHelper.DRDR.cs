﻿namespace RE_Editor.Common;

public static partial class PathHelper {
    public const string CONFIG_NAME      = "DRDR";
    public const string CHUNK_PATH       = @"V:\DRDR\re_chunk_000";
    public const string GAME_PATH        = @"V:\DRDR";
    public const string EXE_PATH         = $@"{GAME_PATH}\DRDR_dump.exe";
    public const string IL2CPP_DUMP_PATH = $@"{GAME_PATH}\il2cpp_dump.json";
    public const string ENUM_HEADER_PATH = $@"{GAME_PATH}\Enums_Internal.hpp";
    public const string REFRAMEWORK_PATH = @"R:\Games\Monster Hunter Rise\REFramework";
    public const string MODS_PATH        = @"R:\Games\Dead Rising Deluxe Remaster\Mods";
    public const string FLUFFY_MODS_PATH = @"R:\Games\Dead Rising Deluxe Remaster\Fluffy Mod Manager\Games\DRDR\Mods";
    public const string PYTHON38_PATH    = @"C:\Program Files\Python38\python.exe";

    public const string NEXUS_URL              = "https://www.nexusmods.com/deadrisingdeluxeremaster/mods/89";
    public const string JSON_VERSION_CHECK_URL = $"http://brutsches.com/{CONFIG_NAME}-Editor.version.json";
    public const string WIKI_URL               = "https://www.nexusmods.com/deadrisingdeluxeremaster/mods/89";

    public static readonly string[] TEST_PATHS = [
        @"\natives\STM\",
    ];
}