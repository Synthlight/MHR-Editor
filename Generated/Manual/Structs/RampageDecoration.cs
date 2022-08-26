using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Generated.Models;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_HyakuryuDecoBaseUserData_Param : IGem {
    [SortOrder(50)]
    public string Name => DataHelper.RAMPAGE_DECORATION_NAME_LOOKUP[Global.locale].TryGet(Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.RAMPAGE_DECORATION_DESC_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }

    public uint GetFirstSkillId() {
        return (uint) HyakuryuSkillId;
    }

    public uint Level => (uint) SlotType;

    public string GetFirstSkillName(Global.LangIndex lang) {
        return DataHelper.RAMPAGE_SKILL_NAME_LOOKUP[lang].TryGet((uint) HyakuryuSkillId);
    }
}