using System.Collections.ObjectModel;
using RE_Editor.Common.Models.List_Wrappers;

namespace RE_Editor.Generated.Models;

public interface ISharpness {
    public ObservableCollection<GenericWrapper<int>> SharpnessValList { get; set; }
    public ObservableCollection<GenericWrapper<int>> TakumiValList    { get; set; }
    public ObservableCollection<GenericWrapper<int>> HandicraftValList {
        get => TakumiValList;
        set => TakumiValList = value;
    }
}