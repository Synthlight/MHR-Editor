using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_wwise_WwiseSetStateParam : RszObject, ICustomReadWrite {
    public string V0 { get; set; }
    public string V1 { get; set; }

    public void Read(BinaryReader reader) {
        reader.BaseStream.Align(4);
        V0 = reader.ReadNullTermWString();
        reader.BaseStream.Align(4);
        V1 = reader.ReadNullTermWString();
    }

    public void Write(BinaryWriter writer) {
        writer.BaseStream.Align(4);
        writer.WriteNullTermWString(V0);
        writer.BaseStream.Align(4);
        writer.WriteNullTermWString(V1);
    }

    public Via_wwise_WwiseSetStateParam Copy() {
        var obj = base.Copy<Via_wwise_WwiseSetStateParam>();
        obj.V0 = V0;
        obj.V1 = V1;
        return obj;
    }
}