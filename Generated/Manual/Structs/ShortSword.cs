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
public partial class Snow_equip_ShortSwordBaseUserData_Param : IMeleeWeapon {
    [SortOrder(50)]
    public string Name => DataHelper.WEAPON_NAME_LOOKUP[Global.locale].TryGet(Id);

    [SortOrder(int.MaxValue)]
    public string Description => DataHelper.WEAPON_DESC_LOOKUP[Global.locale].TryGet(Id);

    public override string ToString() {
        return Name;
    }
}