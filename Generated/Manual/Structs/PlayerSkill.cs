using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_PlEquipSkillBaseUserData_Param {
    [SortOrder(50)]
    public string Name => DataHelper.SKILL_NAME_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }
}