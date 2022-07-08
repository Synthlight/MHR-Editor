using System.Collections.ObjectModel;
using MHR_Editor.Common.Models.List_Wrappers;

namespace MHR_Editor.Generated.Models;

public interface ICraftingCost {
    public ObservableCollection<DataSourceWrapper<uint>> ItemIdList  { get; set; }
    public ObservableCollection<GenericWrapper<uint>>    ItemNumList { get; set; }
}