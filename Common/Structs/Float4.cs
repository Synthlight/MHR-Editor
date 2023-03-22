using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Float4 : RszObject, IViaType {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }

    public void Read(BinaryReader reader) {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
        W = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
        writer.Write(W);
    }

    public Float4 Copy() {
        return new() {
            X = X,
            Y = Y,
            Z = Z,
            W = W
        };
    }
}