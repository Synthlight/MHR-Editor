using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common.Models;

#pragma warning disable CS8618

namespace RE_Editor.Common.Data;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class DataHelper {
    public static readonly Dictionary<uint, Type>       RE_STRUCTS = new();
    public static          Dictionary<uint, StructJson> STRUCT_INFO;
    public static          Dictionary<uint, uint>       GP_CRC_OVERRIDE_INFO;
}