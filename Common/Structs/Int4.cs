using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Int4 : RszObject, IViaType {
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int W { get; set; }

    public void Read(BinaryReader reader) {
        X = reader.ReadInt32();
        Y = reader.ReadInt32();
        Z = reader.ReadInt32();
        W = reader.ReadInt32();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
        writer.Write(W);
    }

    public Int4 Copy() {
        return new() {
            X = X,
            Y = Y,
            Z = Z,
            W = W
        };
    }
}