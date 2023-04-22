using System;
using System.Diagnostics;
using System.Windows;
using RE_Editor.Common;
using RE_Editor.Mods;

namespace RE_Editor.Windows;

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

    private void Btn_test_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;
        if (targetFile.EndsWith("InventoryCatalog_Main.user.2")) OpGameStart.MakeNewInventory(file.rsz.objectData, OpGameStart.Target.WITHOUT_BONUS_WEAPONS);
    }

    private void Btn_make_all_mods_Click(object sender, RoutedEventArgs e) {
        foreach (var make in allMakeModMethods) {
            make.Invoke(null, null);
        }
    }

    private void Btn_open_wiki_OnClick(object sender, RoutedEventArgs e) {
        try {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {PathHelper.WIKI_URL}") {CreateNoWindow = true});
        } catch (Exception err) {
            Console.Error.WriteLine(err);
        }
    }
}