using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Controls.Models;
using MHR_Editor.Common.Data;

namespace MHR_Editor.Common.Models.List_Wrappers;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class SkillId<T> : ListWrapper<T> where T : struct {
    public int Index { get; }

    private T Value_raw;
    [DataSource(DataSourceType.SKILLS)]
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
    public string Value_button => DataHelper.SKILL_NAME_LOOKUP.TryGet((byte) (object) Value).ToStringWithId(Value);

    public SkillId(int index, T value) {
        Index = index;
        Value = value;
    }
}