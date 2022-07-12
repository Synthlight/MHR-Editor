using System.Diagnostics.CodeAnalysis;
using System.IO;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Range : RszObject, IViaType {
    public float R { get; set; }
    public float S { get; set; }

    public void Read(BinaryReader reader) {
        R = reader.ReadSingle();
        S = reader.ReadSingle();
    }
}