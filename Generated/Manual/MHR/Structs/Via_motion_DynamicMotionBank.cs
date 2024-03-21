using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_motion_DynamicMotionBank : RszObject, ICustomReadWrite {
    public float  V0 { get; set; }
    public bool   V1 { get; set; }
    public float  V2 { get; set; }
    public bool   V3 { get; set; }
    public float  V4 { get; set; }
    public bool   V5 { get; set; }
    public float  V6 { get; set; }
    public string V7 { get; set; }
    public string V8 { get; set; }

    public void Read(BinaryReader reader) {
        V0 = reader.ReadSingle();
        V1 = reader.ReadBoolean();
        V2 = reader.ReadSingle();
        V3 = reader.ReadBoolean();
        V4 = reader.ReadSingle();
        V5 = reader.ReadBoolean();
        V6 = reader.ReadSingle();
        V7 = new(reader.ReadChars(4));
        V8 = new(reader.ReadChars(4));
    }

    public void Write(BinaryWriter writer) {
        writer.BaseStream.Align(4);
        writer.Write(V0);
        writer.Write(V1);
        writer.BaseStream.Align(4);
        writer.Write(V2);
        writer.Write(V3);
        writer.BaseStream.Align(4);
        writer.Write(V4);
        writer.Write(V5);
        writer.BaseStream.Align(4);
        writer.Write(V6);
        writer.BaseStream.Align(4);
        writer.Write(V7.ToCharArray());
        writer.BaseStream.Align(4);
        writer.Write(V8.ToCharArray());
    }

    public Via_motion_DynamicMotionBank Copy() {
        var obj = base.Copy<Via_motion_DynamicMotionBank>();
        obj.V0 = V0;
        obj.V1 = V1;
        obj.V2 = V2;
        obj.V3 = V3;
        obj.V4 = V4;
        obj.V5 = V5;
        obj.V6 = V6;
        obj.V7 = V7;
        obj.V8 = V8;
        return obj;
    }
}