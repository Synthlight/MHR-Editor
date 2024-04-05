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
public partial class Snow_data_ArmorBaseUserData_Param {
    [SortOrder(50)]
    public string Name => DataHelper.ARMOR_NAME_LOOKUP[Global.locale].TryGet((uint) Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.ARMOR_DESC_LOOKUP[Global.locale].TryGet((uint) Id);

    public override string ToString() {
        return Name;
    }

    public uint GetArmorType() {
        return (uint) Id & BitMasks.ARMOR_TYPE_BIT_MASK;
    }

    public uint GetArmorId() {
        return (uint) Id & BitMasks.ARMOR_ID_BIT_MASK;
    }

    public Snow_data_ContentsIdSystem_SubCategoryType GetArmorTypeEnum() {
        return (Snow_data_ContentsIdSystem_SubCategoryType) GetArmorType();
    }
}