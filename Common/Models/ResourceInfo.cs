using System.Diagnostics.CodeAnalysis;
using System.IO;

#pragma warning disable CS8618

namespace RE_Editor.Common.Models;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class ResourceInfo {
    public  ReDataFile file;
    public  int        index;
    public  string     str;
    private ulong      tempOffsetPos; // Temp spot to place the position we write this object, so we can update the offset later after we write the string.

    public static ResourceInfo Read(BinaryReader reader, ReDataFile file, int index) {
        var userDataInfo = new ResourceInfo {
            file  = file,
            index = index
        };
        var offset = reader.ReadUInt64();
        userDataInfo.str = reader.ReadNullTermWString((long) offset);
        return userDataInfo;
    }

    public void Write(BinaryWriter writer) {
        // It's just an offset, but we can't write it here, need to write it later.
        tempOffsetPos = (ulong) writer.BaseStream.Position;
        writer.Write(0ul);
    }

    public void UpdateWrite(BinaryWriter writer) {
        writer.WriteCurrentPositionAtOffset((long) tempOffsetPos);
        writer.WriteNullTermWString(str);
    }

    public override string ToString() {
        return str;
    }
}