using System.Diagnostics.CodeAnalysis;

namespace MHR_Editor.Common;

public static class PathHelper {
    public const string CHUNK_PATH       = @"V:\MHR\re_chunk_000";
    public const string ENUM_HEADER_PATH = @"C:\SteamLibrary\common\MonsterHunterRise\Enums_Internal.hpp";
    public const string STRUCT_JSON_PATH = @"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json";
    public const string WIKI_PATH        = @"R:\Games\Monster Hunter Rise\Wiki Dump";
    public const string MODS_PATH        = @"R:\Games\Monster Hunter Rise\Mods";
    public const string RAR_SCRIPT       = @"/mnt/r/Games/Monster\ Hunter\ Rise/MHR-Editor/compress-rar.sh";


    public const string ARMOR_BASE_PATH                   = @"\natives\STM\data\Define\Player\Armor\ArmorBaseData.user.2";
    public const string ARMOR_RECIPE_PATH                 = @"\natives\STM\data\Define\Player\Armor\ArmorProductData.user.2";
    public const string LAYERED_ARMOR_BASE_PATH           = @"\natives\STM\data\Define\Player\Armor\PlOverwearBaseData.user.2";
    public const string LAYERED_ARMOR_RECIPE_PATH         = @"\natives\STM\data\Define\Player\Armor\PlOverwearProductUserData.user.2";
    public const string DECORATION_PATH                   = @"\natives\STM\data\Define\Player\Equip\Decorations\DecorationsBaseData.user.2";
    public const string DECORATION_RECIPE_PATH            = @"\natives\STM\data\Define\Player\Equip\Decorations\DecorationsProductData.user.2";
    public const string RAMPAGE_DECORATION_PATH           = @"\natives\STM\data\Define\Player\Equip\HyakuryuDeco\HyakuryuDecoBaseData.user.2";
    public const string RAMPAGE_DECORATION_RECIPE_PATH    = @"\natives\STM\data\Define\Player\Equip\HyakuryuDeco\HyakuryuDecoProductData.user.2";
    public const string PLAYER_SKILL_PATH                 = @"\natives\STM\data\Define\Player\Skill\PlEquipSkill\PlEquipSkillBaseData.user.2";
    public const string CAT_ARMOR_RECIPE_PATH             = @"\natives\STM\data\Define\Otomo\Equip\Armor\OtAirouArmorProductData.user.2";
    public const string DOG_ARMOR_RECIPE_PATH             = @"\natives\STM\data\Define\Otomo\Equip\Armor\OtDogArmorProductData.user.2";
    public const string CAT_DOG_LAYERED_ARMOR_RECIPE_PATH = @"\natives\STM\data\Define\Otomo\Equip\Overwear\OtOverwearRecipeData.user.2";
    public const string CAT_WEAPON_RECIPE_PATH            = @"\natives\STM\data\Define\Otomo\Equip\Weapon\OtAirouWeaponProductData.user.2";
    public const string DOG_WEAPON_RECIPE_PATH            = @"\natives\STM\data\Define\Otomo\Equip\Weapon\OtDogWeaponProductData.user.2";
    public const string ITEM_DATA_PATH                    = @"\natives\STM\data\System\ContentsIdSystem\Item\Normal\ItemData.user.2";
    public const string PETALACE_DATA_PATH                = @"\natives\STM\data\System\ContentsIdSystem\LvBuffCage\Normal\NormalLvBuffCageBaseData.user.2";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static IEnumerable<string> GetAllWeaponFilePaths(WeaponDataType type) {
        return Global.WEAPON_TYPES.Select(s => @$"\natives\STM\data\Define\Player\Weapon\{s}\{s}{type}Data.user.2");
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum WeaponDataType {
        Base,
        Change,
        Process,
        Product,
        UpdateTree
    }
}