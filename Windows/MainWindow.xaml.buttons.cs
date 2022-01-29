using System;
using System.Diagnostics;
using System.Windows;

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