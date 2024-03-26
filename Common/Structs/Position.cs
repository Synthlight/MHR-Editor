using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Position : RszObject, IViaType {
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public void Read(BinaryReader reader) {
        X = reader.ReadDouble();
        Y = reader.ReadDouble();
        Z = reader.ReadDouble();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
    }

    public Position Copy() {
        return new() {
            X = X,
            Y = Y,
            Z = Z
        };
    }
}