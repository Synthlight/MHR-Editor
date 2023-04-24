using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_motion_MotionBank : RszObject, ICustomReadWrite {
    public string V0 { get; set; }
    public float  V1 { get; set; }
    public float  V2 { get; set; }
    public float  V3 { get; set; }

    public void Read(BinaryReader reader) {
        V0 = new(reader.ReadChars(4));
        V1 = reader.ReadSingle();
        V2 = reader.ReadSingle();
        V3 = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.BaseStream.Align(4);
        writer.Write(V0.ToCharArray());
        writer.BaseStream.Align(4);
        writer.Write(V1);
        writer.BaseStream.Align(4);
        writer.Write(V2);
        writer.BaseStream.Align(4);
        writer.Write(V3);
    }

    public Via_motion_MotionBank Copy() {
        var obj = base.Copy<Via_motion_MotionBank>();
        obj.V0 = V0;
        obj.V1 = V1;
        obj.V2 = V2;
        obj.V3 = V3;
        return obj;
    }
}