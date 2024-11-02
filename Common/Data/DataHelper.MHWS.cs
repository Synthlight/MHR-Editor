using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618

namespace RE_Editor.Common.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public static partial class DataHelper {
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>> ITEM_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>> ITEM_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<Guid, string>> ITEM_INFO_LOOKUP_BY_GUID;
    public static Dictionary<Global.LangIndex, Dictionary<Guid, string>> ARMOR_INFO_LOOKUP_BY_GUID;
    public static Dictionary<Global.LangIndex, Dictionary<Guid, string>> WEAPON_INFO_LOOKUP_BY_GUID;
}