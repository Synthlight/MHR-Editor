using System.Collections.ObjectModel;
using MHR_Editor.Common.Models.List_Wrappers;

namespace MHR_Editor.Generated.Models;

public interface ISharpness {
    public ObservableCollection<GenericWrapper<int>> SharpnessValList { get; set; }
    public ObservableCollection<GenericWrapper<int>> TakumiValList    { get; set; }
    public ObservableCollection<GenericWrapper<int>> HandicraftValList {
        get => TakumiValList;
        set => TakumiValList = value;
    }
}