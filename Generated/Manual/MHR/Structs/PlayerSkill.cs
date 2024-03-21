using System.Diagnostics.CodeAnalysis;
using RE_Editor.Models.Enums;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;

namespace RE_Editor.Models.Structs;

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

    [SortOrder(2000)]
    public Snow_data_DataDef_PlEquipSkillId EnumName => Enum.GetValues<Snow_data_DataDef_PlEquipSkillId>()[Index];
}