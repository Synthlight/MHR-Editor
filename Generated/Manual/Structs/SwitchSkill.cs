using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_PlSwitchActionBaseUserData_Param {
    [SortOrder(50)]
    public string Name => DataHelper.SWITCH_SKILL_NAME_LOOKUP[Global.locale].TryGet((uint) ActionId);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.SWITCH_SKILL_DESC_LOOKUP[Global.locale].TryGet((uint) ActionId);

    public override string ToString() {
        return Name;
    }
}