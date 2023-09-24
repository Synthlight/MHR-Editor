using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Common;

public static class PathHelper {
    public const string CHUNK_PATH       = @"V:\RE4\re_chunk_000";
    public const string ENUM_HEADER_PATH = @"O:\SteamLibrary\steamapps\common\RESIDENT EVIL 4  BIOHAZARD RE4\Enums_Internal.hpp";
    public const string STRUCT_JSON_PATH = @"R:\Games\Monster Hunter Rise\RE_RSZ\rszre4.json";
    public const string MODS_PATH        = @"R:\Games\Resident Evil 4 Remake\Mods";
    public const string FLUFFY_MODS_PATH = @"R:\Games\Resident Evil 4 Remake\Fluffy Mod Manager\Games\RE4R\Mods";

    public const string NEXUS_URL              = "";
    public const string JSON_VERSION_CHECK_URL = "";
    public const string WIKI_URL               = "";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly string[] TEST_PATHS = {
        @"\natives\STM\",
    };

    public const string ITEM_DRAG_CRAFT_DATA_PATH       = @"\natives\STM\_Chainsaw\AppSystem\UI\UserData\ItemCombineDefinitionUserData.user.2";
    public const string ITEM_DATA_PATH                  = @"\natives\STM\_Chainsaw\AppSystem\UI\UserData\ItemDefinitionUserData.user.2";
    public const string ITEM_DATA_PATH_AO               = @"\natives\STM\_AnotherOrder\AppSystem\UI\Userdata\ItemDefinitionUserData_AO.user.2";
    public const string ITEM_DATA_PATH_AO_OVR           = @"\natives\STM\_AnotherOrder\AppSystem\UI\Userdata\ItemDefinitionUserData_OVR_AO.user.2";
    public const string ITEM_DATA_PATH_MC               = @"\natives\STM\_Mercenaries\AppSystem\UI\Userdata\ItemDefinitionUserData_MC.user.2";
    public const string ITEM_DATA_PATH_MC_OVR           = @"\natives\STM\_Mercenaries\AppSystem\UI\Userdata\ItemDefinitionUserData_OVR_MC.user.2";
    public const string NEW_GAME_INVENTORY_DATA_PATH    = @"\natives\STM\_Chainsaw\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_Main.user.2";
    public const string NEW_GAME_INVENTORY_AO_DATA_PATH = @"\natives\STM\_AnotherOrder\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_AO.user.2";
    public const string HARPOON_DATA_PATH               = @"\natives\STM\_Chainsaw\AppSystem\Character\ch1f1z0\UserData\Harpoon\ch1f1z0ShellHarpoonShellInfo.user.2";
    public const string WEAPON_UPGRADE_DATA_PATH        = @"\natives\STM\_Chainsaw\AppSystem\WeaponCustom\WeaponDetailCustomUserdata.user.2";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly List<string> NEW_GAME_INVENTORY_MC_DATA_PATHS = new() {
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-1.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-1-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-3.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-4.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-5.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-5-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_1-6.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-1.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-1-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-3.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-4.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-5.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-5-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_2-6.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-1.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-1-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-3.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-4.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-5.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-5-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_3-6.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-1.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-1-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-3.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-4.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-5.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-5-2.user.2",
        @"\natives\STM\_Mercenaries\AppSystem\Inventory\InventoryCatalog\InventoryCatalog_MC_4-6.user.2",
    };
}