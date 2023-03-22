using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Vec3 : RszObject, IViaType {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public void Read(BinaryReader reader) {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
        reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
        writer.Write(0f);
    }

    public Vec3 Copy() {
        return new() {
            X = X,
            Y = Y,
            Z = Z
        };
    }
}