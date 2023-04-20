using System.Collections.ObjectModel;
using RE_Editor.Common.Models.List_Wrappers;

namespace RE_Editor.Generated.Models;

public interface ICraftingCost {
    public ObservableCollection<DataSourceWrapper<uint>> ItemIdList  { get; set; }
    public ObservableCollection<GenericWrapper<uint>>    ItemNumList { get; set; }
}