using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class App_user_data_ItemData_cData {
    [SortOrder(50)]
    public string Name => DataHelper.ITEM_INFO_LOOKUP_BY_GUID[Global.locale].TryGet(RawName.Value);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.ITEM_INFO_LOOKUP_BY_GUID[Global.locale].TryGet(RawExplain.Value);

    public override string ToString() {
        return Name;
    }
}