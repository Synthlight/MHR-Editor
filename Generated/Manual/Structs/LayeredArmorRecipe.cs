using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models.List_Wrappers;
using MHR_Editor.Generated.Models;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_equip_PlOverwearProductUserData_Param : ICraftingCost {
    [SortOrder(50)]
    public string Name => DataHelper.ARMOR_SERIES_LOOKUP[Global.locale].TryGet(GetLayeredId());

    public override string ToString() {
        return Name;
    }

    public uint GetLayeredId() {
        return (uint) Id & BitMasks.LAYERED_ID_BIT_MASK;
    }

    [DisplayName("")]
    public ObservableCollection<DataSourceWrapper<uint>> ItemIdList {
        get => Item;
        set => Item = value;
    }

    [DisplayName("")]
    public ObservableCollection<GenericWrapper<uint>> ItemNumList {
        get => ItemNum;
        set => ItemNum = value;
    }
}