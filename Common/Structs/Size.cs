using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Size : RszObject, IViaType {
    public float W { get; set; }
    public float H { get; set; }

    public void Read(BinaryReader reader) {
        W = reader.ReadSingle();
        H = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(W);
        writer.Write(H);
    }

    public Size Copy() {
        return new() {
            W = W,
            H = H
        };
    }
}