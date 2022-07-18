using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Generated.Models;

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

    private void Btn_wiki_dump_Click(object sender, RoutedEventArgs e) {
        WikiDump.DumpAll();
    }

    private void Btn_open_wiki_OnClick(object sender, RoutedEventArgs e) {
        try {
            Process.Start(new ProcessStartInfo("cmd", "/c start https://github.com/Synthlight/MHR-Editor/wiki") {CreateNoWindow = true});
        } catch (Exception err) {
            Console.Error.WriteLine(err);
        }
    }

    private void Btn_create_sorted_gem_mods_Click(object sender, RoutedEventArgs e) {
        const string inPath          = @"V:\MHR\re_chunk_000";
        const string outPath         = @"R:\Games\Monster Hunter Rise\Mods\Sorted Gems";
        const string decoPath        = @"\natives\STM\data\Define\Player\Equip\Decorations\DecorationsBaseData.user.2";
        const string rampageDecoPath = @"\natives\STM\data\Define\Player\Equip\HyakuryuDeco\HyakuryuDecoBaseData.user.2";
        const string modVersion      = "1.1";

        var mods = new List<NexusMod>();
        foreach (var (lang, name) in Global.LANGUAGE_NAME_LOOKUP) {
            mods.Add(new() {
                name     = $"Gems Sorted by Gem Name ({name})",
                filename = $"Gems Sorted by Gem Name ({lang})",
                desc     = "Sorts all the gems by their name.",
                files    = new[] {decoPath, rampageDecoPath},
                action   = data => SortGems(data, GemSortType.GEM_NAME, lang),
                version  = modVersion
            });
            mods.Add(new() {
                name     = $"Gems Sorted by Skill Name ({name})",
                filename = $"Gems Sorted by Skill Name ({lang})",
                desc     = "Sorts all the gems by their skill name.",
                files    = new[] {decoPath, rampageDecoPath},
                action   = data => SortGems(data, GemSortType.SKILL_NAME, lang),
                version  = modVersion
            });
        }

        WriteMods(mods, inPath, outPath);
    }

    private void Btn_sort_gems_by_gem_name_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        SortGems(file.rsz.objectData, GemSortType.GEM_NAME, Global.locale);
    }

    private void Btn_sort_gems_by_skill_name_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        SortGems(file.rsz.objectData, GemSortType.SKILL_NAME, Global.locale);
    }

    private static void SortGems(IEnumerable<RszObject> rszObjectData, GemSortType sortType, Global.LangIndex lang) {
        var currentLang = Global.locale;
        Global.locale = lang;
        var gems = rszObjectData.OfType<IGem>().Where(gem => gem.SortId > 0).ToList();
        var gemsInNameOrder = sortType switch {
            GemSortType.GEM_NAME => from gem in gems
                                    orderby gem.Name
                                    select gem,
            GemSortType.SKILL_NAME => from gem in gems
                                      orderby gem.GetFirstSkillName(Global.locale)
                                      select gem,
            _ => throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null)
        };
        var sortId = gems.Any() && gems[0] is Snow_data_HyakuryuDecoBaseUserData_Param ? 15000u : 10000u;
        foreach (var gem in gemsInNameOrder) {
            gem.SortId = sortId++;
        }
        Global.locale = currentLang;
    }

    private enum GemSortType {
        GEM_NAME,
        SKILL_NAME
    }
}