﻿using System;
using System.Collections.Generic;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using RE_Editor.Data.MHWS;

namespace RE_Editor.Data;

public static partial class DataInit {
    // ReSharper disable once IdentifierTypo
    private static void LoadDicts() {
        DataHelper.ITEM_NAME_LOOKUP         = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ITEM_NAME_LOOKUP);
        DataHelper.ITEM_DESC_LOOKUP         = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ITEM_DESC_LOOKUP);
        DataHelper.ITEM_INFO_LOOKUP_BY_GUID = LoadDict<Global.LangIndex, Dictionary<Guid, string>>(Assets.ITEM_INFO_LOOKUP_BY_GUID);

        foreach (var lang in Global.LANGUAGES) {
            DataHelper.ITEM_NAME_LOOKUP[lang][0] = "None";
            DataHelper.ITEM_DESC_LOOKUP[lang][0] = "None";
        }

        DataHelper.ARMOR_INFO_LOOKUP_BY_GUID  = LoadDict<Global.LangIndex, Dictionary<Guid, string>>(Assets.ARMOR_INFO_LOOKUP_BY_GUID);
        DataHelper.WEAPON_INFO_LOOKUP_BY_GUID = LoadDict<Global.LangIndex, Dictionary<Guid, string>>(Assets.WEAPON_INFO_LOOKUP_BY_GUID);
    }
}