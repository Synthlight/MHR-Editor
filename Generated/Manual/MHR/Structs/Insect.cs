using System.Diagnostics.CodeAnalysis;
using RE_Editor.Models.Enums;
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
    public string Name => DataHelper.WEAPON_NAME_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }

    public Snow_data_ContentsIdSystem_WeaponId GetWeaponEnum() {
        return (Snow_data_ContentsIdSystem_WeaponId) Id;
    }

    public uint GetWeaponId() {
        return Id & BitMasks.ITEM_ID_BIT_MASK;
    }
}