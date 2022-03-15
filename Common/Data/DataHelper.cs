using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common.Models;

#pragma warning disable CS8618

namespace MHR_Editor.Common.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DataHelper {
    public static readonly Dictionary<uint, Type>                                 MHR_STRUCTS = new();
    public static          Dictionary<uint, StructJson>                           STRUCT_INFO;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> ARMOR_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> ARMOR_DESC_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> CAT_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> CAT_DESC_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> DANGO_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> DANGO_DESC_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> DANGO_SKILL_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> DECORATION_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> DECORATION_DESC_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> DOG_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> DOG_DESC_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> ITEM_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> ITEM_DESC_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> RAMPAGE_SKILL_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> SKILL_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> WEAPON_NAME_LOOKUP;
    public static          Dictionary<Global.LangIndex, Dictionary<uint, string>> WEAPON_DESC_LOOKUP;
}