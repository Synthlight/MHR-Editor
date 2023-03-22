using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GameObjectRef : RszObject, IViaType {
    public System.Guid Value { get; set; }

    public void Read(BinaryReader reader) {
        Value = new(reader.ReadBytes(16));
    }

    public void Write(BinaryWriter writer) {
        writer.Write(Value.ToByteArray());
    }

    public GameObjectRef Copy() {
        return new() {
            Value = Value
        };
    }
}