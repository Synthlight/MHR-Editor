using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RangeI : RszObject, IViaType {
    public int R { get; set; }
    public int S { get; set; }

    public void Read(BinaryReader reader) {
        R = reader.ReadInt32();
        S = reader.ReadInt32();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(R);
        writer.Write(S);
    }

    public RangeI Copy() {
        return new() {
            R = R,
            S = S
        };
    }
}