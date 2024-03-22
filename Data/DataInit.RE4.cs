using System.Collections.Generic;
using RE_Editor.Common;
using RE_Editor.Common.Data;

namespace RE_Editor.Data;

public static partial class DataInit {
    // ReSharper disable once IdentifierTypo
    private static void LoadDicts() {
        DataHelper.ITEM_NAME_LOOKUP   = new();
        DataHelper.WEAPON_NAME_LOOKUP = new();

        foreach (var variant in Global.VARIANTS) {
            DataHelper.ITEM_NAME_LOOKUP[variant]   = LoadDict<Global.LangIndex, Dictionary<uint, string>>(GetAsset($"{variant}_ITEM_NAME_LOOKUP"));
            DataHelper.WEAPON_NAME_LOOKUP[variant] = LoadDict<Global.LangIndex, Dictionary<uint, string>>(GetAsset($"{variant}_WEAPON_NAME_LOOKUP"));
        }
        // Now 'fill in' the blanks with stuff from base without overwriting anything.
        foreach (var (key, value) in DataHelper.ITEM_NAME_LOOKUP["CH"]) {
            if (!DataHelper.ITEM_NAME_LOOKUP["AO"].ContainsKey(key)) DataHelper.ITEM_NAME_LOOKUP["AO"][key] = value;
            if (!DataHelper.ITEM_NAME_LOOKUP["MC"].ContainsKey(key)) DataHelper.ITEM_NAME_LOOKUP["MC"][key] = value;
        }
        foreach (var (key, value) in DataHelper.WEAPON_NAME_LOOKUP["CH"]) {
            if (!DataHelper.WEAPON_NAME_LOOKUP["AO"].ContainsKey(key)) DataHelper.WEAPON_NAME_LOOKUP["AO"][key] = value;
            if (!DataHelper.WEAPON_NAME_LOOKUP["MC"].ContainsKey(key)) DataHelper.WEAPON_NAME_LOOKUP["MC"][key] = value;
        }
    }
}