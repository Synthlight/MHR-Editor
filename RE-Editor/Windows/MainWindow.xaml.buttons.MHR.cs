using System.Windows;
using RE_Editor.Common;
using RE_Editor.Models;
using RE_Editor.Mods;

namespace RE_Editor.Windows;

public partial class MainWindow {
    private void Btn_wiki_dump_Click(object sender, RoutedEventArgs e) {
        WikiDump.DumpAll();
    }

    private void Btn_all_cheats_Click(object sender, RoutedEventArgs e) {
        Btn_max_sharpness_Click(sender, e);
        Btn_max_slots_Click(sender, e);
        Btn_max_skills_Click(sender, e);
        Btn_no_cost_Click(sender, e);
    }

    private void Btn_max_sharpness_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        CheatMod.MaxSharpness(file.rsz.objectData);
    }

    private void Btn_max_slots_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        CheatMod.MaxSlots(file.rsz.objectData);
    }

    private void Btn_max_skills_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        CheatMod.MaxSkills(file.rsz.objectData);
    }

    private void Btn_no_cost_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        CheatMod.NoCost(file.rsz.objectData);
    }

    private void Btn_no_unlock_flag_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        CheatMod.MaxSkills(file.rsz.objectData);
    }

    private void Btn_sort_gems_by_gem_name_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        SortedGems.SortGems(file.rsz.objectData, GemSortType.GEM_NAME, Global.locale);
    }

    private void Btn_sort_gems_by_skill_name_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        SortedGems.SortGems(file.rsz.objectData, GemSortType.SKILL_NAME, Global.locale);
    }
}