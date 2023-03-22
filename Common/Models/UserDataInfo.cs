using System.Diagnostics.CodeAnalysis;
using System.IO;

#pragma warning disable CS8618

namespace RE_Editor.Common.Models;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class UserDataInfo {
    public  uint       hash;
    public  uint       crc;
    public  ReDataFile file;
    public  int        index;
    public  string     str => file.rsz.userDataInfo[index].str;
    private ulong      tempOffsetPos; // Temp spot to place the position we write this object so we can update the offset later after we write the string.

    public static UserDataInfo Read(BinaryReader reader, ReDataFile file, int index) {
        var userDataInfo = new UserDataInfo();
        userDataInfo.hash  = reader.ReadUInt32();
        userDataInfo.crc   = reader.ReadUInt32();
        userDataInfo.file  = file;
        userDataInfo.index = index;
        // Ignore the string, just reference the one from `RSZ`.
        reader.ReadUInt64();
        //userDataInfo.str = reader.ReadNullTermWString((long) stringOffset);
        return userDataInfo;
    }

    public void Write(BinaryWriter writer) {
        writer.Write(hash);
        writer.Write(crc);
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