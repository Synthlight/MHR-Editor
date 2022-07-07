using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Generated;
using MHR_Editor.Models.Enums;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_ItemUserData_Param {
    [SortOrder(50)]
    public string Name => DataHelper.ITEM_NAME_LOOKUP[Global.locale].TryGet(Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.ITEM_DESC_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }

    public uint GetItemType() {
        return Id & BitMasks.ITEM_TYPE_BIT_MASK;
    }

    public uint GetItemId() {
        return Id & BitMasks.ITEM_ID_BIT_MASK;
    }

    public uint GetItemEnumId() {
        return (uint) Enum.GetValues<Snow_data_ContentsIdSystem_ItemId>()[Index - 1] & BitMasks.ITEM_ID_BIT_MASK;
    }

    public Snow_data_ContentsIdSystem_SubCategoryType GetItemTypeEnum() {
        return (Snow_data_ContentsIdSystem_SubCategoryType) GetItemType();
    }
}