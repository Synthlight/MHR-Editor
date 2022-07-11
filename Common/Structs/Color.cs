using System.Diagnostics.CodeAnalysis;
using System.IO;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Common.Structs;

#pragma warning disable CS8618

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Color : RszObject, IViaType {
    public string RGBA { get; set; }

    public void Read(BinaryReader reader) {
        var r = reader.ReadInt32();
        var g = reader.ReadInt32();
        var b = reader.ReadInt32();
        var a = reader.ReadInt32();
        RGBA = $"#{r:x2}{g:x2}{b:x2}{a:x2}";
    }
}