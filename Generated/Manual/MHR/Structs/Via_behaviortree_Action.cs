using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Models;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_behaviortree_Action : RszObject, ICustomReadWrite {
    public bool Enabled { get; set; }
    public uint Id      { get; set; }

    public void Read(BinaryReader reader) {
        Enabled = reader.ReadBoolean();
        reader.BaseStream.Align(4);
        Id = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(Enabled);
        writer.BaseStream.Align(4);
        writer.Write(Id);
    }

    public Via_behaviortree_Action Copy() {
        var obj = base.Copy<Via_behaviortree_Action>();
        obj.Enabled = Enabled;
        obj.Id      = Id;
        return obj;
    }
}