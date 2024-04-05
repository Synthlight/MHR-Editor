using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_equip_InsectBaseUserData_Param {
    [SortOrder(50)]
    public string Name => DataHelper.WEAPON_NAME_LOOKUP[Global.locale].TryGet((uint) Id);

    public override string ToString() {
        return Name;
    }

    public uint GetWeaponId() {
        return (uint) Id & BitMasks.ITEM_ID_BIT_MASK;
    }
}