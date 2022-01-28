using System.Diagnostics.CodeAnalysis;

namespace MHR_Editor.Common.Models.List_Wrappers;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class GenericWrapper<T> : ListWrapper<T> where T : struct {
    public          int Index { get; }
    public override T   Value { get; set; }

    public GenericWrapper(int index, T value) {
        Index = index;
        Value = value;
    }
}