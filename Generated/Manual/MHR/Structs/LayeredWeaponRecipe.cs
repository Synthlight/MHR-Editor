using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common.Models.List_Wrappers;
using RE_Editor.Generated.Models;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_equip_OverwearWeaponProductUserData_Param : ICraftingCost {
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