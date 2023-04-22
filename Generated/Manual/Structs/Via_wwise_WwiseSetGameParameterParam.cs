using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_wwise_WwiseSetGameParameterParam : RszObject, ICustomReadWrite {
    public string V0 { get; set; }
    public bool   V1 { get; set; }
    public bool   V2 { get; set; }
    public uint   V3 { get; set; }
    public uint   V4 { get; set; }

    public void Read(BinaryReader reader) {
        reader.BaseStream.Align(4);
        V0 = reader.ReadNullTermWString();
        V1 = reader.ReadBoolean();
        V2 = reader.ReadBoolean();
        reader.BaseStream.Align(4);
        V3 = reader.ReadUInt32();
        reader.BaseStream.Align(4);
        V4 = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer) {
        writer.BaseStream.Align(4);
        writer.WriteNullTermWString(V0);
        writer.Write(V1);
        writer.Write(V2);
        writer.BaseStream.Align(4);
        writer.Write(V3);
        writer.BaseStream.Align(4);
        writer.Write(V4);
    }

    public Via_wwise_WwiseSetGameParameterParam Copy() {
        var obj = base.Copy<Via_wwise_WwiseSetGameParameterParam>();
        obj.V0 = V0;
        obj.V1 = V1;
        obj.V2 = V2;
        obj.V3 = V3;
        obj.V4 = V4;
        return obj;
    }
}