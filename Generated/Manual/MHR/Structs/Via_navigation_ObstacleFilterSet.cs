using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_navigation_ObstacleFilterSet : RszObject, ICustomReadWrite {
    public string V0 { get; set; }
    public float  V1 { get; set; }

    public void Read(BinaryReader reader) {
        V0 = new(reader.ReadChars(4));
        V1 = reader.ReadSingle();
    }

    public void Write(BinaryWriter writer) {
        writer.BaseStream.Align(4);
        writer.Write(V0.ToCharArray());
        writer.BaseStream.Align(4);
        writer.Write(V1);
    }

    public Via_navigation_ObstacleFilterSet Copy() {
        var obj = base.Copy<Via_navigation_ObstacleFilterSet>();
        obj.V0 = V0;
        obj.V1 = V1;
        return obj;
    }
}