using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Controls.Models;
using MHR_Editor.Common.Data;

namespace MHR_Editor.Common.Models.List_Wrappers;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class DataSourceWrapper<T> : ListWrapper<T> where T : struct {
    private readonly StructJson.Field field;

    public int Index { get; }

    private T Value_raw;
    public override T Value {
        get => Value_raw;
        set {
            if (EqualityComparer<T>.Default.Equals(Value_raw, value)) return;
            Value_raw = value;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(Value_button));
        }
    }

    [CustomSorter(typeof(ButtonSorter))]
    [DisplayName("Value")]
    public string Value_button => GetDataLookupSource()[Global.locale].TryGet((uint) Convert.ChangeType(Value, TypeCode.UInt32)).ToStringWithId(Value, ShowAsHex());

    public DataSourceWrapper(int index, T value, StructJson.Field field) {
        Index      = index;
        Value      = value;
        this.field = field;
    }

    private Dictionary<Global.LangIndex, Dictionary<uint, string>> GetDataLookupSource() {
        return field.originalType?.Replace("[]", "") switch {
            "snow.data.ContentsIdSystem.ItemId" => DataHelper.ITEM_NAME_LOOKUP,
            "snow.data.DataDef.PlEquipSkillId" => DataHelper.SKILL_NAME_LOOKUP,
            "snow.data.DataDef.PlHyakuryuSkillId" => DataHelper.RAMPAGE_SKILL_NAME_LOOKUP,
            "snow.data.DataDef.PlKitchenSkillId" => DataHelper.DANGO_NAME_LOOKUP,
            _ => throw new InvalidOperationException($"No data source lookup known for: {field.originalType}")
        };
    }

    private bool ShowAsHex() {
        return field.originalType?.Replace("[]", "") switch {
            "snow.data.ContentsIdSystem.ItemId" => true,
            _ => false
        };
    }
}