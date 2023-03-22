using System.Windows;
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
    }

    private void Btn_wiki_dump_Click(object sender, RoutedEventArgs e) {
        WikiDump.DumpAll();
    }

    private void Btn_make_all_mods_Click(object sender, RoutedEventArgs e) {
        foreach (var make in allMakeModMethods) {
            make.Invoke(null, null);
        }
    }
}