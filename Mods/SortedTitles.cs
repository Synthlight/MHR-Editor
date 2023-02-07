using System;
using System.Collections.Generic;
using System.Linq;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Models;
using MHR_Editor.Models.Structs;
using MHR_Editor.Util;

namespace MHR_Editor.Mods;

public static class SortedTitles {
    public static void Make() {
        const string version      = "1.3";
        const string outPath      = $@"{PathHelper.MODS_PATH}\Sorted Titles";
        const string bundleByName = "Titles Sorted by Name";

        var langs = Enum.GetValues<Global.LangIndex>();

        var mods = new List<INexusModVariant>();
        mods.AddRange(langs.Select(lang => new NexusModVariant {
                               Name         = $"{bundleByName} ({lang})",
                               NameAsBundle = bundleByName,
                               Desc         = "Sorts all the guid card titles by their name.",
                               Files        = new[] {PathHelper.GUILD_CARD_TITLE_DATA},
                               Action       = data => SortTitles(data, lang),
                               Version      = version
                           })
                           .Cast<INexusModVariant>());
        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, $"{bundleByName} (Fluffy Selective Install)", true);
    }

    public static void SortTitles(IEnumerable<RszObject> rszObjectData, Global.LangIndex lang) {
        var currentLang = Global.locale;
        Global.locale = lang;
        var titles = rszObjectData.OfType<Snow_data_AchievementUserData_Param>().Where(gem => gem.SortID > 0).ToList();
        var titlesInOrder = from title in titles
                            orderby title.RealName
                            select title;
        var sortId = 10000;
        foreach (var title in titlesInOrder) {
            title.SortID = sortId++;
        }
        Global.locale = currentLang;
    }
}