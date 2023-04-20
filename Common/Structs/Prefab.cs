﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

#pragma warning disable CS8618

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Prefab : RszObject, IViaType {
    public bool   Enabled { get; set; }
    public string Name    { get; set; }

    public void Read(BinaryReader reader) {
        Enabled = reader.ReadBoolean();
        reader.BaseStream.Align(4);
        Name = reader.ReadWString();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(Enabled);
        writer.BaseStream.Align(4);
        writer.WriteWString(Name);
    }

    public Prefab Copy() {
        return new() {
            Enabled = Enabled,
            Name = Name
        };
    }
}