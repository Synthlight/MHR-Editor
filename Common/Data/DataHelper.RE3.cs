using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618

namespace RE_Editor.Common.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class DataHelper {
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>> ITEM_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>> WEAPON_NAME_LOOKUP;
}