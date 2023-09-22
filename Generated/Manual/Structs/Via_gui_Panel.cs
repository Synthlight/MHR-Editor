using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common.Models;

// ReSharper disable CheckNamespace
namespace RE_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Via_gui_Panel : RszObject, ICustomReadWrite {
    // TODO: Implement this class.
    public void Read(BinaryReader reader) {
    }

    public void Write(BinaryWriter writer) {
    }

    public Via_gui_Panel Copy() {
        var obj = base.Copy<Via_gui_Panel>();
        return obj;
    }
}