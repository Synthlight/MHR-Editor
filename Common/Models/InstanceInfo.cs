﻿using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RE_Editor.Common.Models;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class InstanceInfo {
    public uint hash;
    public uint crc;

    public static InstanceInfo Read(BinaryReader reader) {
        var instanceInfo = new InstanceInfo();
        instanceInfo.hash = reader.ReadUInt32();
        instanceInfo.crc  = reader.ReadUInt32();
        return instanceInfo;
    }

    public void Write(BinaryWriter writer) {
        writer.Write(hash);
        writer.Write(crc);
    }
}