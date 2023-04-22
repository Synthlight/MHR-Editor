using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common.Models;

#pragma warning disable CS8618

namespace RE_Editor.Common.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DataHelper {
    public static readonly Dictionary<uint, Type>       RE_STRUCTS = new();
    public static          Dictionary<uint, StructJson> STRUCT_INFO;

    public static Dictionary<Global.LangIndex, Dictionary<uint, string>> ITEM_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>> WEAPON_NAME_LOOKUP;
}