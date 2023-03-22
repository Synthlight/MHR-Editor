using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Common.Structs;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class Via_AnimationCurve : RszObject, ICustomReadWrite {
    public ObservableCollection<Vec4> V0 { get; set; }
    public float                      V1 { get; set; }
    public float                      V2 { get; set; }
    public float                      V3 { get; set; }
    public float                      V4 { get; set; }
    public float                      V5 { get; set; }
    public float                      V6 { get; set; }

    public void Read(BinaryReader reader) {
        V0 = new();
        var count = reader.ReadInt32();
        if (count > 0) {
            reader.BaseStream.Align(16);
            for (var i = 0; i < count; i++) {
                var vec4 = new Vec4();
                vec4.Read(reader);
                V0.Add(vec4);
            }
        }
        V1 = reader.ReadSingle();
        V2 = reader.ReadSingle();
        V3 = reader.ReadSingle();
        V4 = reader.ReadSingle();
        V5 = reader.ReadSingle();
        V6 = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(V0.Count);
        if (V0.Count > 0) {
            writer.BaseStream.Align(16);
            foreach (var vec4 in V0) {
                vec4.Write(writer);
            }
        }
        writer.Write(V1);
        writer.Write(V2);
        writer.Write(V3);
        writer.Write(V4);
        writer.Write(V5);
        writer.Write(V6);
    }

    public Via_AnimationCurve Copy() {
        var obj = base.Copy<Via_AnimationCurve>();
        obj.V0 ??= new();
        foreach (var x in V0) {
            obj.V0.Add(x.Copy());
        }
        obj.V1 = V1;
        obj.V2 = V2;
        obj.V3 = V3;
        obj.V4 = V4;
        obj.V5 = V5;
        obj.V6 = V6;
        return obj;
    }
}