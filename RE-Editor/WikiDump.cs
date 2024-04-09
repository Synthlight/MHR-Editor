using System.Collections.Generic;
using System.IO;
using System.Linq;
using RE_Editor.Models.Enums;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using RE_Editor.Common.Models;
using RE_Editor.Generated.Models;
using RE_Editor.Models.Structs;

namespace RE_Editor;

public static class WikiDump {
    public static void DumpAll() {
        DumpArmor();
        DumpItems();
        DumpSkills();
        DumpWeapons();
        DumpArmorIdToLayeredIdForLayeredProgressionMod();
    }

    public static void DumpArmor() {
        var armor = ReDataFile.Read(PathHelper.CHUNK_PATH + PathHelper.ARMOR_BASE_PATH)
                              .rsz.objectData.OfType<Snow_data_ArmorBaseUserData_Param>();
        using var writer = new StreamWriter(File.Create(PathHelper.WIKI_PATH + @"\Armor.md"));
        foreach (var a in armor) {
            if ((Snow_data_ContentsIdSystem_SubCategoryType) a.GetArmorType() == Snow_data_ContentsIdSystem_SubCategoryType.C_Unclassified) continue;
            writer.WriteLine($"| {a.Id:X8}" +
                             $" | {DataHelper.ARMOR_NAME_LOOKUP[Global.locale].TryGet((uint) a.Id, "")}" +
                             $" | {a.GetArmorId()}" +
                             $" | {a.GetArmorTypeEnum()}" +
                             $" | {(uint) a.ModelId}" +
                             $" | {a.ModelId}" +
                             $" | {(uint) a.Series}" +
                             $" | {DataHelper.ARMOR_SERIES_LOOKUP[Global.locale].TryGet((uint) a.Series, "")}" +
                             $" | 0x{(uint) a.IdAfterExChange:X8}" +
                             $" | {a.SortId}" +
                             $" | {a.IsValid} |");
        }
    }

    public static void DumpItems() {
        var item = ReDataFile.Read(PathHelper.CHUNK_PATH + PathHelper.ITEM_DATA_PATH)
                             .rsz.objectData.OfType<Snow_data_ItemUserData_Param>();
        using var writer = new StreamWriter(File.Create(PathHelper.WIKI_PATH + @"\Items.md"));
        foreach (var a in item) {
            if ((Snow_data_ContentsIdSystem_SubCategoryType) a.GetItemType() == Snow_data_ContentsIdSystem_SubCategoryType.C_Unclassified) continue;
            var dict = DataHelper.ITEM_NAME_LOOKUP[Global.locale];
            if (!dict.ContainsKey(a.Id) || dict[a.Id].Trim() == "") continue;
            writer.WriteLine($"| {a.Id}" +
                             $" | {a.Id:X8}" +
                             $" | {a.GetItemEnumId()}" +
                             $" | {a.Id:X8}" +
                             $" | {dict[a.Id]}" +
                             $" | {a.Type}" +
                             $" | {a.ItemGroup} |");
        }
    }

    public static void DumpSkills() {
        var item = ReDataFile.Read(PathHelper.CHUNK_PATH + PathHelper.PLAYER_SKILL_PATH)
                             .rsz.objectData.OfType<Snow_data_PlEquipSkillBaseUserData_Param>();
        using var writer = new StreamWriter(File.Create(PathHelper.WIKI_PATH + @"\Skills.md"));
        foreach (var a in item) {
            if (a.Id <= 0) continue;
            writer.WriteLine($"| {a.Id}" +
                             $" | {a.EnumName}" +
                             $" | {a.Name} |");
        }
    }

    public static void DumpWeapons() {
        foreach (var type in Global.WEAPON_TYPES) {
            var weapon = ReDataFile.Read(PathHelper.CHUNK_PATH + $@"\natives\STM\data\Define\Player\Weapon\{type}\{type}BaseData.user.2")
                                   .rsz.objectData.OfType<IWeapon>();
            using var writer = new StreamWriter(File.Create(PathHelper.WIKI_PATH + $@"\{type}.md"));
            foreach (var a in weapon.OrderBy(w => w.Id)) {
                if (a.ModelId == Snow_data_ParamEnum_WeaponModelId.None) continue;
                writer.WriteLine($"| {a.Id:X8}" +
                                 $" | {a.GetWeaponEnum()}" +
                                 $" | {(uint) a.ModelId:X8}" +
                                 $" | {a.ModelId}" +
                                 $" | {a.SortId}" +
                                 $" | {a.Name} |");
            }
        }
    }

    public static void DumpArmorIdToLayeredIdForLayeredProgressionMod() {
        var armor = ReDataFile.Read(PathHelper.CHUNK_PATH + PathHelper.ARMOR_BASE_PATH)
                              .rsz.objectData.OfType<Snow_data_ArmorBaseUserData_Param>();
        var layeredArmor = ReDataFile.Read(PathHelper.CHUNK_PATH + PathHelper.LAYERED_ARMOR_BASE_PATH)
                                     .rsz.objectData.OfType<Snow_equip_PlOverwearBaseUserData_Param>();
        var writer                = new StreamWriter(File.Create(PathHelper.WIKI_PATH + @"\Layered Armor.lua"));
        var armorIdToLayeredIdMap = new Dictionary<uint, string>();
        var modelIdToLayeredId    = new Dictionary<Snow_data_DataDef_PlArmorModelId, uint>();
        var modelIdToLayeredName  = new Dictionary<Snow_data_DataDef_PlArmorModelId, string>();
        foreach (var a in layeredArmor) {
            if (!a.IsValid) continue;
            armorIdToLayeredIdMap[a.GetArmorId()] = $"{a.GetLayeredId()} -- {a.Name}";
            modelIdToLayeredId[a.ModelId]         = a.GetLayeredId();
            modelIdToLayeredName[a.ModelId]       = a.Name;
        }
        foreach (var a in armor) {
            if (!a.IsValid) continue;
            if (armorIdToLayeredIdMap.ContainsKey(a.GetArmorId())) continue; // Already mapped, ignore.
            if (modelIdToLayeredId.ContainsKey(a.ModelId)) {
                armorIdToLayeredIdMap[a.GetArmorId()] = $"{modelIdToLayeredId[a.ModelId]} -- {modelIdToLayeredName[a.ModelId]}";
            }
        }
        foreach (var key in armorIdToLayeredIdMap.Keys.OrderBy(u => u)) {
            writer.WriteLine($"armorID_layeredID[{key}] = {armorIdToLayeredIdMap[key]}");
        }
    }
}