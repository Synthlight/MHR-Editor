using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Models.Enums;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_ArmorBaseUserData_Param {
    private const uint armorTypeBitMask = 0xFFF00000;
    private const uint armorIdBitMask   = armorTypeBitMask ^ 0xFFFFFFFF;

    [SortOrder(50)]
    public string Name => DataHelper.ARMOR_NAME_LOOKUP[Global.locale].TryGet(Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.ARMOR_DESC_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }

    public uint GetArmorType() {
        return Id & armorTypeBitMask;
    }

    public uint GetArmorId() {
        return Id & armorIdBitMask;
    }

    public string GetArmorTypeName() {
        return Enum.GetValues<Snow_data_ContentsIdSystem_SubCategoryType>().First(type => (uint) type == GetArmorType()).ToString();
    }
}