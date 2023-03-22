using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Mat4 : RszObject, IViaType {
    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float Z1 { get; set; }
    public float W1 { get; set; }
    public float X2 { get; set; }
    public float Y2 { get; set; }
    public float Z2 { get; set; }
    public float W2 { get; set; }
    public float X3 { get; set; }
    public float Y3 { get; set; }
    public float Z3 { get; set; }
    public float W3 { get; set; }
    public float X4 { get; set; }
    public float Y4 { get; set; }
    public float Z4 { get; set; }
    public float W4 { get; set; }

    public void Read(BinaryReader reader) {
        X1 = reader.ReadSingle();
        Y1 = reader.ReadSingle();
        Z1 = reader.ReadSingle();
        W1 = reader.ReadSingle();
        X2 = reader.ReadSingle();
        Y2 = reader.ReadSingle();
        Z2 = reader.ReadSingle();
        W2 = reader.ReadSingle();
        X3 = reader.ReadSingle();
        Y3 = reader.ReadSingle();
        Z3 = reader.ReadSingle();
        W3 = reader.ReadSingle();
        X4 = reader.ReadSingle();
        Y4 = reader.ReadSingle();
        Z4 = reader.ReadSingle();
        W4 = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(X1);
        writer.Write(Y1);
        writer.Write(Z1);
        writer.Write(W1);
        writer.Write(X2);
        writer.Write(Y2);
        writer.Write(Z2);
        writer.Write(W2);
        writer.Write(X3);
        writer.Write(Y3);
        writer.Write(Z3);
        writer.Write(W3);
        writer.Write(X4);
        writer.Write(Y4);
        writer.Write(Z4);
        writer.Write(W4);
    }

    public Mat4 Copy() {
        return new() {
            X1 = X1,
            X2 = X2,
            X3 = X3,
            X4 = X4,
            Y1 = Y1,
            Y2 = Y2,
            Y3 = Y3,
            Y4 = Y4,
            Z1 = Z1,
            Z2 = Z2,
            Z3 = Z3,
            Z4 = Z4,
            W1 = W1,
            W2 = W2,
            W3 = W3,
            W4 = W4
        };
    }
}