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
public partial class Snow_data_ItemUserData_Param {
    private const uint itemTypeBitMask = 0xFFF00000;
    private const uint itemIdBitMask   = itemTypeBitMask ^ 0xFFFFFFFF;

    [SortOrder(50)]
    public string Name => DataHelper.ITEM_NAME_LOOKUP[Global.locale].TryGet(Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.ITEM_DESC_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }

    public uint GetItemType() {
        return Id & itemTypeBitMask;
    }

    public uint GetItemId() {
        return Id & itemIdBitMask;
    }

    public uint GetItemEnumId() {
        return (uint) Enum.GetValues<Snow_data_ContentsIdSystem_ItemId>()[Index - 1] & itemIdBitMask;
    }

    public string GetItemTypeName() {
        return Enum.GetValues<Snow_data_ContentsIdSystem_SubCategoryType>().First(type => (uint) type == GetItemType()).ToString();
    }
}