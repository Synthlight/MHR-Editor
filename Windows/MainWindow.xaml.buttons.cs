using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using MHR_Editor.Common;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models;
using MHR_Editor.Models.Enums;
using MHR_Editor.Models.Structs;

namespace MHR_Editor.Windows;

public partial class MainWindow {
    private void Btn_open_Click(object sender, RoutedEventArgs e) {
        Load();
    }

    private void Btn_save_Click(object sender, RoutedEventArgs e) {
        Save();
    }

    private void Btn_save_as_Click(object sender, RoutedEventArgs e) {
        Save(true);
    }

    private void Btn_dump_armor_Click(object sender, RoutedEventArgs e) {
        const string inPath  = @"V:\MHR\re_chunk_000";
        const string outPath = @"R:\Games\Monster Hunter Rise\Wiki Dump";

        DumpArmorForWiki(inPath, outPath);
        DumpItemsForWiki(inPath, outPath);
        DumpArmorIdToLayeredIdForLayeredProgressionMod(inPath, outPath);
    }

    private static void DumpArmorForWiki(string inPath, string outPath) {
        var armor = ReDataFile.Read(inPath + @"\natives\STM\data\Define\Player\Armor\ArmorBaseData.user.2")
                              .rsz.objectData.OfType<Snow_data_ArmorBaseUserData_Param>();
        using var writer = new StreamWriter(File.Create(outPath + @"\Armor.md"));
        foreach (var a in armor) {
            if ((Snow_data_ContentsIdSystem_SubCategoryType) a.GetArmorType() == Snow_data_ContentsIdSystem_SubCategoryType.C_Unclassified) continue;
            writer.WriteLine($"| {a.Id:X8}" +
                             $" | {DataHelper.ARMOR_NAME_LOOKUP[Global.locale].TryGet(a.Id, "")}" +
                             $" | {a.GetArmorId()}" +
                             $" | {a.GetArmorTypeName()}" +
                             $" | {(uint) a.ModelId}" +
                             $" | {a.ModelId}" +
                             $" | {(uint) a.Series}" +
                             $" | {DataHelper.ARMOR_SERIES_LOOKUP[Global.LangIndex.eng].TryGet((uint) a.Series, "")}" +
                             $" | 0x{(uint) a.IdAfterExChange:X8}" +
                             $" | {a.SortId}" +
                             $" | {a.IsValid} |");
        }
    }

    private static void DumpItemsForWiki(string inPath, string outPath) {
        var item = ReDataFile.Read(inPath + @"\natives\STM\data\System\ContentsIdSystem\Item\Normal\ItemData.user.2")
                             .rsz.objectData.OfType<Snow_data_ItemUserData_Param>();
        using var writer = new StreamWriter(File.Create(outPath + @"\Items.md"));
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

    private static void DumpArmorIdToLayeredIdForLayeredProgressionMod(string inPath, string outPath) {
        var armor = ReDataFile.Read(inPath + @"\natives\STM\data\Define\Player\Armor\ArmorBaseData.user.2")
                              .rsz.objectData.OfType<Snow_data_ArmorBaseUserData_Param>();
        var layeredArmor = ReDataFile.Read(inPath + @"\natives\STM\data\Define\Player\Armor\PlOverwearBaseData.user.2")
                                     .rsz.objectData.OfType<Snow_equip_PlOverwearBaseUserData_Param>();
        var writer                = new StreamWriter(File.Create(outPath + @"\Layered Armor.lua"));
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

    private void Btn_test_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;
    }

    private void Btn_open_wiki_OnClick(object sender, RoutedEventArgs e) {
        try {
            Process.Start(new ProcessStartInfo("cmd", "/c start https://github.com/Synthlight/MHR-Editor/wiki") {CreateNoWindow = true});
        } catch (Exception err) {
            Console.Error.WriteLine(err);
        }
    }
}