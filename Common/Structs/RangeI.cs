using System.Diagnostics.CodeAnalysis;
using System.IO;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RangeI : RszObject, IViaType {
    public int R { get; set; }
    public int S { get; set; }

    public void Read(BinaryReader reader) {
        R = reader.ReadInt32();
        S = reader.ReadInt32();
    }
}