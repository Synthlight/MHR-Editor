using System;
using System.Collections.Generic;
using System.Linq;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Generated.Models;
using MHR_Editor.Models;
using MHR_Editor.Models.Structs;
using MHR_Editor.Util;

namespace MHR_Editor.Mods;

public static class SortedGems {
    public static void Make() {
        const string version           = "1.2";
        const string outPath           = $@"{PathHelper.MODS_PATH}\Sorted Gems";
        const string bundleByNameName  = "Gems Sorted by Gem Name";
        const string bundleBySkillName = "Gems Sorted by Skill Name";

        var langs = Enum.GetValues<Global.LangIndex>();

        var mods = new List<INexusModVariant>();
        mods.AddRange(langs.Select(lang => new NexusModVariant {
                               Name         = $"{bundleByNameName} ({lang})",
                               NameAsBundle = bundleByNameName,
                               Desc         = "Sorts all the gems by their name.",
                               Files        = new[] {PathHelper.DECORATION_PATH, PathHelper.RAMPAGE_DECORATION_PATH},
                               Action       = data => SortGems(data, GemSortType.GEM_NAME, lang),
                               Version      = version
                           })
                           .Cast<INexusModVariant>());
        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, $"{bundleByNameName} (Fluffy Selective Install)");

        mods.Clear();
        mods.AddRange(langs.Select(lang => new NexusModVariant {
                               Name         = $"{bundleBySkillName} ({lang})",
                               NameAsBundle = bundleBySkillName,
                               Desc         = "Sorts all the gems by their skill name.",
                               Files        = new[] {PathHelper.DECORATION_PATH, PathHelper.RAMPAGE_DECORATION_PATH},
                               Action       = data => SortGems(data, GemSortType.SKILL_NAME, lang),
                               Version      = version
                           })
                           .Cast<INexusModVariant>());
        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, $"{bundleBySkillName} (Fluffy Selective Install)");
    }

    public static void SortGems(IEnumerable<RszObject> rszObjectData, GemSortType sortType, Global.LangIndex lang) {
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
}