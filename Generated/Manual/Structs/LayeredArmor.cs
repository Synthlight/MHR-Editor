using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_equip_PlOverwearBaseUserData_Param {
    private const uint layeredIdBitMask = 0x0FFF;
    private const uint armorIdBitMask   = 0x000FFFFF;

    [SortOrder(50)]
    public string Name => DataHelper.ARMOR_SERIES_LOOKUP[Global.locale].TryGet((uint) Series);

    public override string ToString() {
        return Name;
    }

    public uint GetArmorId() {
        return (uint) RelativeId & armorIdBitMask;
    }

    public uint GetLayeredId() {
        return (uint) Id & layeredIdBitMask;
    }
}