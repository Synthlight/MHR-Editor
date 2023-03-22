using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Quaternion : RszObject, IViaType {
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

    public Quaternion Copy() {
        var obj = base.Copy<Quaternion>();
        obj.X = X;
        obj.Y = Y;
        obj.W = W;
        obj.Z = Z;
        return obj;
    }
}