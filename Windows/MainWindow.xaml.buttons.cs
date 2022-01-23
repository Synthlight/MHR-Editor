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

    private void Btn_cheat_Click(object sender, RoutedEventArgs e) {
        /*
        foreach (var item in items) {
            switch (item) {
                case Armor armor:
                    armor.DecorationsNum1 = armor.DecorationsNum2 = 0;
                    armor.DecorationsNum3 = 3;
                    if (armor.Skill1Level > 0) armor.Skill1Level = 10;
                    if (armor.Skill2Level > 0) armor.Skill2Level = 10;
                    if (armor.Skill3Level > 0) armor.Skill3Level = 10;
                    if (armor.Skill4Level > 0) armor.Skill4Level = 10;
                    if (armor.Skill5Level > 0) armor.Skill5Level = 10;
                    break;
                case Decoration decoration:
                    if (decoration.Skill1Level > 0) decoration.Skill1Level = 10;
                    break;
                case GreatSword greatSword:
                    greatSword.DecorationsNum1 = greatSword.DecorationsNum2 = 0;
                    greatSword.DecorationsNum3 = 3;
                    greatSword.SharpnessVal1   = 10;
                    greatSword.SharpnessVal2   = 10;
                    greatSword.SharpnessVal3   = 10;
                    greatSword.SharpnessVal4   = 10;
                    greatSword.SharpnessVal5   = 10;
                    greatSword.SharpnessVal6   = 350;
                    greatSword.SharpnessVal7   = 0;
                    break;
            }
        }
        */
    }

    private void Btn_open_wiki_OnClick(object sender, RoutedEventArgs e) {
        try {
            Process.Start("https://github.com/Synthlight/MHR-Editor/wiki");
        } catch (Exception err) {
            Console.Error.WriteLine(err);
        }
    }
}