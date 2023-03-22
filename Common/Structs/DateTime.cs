using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DateTime : RszObject, IViaType {
    public long Value { get; set; }

    public void Read(BinaryReader reader) {
        Value = reader.ReadInt64();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(Value);
    }

    public DateTime Copy() {
        return new() {
            Value = Value
        };
    }
}