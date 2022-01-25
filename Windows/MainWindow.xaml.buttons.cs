using System;
using System.Diagnostics;
using System.Windows;
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

    private void Btn_cheat_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        foreach (var item in file.rsz.objectData) {
            switch (item) {
                case Armor armor:
                    armor.DecorationsNumList[0].Value = armor.DecorationsNumList[1].Value = 0;
                    armor.DecorationsNumList[2].Value = 3;

                    switch (armor.Id) {
                        case 0xC100008: // Dober
                            armor.SkillIdList[0].Value = 88; // Item Prolonger
                            armor.SkillIdList[1].Value = 96; // Hunger Resistance
                            armor.SkillIdList[2].Value = 82; // Capture Master
                            armor.SkillIdList[3].Value = 83; // Carving Master
                            armor.SkillIdList[4].Value = 61; // Earplugs
                            ArmorHack(armor);
                            break;
                        case 0xC200008: // Dober
                            armor.SkillIdList[0].Value = 1; // Attack Boost
                            armor.SkillIdList[1].Value = 62; // Windproof
                            armor.SkillIdList[2].Value = 65; // Evade Window
                            armor.SkillIdList[3].Value = 84; // Good Luck
                            armor.SkillIdList[4].Value = 26; // Mind's Eye
                            ArmorHack(armor);
                            break;
                        case 0xC300008: // Dober
                            armor.SkillIdList[0].Value = 40; // Quick Sheathe
                            armor.SkillIdList[1].Value = 6; // Critical Eye
                            armor.SkillIdList[2].Value = 105; // Wall Runner
                            armor.SkillIdList[3].Value = 8; // Weakness Exploit
                            armor.SkillIdList[4].Value = 38; // Critical Draw
                            ArmorHack(armor);
                            break;
                        case 0xC400008: // Dober
                            armor.SkillIdList[0].Value = 76; // Stun Resistance
                            armor.SkillIdList[1].Value = 30; // Focus
                            armor.SkillIdList[2].Value = 32; // Marathon Runner
                            armor.SkillIdList[3].Value = 10; // Maximum Might
                            armor.SkillIdList[4].Value = 36; // Guard Up
                            ArmorHack(armor);
                            break;
                        case 0xC500008: // Dober
                            armor.SkillIdList[0].Value = 41; // Slugger
                            armor.SkillIdList[1].Value = 81; // Partbreaker
                            armor.SkillIdList[2].Value = 60; // Speed Eating
                            armor.SkillIdList[3].Value = 58; // Recovery Up
                            armor.SkillIdList[4].Value = 59; // Recovery Speed
                            ArmorHack(armor);
                            break;


                        case 0xC100003: // Leather S
                            armor.SkillIdList[0].Value = 80; // Geologist
                            armor.SkillIdList[1].Value = 79; // Botanist
                            armor.SkillIdList[2].Value = 105; // Wall Runner
                            armor.SkillIdList[3].Value = 32; // Marathon Runner
                            armor.SkillIdList[4].Value = 95; // Carving Pro
                            ArmorHack(armor);
                            break;
                        case 0xC200003: // Leather S
                            armor.SkillIdList[0].Value = 83; // Carving Master
                            armor.SkillIdList[1].Value = 96; // Hunger Resistance
                            armor.SkillIdList[2].Value = 104; // Wirebug Whisperer
                            armor.SkillIdList[3].Value = 60; // Speed Eating
                            armor.SkillIdList[4].Value = 58; // Recovery Up
                            ArmorHack(armor);
                            break;
                        case 0xC300003: // Leather S
                            armor.SkillIdList[0].Value = 59; // Recovery Speed
                            armor.SkillIdList[1].Value = 33; // Constitution
                            armor.SkillIdList[2].Value = 34; // Stamina Surge
                            armor.SkillIdList[3].Value = 40; // Quick Sheathe
                            armor.SkillIdList[4].Value = 61; // Earplugs
                            ArmorHack(armor);
                            break;
                        case 0xC400003: // Leather S
                            armor.SkillIdList[0].Value = 76; // Stun Resistance
                            armor.SkillIdList[1].Value = 30; // Focus
                            armor.SkillIdList[2].Value = 32; // Marathon Runner
                            armor.SkillIdList[3].Value = 10; // Maximum Might
                            armor.SkillIdList[4].Value = 36; // Guard Up
                            ArmorHack(armor);
                            break;
                        case 0xC500003: // Leather S
                            armor.SkillIdList[0].Value = 84; // Good Luck
                            armor.SkillIdList[1].Value = 1; // Attack Boost
                            armor.SkillIdList[2].Value = 6; // Critical Eye
                            armor.SkillIdList[3].Value = 62; // Windproof
                            armor.SkillIdList[4].Value = 88; // Item Prolonger
                            ArmorHack(armor);
                            break;
                    }


                    for (var i = 0; i < armor.SkillLevelList.Count; i++) {
                        var id    = armor.SkillIdList[i];
                        var level = armor.SkillLevelList[i];

                        if (id.Value > 0 || level.Value > 0) level.Value = 10;
                    }

                    break;
                case Decoration decoration:
                    if (decoration.SkillLevelList[0].Value > 0) decoration.SkillLevelList[0].Value = 10;
                    break;
                case GreatSword greatSword:
                    greatSword.DecorationsNumList[0].Value = greatSword.DecorationsNumList[1].Value = 0;
                    greatSword.DecorationsNumList[2].Value = 3;
                    greatSword.SharpnessValList[0].Value   = 10;
                    greatSword.SharpnessValList[1].Value   = 10;
                    greatSword.SharpnessValList[2].Value   = 10;
                    greatSword.SharpnessValList[3].Value   = 10;
                    greatSword.SharpnessValList[4].Value   = 10;
                    greatSword.SharpnessValList[5].Value   = 350;
                    greatSword.SharpnessValList[6].Value   = 0;
                    break;
            }
        }
    }

    private void ArmorHack(Armor armor) {
        armor.FireRegVal =
            armor.IceRegVal =
                armor.ThunderRegVal =
                    armor.WaterRegVal =
                        armor.DragonRegVal = 30;
    }

    private void Btn_open_wiki_OnClick(object sender, RoutedEventArgs e) {
        try {
            Process.Start("https://github.com/Synthlight/MHR-Editor/wiki");
        } catch (Exception err) {
            Console.Error.WriteLine(err);
        }
    }
}