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
    private const uint subCategoryTypeBitMask = 0xFFF00000;
    private const uint subCategoryIdBitMask   = subCategoryTypeBitMask ^ 0xFFFFFFFF;

    [SortOrder(50)]
    public string Name => DataHelper.ARMOR_NAME_LOOKUP[Global.locale].TryGet(Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.ARMOR_DESC_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }

    public uint GetSubcategoryType() {
        return Id & subCategoryTypeBitMask;
    }

    public uint GetSubcategoryId() {
        return Id & subCategoryIdBitMask;
    }

    public string GetSubcategoryName() {
        return Enum.GetValues<Snow_data_ContentsIdSystem_SubCategoryType>().First(type => (uint) type == GetSubcategoryType()).ToString();
    }
}