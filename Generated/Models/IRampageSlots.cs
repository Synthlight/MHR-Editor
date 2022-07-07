using System.Collections.ObjectModel;
using MHR_Editor.Common.Models.List_Wrappers;

namespace MHR_Editor.Generated.Models;

public interface IRampageSlots {
    public ObservableCollection<GenericWrapper<uint>> HyakuryuSlotNumList { get; set; }
    public ObservableCollection<GenericWrapper<uint>> RampageSlotNumList {
        get => HyakuryuSlotNumList;
        set => HyakuryuSlotNumList = value;
    }
}