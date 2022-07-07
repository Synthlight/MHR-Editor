using System.Collections.ObjectModel;
using MHR_Editor.Common.Models.List_Wrappers;

namespace MHR_Editor.Generated.Models;

public interface ISlots {
    public ObservableCollection<GenericWrapper<uint>> SlotNumList { get; set; }
}