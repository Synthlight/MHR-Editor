using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Float2 : RszObject, IViaType {
    public float X { get; set; }
    public float Y { get; set; }

    public void Read(BinaryReader reader) {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(X);
        writer.Write(Y);
    }

    public Float2 Copy() {
        return new() {
            X = X,
            Y = Y
        };
    }
}