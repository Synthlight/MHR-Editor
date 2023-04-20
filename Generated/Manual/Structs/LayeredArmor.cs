using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_equip_PlOverwearBaseUserData_Param {
    [SortOrder(50)]
    public string Name => DataHelper.ARMOR_SERIES_LOOKUP[Global.locale].TryGet((uint) Series);

    public override string ToString() {
        return Name;
    }

    public uint GetArmorId() {
        return (uint) RelativeId & BitMasks.ARMOR_ID_BIT_MASK;
    }

    public uint GetLayeredId() {
        return (uint) Id & BitMasks.LAYERED_ID_BIT_MASK;
    }
}