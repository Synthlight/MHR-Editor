﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

#pragma warning disable CS8618

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Color : RszObject, IViaType {
    public string RGBA { get; set; }

    public void Read(BinaryReader reader) {
        var r = reader.ReadByte();
        var g = reader.ReadByte();
        var b = reader.ReadByte();
        var a = reader.ReadByte();
        RGBA = $"#{r:x2}{g:x2}{b:x2}{a:x2}";
    }

    public void Write(BinaryWriter writer) {
        writer.Write(byte.Parse(RGBA[1..3], NumberStyles.HexNumber));
        writer.Write(byte.Parse(RGBA[3..5], NumberStyles.HexNumber));
        writer.Write(byte.Parse(RGBA[5..7], NumberStyles.HexNumber));
        writer.Write(byte.Parse(RGBA[7..9], NumberStyles.HexNumber));
    }

    public Color Copy() {
        return new() {
            RGBA = RGBA
        };
    }
}