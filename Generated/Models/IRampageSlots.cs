using System.Collections.ObjectModel;
using RE_Editor.Common.Models.List_Wrappers;

namespace RE_Editor.Generated.Models;

public interface IRampageSlots {
    public ObservableCollection<GenericWrapper<uint>> HyakuryuSlotNumList { get; set; }
    public ObservableCollection<GenericWrapper<uint>> RampageSlotNumList {
        get => HyakuryuSlotNumList;
        set => HyakuryuSlotNumList = value;
    }
}