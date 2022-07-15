using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

    public void Write(BinaryWriter writer) {
        writer.Write(int.Parse(RGBA[1..2], NumberStyles.HexNumber));
        writer.Write(int.Parse(RGBA[3..4], NumberStyles.HexNumber));
        writer.Write(int.Parse(RGBA[5..6], NumberStyles.HexNumber));
        writer.Write(int.Parse(RGBA[7..8], NumberStyles.HexNumber));
    }
}