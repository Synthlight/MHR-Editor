using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;
using RE_Editor.Generated.Models;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_DecorationsBaseUserData_Param : IGem {
    [SortOrder(50)]
    public string Name => DataHelper.DECORATION_NAME_LOOKUP[Global.locale].TryGet(Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.DECORATION_DESC_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }

    public uint GetFirstSkillId() {
        return SkillIdList[0].Value;
    }

    public uint Level => (uint) DecorationLv;

    public string GetFirstSkillName(Global.LangIndex lang) {
        return DataHelper.SKILL_NAME_LOOKUP[lang].TryGet(SkillIdList[0].Value);
    }
}