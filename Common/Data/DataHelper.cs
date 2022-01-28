using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common.Models;

#pragma warning disable CS8618

namespace MHR_Editor.Common.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DataHelper {
    public static readonly Dictionary<uint, Type>       MHR_STRUCTS = new();
    public static          Dictionary<uint, StructJson> STRUCT_INFO;
    public static          Dictionary<uint, string>     ARMOR_NAME_LOOKUP;
    public static          Dictionary<uint, string>     ITEM_NAME_LOOKUP;
    public static          Dictionary<byte, string>     SKILL_NAME_LOOKUP;
    public static          Dictionary<uint, string>     WEAPON_NAME_LOOKUP;
}