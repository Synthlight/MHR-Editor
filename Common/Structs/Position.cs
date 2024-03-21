using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Position : RszObject, IViaType { // TODO
    public void Read(BinaryReader reader) {
    }

    public void Write(BinaryWriter writer) {
    }

    public Position Copy() {
        return new() {
        };
    }
}