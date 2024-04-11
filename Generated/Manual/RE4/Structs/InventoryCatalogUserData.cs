using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common.Attributes;
using RE_Editor.Models.Enums;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Chainsaw_InventoryCatalogUserData_Data {
    [SortOrder(50)]
    public string Name => CharacterKindID switch {
        Chainsaw_CharacterKindID.ch0_a0z0 => "Leon (Main)",
        Chainsaw_CharacterKindID.ch6_i0z0 => "Leon (MC)",
        Chainsaw_CharacterKindID.ch3_a8z0 => "Ada (AO)",
        _ => "Unknown"
    };
}