using System.Collections.Generic;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using RE_Editor.Data.DD2;

namespace RE_Editor.Data;

public static partial class DataInit {
    // ReSharper disable once IdentifierTypo
    private static void LoadDicts() {
        DataHelper.ITEM_NAME_LOOKUP = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ITEM_NAME_LOOKUP);
    }
}