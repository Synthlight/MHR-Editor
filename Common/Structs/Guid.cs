using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Guid : RszObject, IViaType {
    public System.Guid Value { get; set; }

    public static ObservableCollection<Guid> NewIdInAList =>
        new() {
            new() {
                Value = System.Guid.NewGuid()
            }
        };

    public void Read(BinaryReader reader) {
        Value = new(reader.ReadBytes(16));
    }

    public void Write(BinaryWriter writer) {
        writer.Write(Value.ToByteArray());
    }
}