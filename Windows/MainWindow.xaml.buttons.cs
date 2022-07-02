using System;
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

        var armor = ReDataFile.Read(inPath + @"\natives\STM\data\Define\Player\Armor\ArmorBaseData.user.2")
                              .rsz.objectData.OfType<Snow_data_ArmorBaseUserData_Param>();
        var writer = new StringWriter();
        foreach (var a in armor) {
            if ((Snow_data_ContentsIdSystem_SubCategoryType) a.GetSubcategoryType() == Snow_data_ContentsIdSystem_SubCategoryType.C_Unclassified) continue;
            writer.WriteLine($"| {a.Id:X8}" +
                             $" | {DataHelper.ARMOR_NAME_LOOKUP[Global.locale].TryGet(a.Id, "")}" +
                             $" | {a.GetSubcategoryId()}" +
                             $" | {a.GetSubcategoryName()}" +
                             $" | {(uint) a.ModelId}" +
                             $" | {a.ModelId}" +
                             $" | {(uint) a.Series}" +
                             $" | {DataHelper.ARMOR_SERIES_LOOKUP[Global.LangIndex.eng].TryGet((uint) a.Series, "")}" +
                             $" | 0x{(uint) a.IdAfterExChange:X8}" +
                             $" | {a.SortId}" +
                             $" | {a.IsValid} |");
        }
        File.WriteAllText(outPath + @"\Armor.md", writer.ToString());
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