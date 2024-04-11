using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common.Attributes;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Chainsaw_InventoryItemSaveData {
    [SortOrder(99999999)]
    public string Summary => Item[0].ItemId_button;
}