using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;

namespace RE_Editor.Models.Structs;

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