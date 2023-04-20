using System.Collections.ObjectModel;
using RE_Editor.Common.Models.List_Wrappers;

namespace RE_Editor.Generated.Models;

public interface ISlots {
    public ObservableCollection<GenericWrapper<uint>> SlotNumList { get; set; }
}