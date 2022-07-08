using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Models.Enums;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_PlHyakuryuSkillBaseUserData_Param {
    [SortOrder(50)]
    public string Name => DataHelper.RAMPAGE_SKILL_NAME_LOOKUP[Global.locale].TryGet((uint) Id);

    public override string ToString() {
        return Name;
    }

    public Snow_data_DataDef_PlHyakuryuSkillId GetSkillEnum() {
        return Enum.GetValues<Snow_data_DataDef_PlHyakuryuSkillId>()[Index];
    }
}