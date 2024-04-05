using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;
using RE_Editor.Common.Models.List_Wrappers;
using RE_Editor.Generated.Models;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_ArmorProductUserData_Param : ICraftingCost {
    [SortOrder(50)]
    public string Name => DataHelper.ARMOR_NAME_LOOKUP[Global.locale].TryGet((uint) Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.ARMOR_DESC_LOOKUP[Global.locale].TryGet((uint) Id);

    public override string ToString() {
        return Name;
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