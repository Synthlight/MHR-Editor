using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Range : RszObject, IViaType {
    public float R { get; set; }
    public float S { get; set; }

    public void Read(BinaryReader reader) {
        R = reader.ReadSingle();
        S = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(R);
        writer.Write(S);
    }

    public Range Copy() {
        var obj = base.Copy<Range>();
        obj.R = R;
        obj.S = S;
        return obj;
    }
}