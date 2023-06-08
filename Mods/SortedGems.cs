using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Generated.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class SortedGems : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string version           = "1.7.1";
        const string outPath           = $@"{PathHelper.MODS_PATH}\Sorted Gems";
        const string bundleByNameName  = "Gems Sorted by Gem Name";
        const string bundleBySkillName = "Gems Sorted by Skill Name";
        const string bundleBySkillId   = "Gems Sorted by Skill ID";

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
        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, $"{bundleByNameName} (Fluffy Selective Install)", true);

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
        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, $"{bundleBySkillName} (Fluffy Selective Install)", true);

        ModMaker.WriteMods(new List<INexusMod> {
            new NexusMod {
                Name    = bundleBySkillId,
                Desc    = "Sorts all the gems by their skill id.",
                Files   = new[] {PathHelper.DECORATION_PATH, PathHelper.RAMPAGE_DECORATION_PATH},
                Action  = data => SortGems(data, GemSortType.SKILL_ID, Global.LangIndex.eng), // Lang doesn't matter for this one.
                Version = version
            }
        }, PathHelper.CHUNK_PATH, outPath, copyToFluffy: true);
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
            GemSortType.SKILL_ID => from gem in gems
                                    orderby gem.GetFirstSkillId(), gem.Level
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