﻿using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common.Models;

#pragma warning disable CS8618

namespace RE_Editor.Common.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DataHelper {
    public static readonly Dictionary<uint, Type>       RE_STRUCTS = new();
    public static          Dictionary<uint, StructJson> STRUCT_INFO;

    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   ARMOR_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   ARMOR_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   ARMOR_SERIES_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   CAT_DOG_ARMOR_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   CAT_DOG_ARMOR_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   CAT_DOG_WEAPON_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   CAT_DOG_WEAPON_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   DANGO_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   DANGO_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   DANGO_SKILL_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   DECORATION_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   DECORATION_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<string, string>> GC_TITLE_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<string, string>> GC_TITLE_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   ITEM_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   ITEM_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   PETALACE_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   PETALACE_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   RAMPAGE_DECORATION_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   RAMPAGE_DECORATION_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   RAMPAGE_SKILL_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   SKILL_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   SWITCH_SKILL_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   SWITCH_SKILL_DESC_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   WEAPON_NAME_LOOKUP;
    public static Dictionary<Global.LangIndex, Dictionary<uint, string>>   WEAPON_DESC_LOOKUP;
}