using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Common.Structs;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_render_ColorCorrectLinearParams : RszObject, ICustomReadWrite {
    public float                      V0 { get; set; }
    public ObservableCollection<Vec4> V1 { get; set; }

    public void Read(BinaryReader reader) {
        reader.BaseStream.Align(4);
        V0 = reader.ReadSingle();
        reader.BaseStream.Align(16);
        V1 = new() {new()};
        V1[0].Read(reader);
    }

    public void Write(BinaryWriter writer) {
        writer.BaseStream.Align(4);
        writer.Write(V0);
        writer.BaseStream.Align(16);
        V1[0].Write(writer);
    }

    public Via_render_ColorCorrectLinearParams Copy() {
        var obj = base.Copy<Via_render_ColorCorrectLinearParams>();
        obj.V0 = V0;
        obj.V1 = new() {V1[0]};
        return obj;
    }
}