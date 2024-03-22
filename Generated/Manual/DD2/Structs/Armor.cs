﻿using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;

namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class App_ItemArmorParam {
    [SortOrder(50)]
    public string Name => DataHelper.ITEM_NAME_LOOKUP[Global.locale].TryGet((uint) Id);
}