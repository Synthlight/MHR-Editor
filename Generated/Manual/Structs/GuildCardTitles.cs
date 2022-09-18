using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class Snow_data_AchievementUserData_Param {
    [SortOrder(50)]
    public string RealName => DataHelper.GC_TITLE_NAME_LOOKUP[Global.locale].TryGet(Name);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.GC_TITLE_DESC_LOOKUP[Global.locale].TryGet(Explin);

    public int IdFromName() {
        return int.Parse(Name.Replace("MR_", "").Replace("N_", ""));
    }

    public override string ToString() {
        return RealName;
    }
}