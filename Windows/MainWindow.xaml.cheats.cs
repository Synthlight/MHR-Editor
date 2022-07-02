using System.Windows;
using MHR_Editor.Common.Models.Game;
using MHR_Editor.Models.Structs;

namespace MHR_Editor.Windows;

public partial class MainWindow {
    private void Btn_all_cheats_Click(object sender, RoutedEventArgs e) {
        Btn_max_sharpness_Click(sender, e);
        Btn_max_slots_Click(sender, e);
        Btn_max_skills_Click(sender, e);
    }

    private void Btn_max_sharpness_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        foreach (var obj in file.rsz.objectData) {
            switch (obj) {
                case IMeleeWeapon weapon:
                    weapon.SharpnessValList[0].Value = 10;
                    weapon.SharpnessValList[1].Value = 10;
                    weapon.SharpnessValList[2].Value = 10;
                    weapon.SharpnessValList[3].Value = 10;
                    weapon.SharpnessValList[4].Value = 10;
                    weapon.SharpnessValList[5].Value = 10;
                    weapon.SharpnessValList[6].Value = 340;
                    foreach (var handicraft in weapon.TakumiValList) {
                        handicraft.Value = 0;
                    }
                    break;
            }
        }
    }

    private void Btn_max_slots_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        foreach (var obj in file.rsz.objectData) {
            switch (obj) {
                case Snow_data_ArmorBaseUserData_Param armor:
                    armor.DecorationsNumList[0].Value =
                        armor.DecorationsNumList[1].Value =
                            armor.DecorationsNumList[2].Value = 0;
                    armor.DecorationsNumList[3].Value = 3;
                    break;
                case IMeleeWeapon weapon:
                    weapon.SlotNumList[0].Value =
                        weapon.SlotNumList[1].Value =
                            weapon.SlotNumList[2].Value = 0;
                    weapon.SlotNumList[3].Value = 3;
                    weapon.HyakuryuSlotNumList[0].Value =
                        weapon.HyakuryuSlotNumList[1].Value = 0;
                    weapon.HyakuryuSlotNumList[2].Value = 1;
                    break;
            }
        }
    }

    private void Btn_max_skills_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        foreach (var obj in file.rsz.objectData) {
            switch (obj) {
                case Snow_data_ArmorBaseUserData_Param armor:
                    foreach (var level in armor.SkillLvList) {
                        if (level.Value > 0) level.Value = 10;
                    }
                    break;
                case Snow_data_DecorationsBaseUserData_Param decoration:
                    if (decoration.SkillLvList[0].Value > 0) decoration.SkillLvList[0].Value = 10;
                    break;
            }
        }
    }
}