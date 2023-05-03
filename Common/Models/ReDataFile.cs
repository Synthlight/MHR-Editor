using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

#pragma warning disable CS8618

namespace RE_Editor.Common.Models;

public class ReDataFile {
    public Magic              magic;
    public int                resourceCount;
    public int                infoCount;
    public ulong              resourceOffset;
    public List<UserDataInfo> userDataInfo; // String names of other files being referenced. MUST MATCH THE INNER ONE IN `RSZ`.
    public RSZ                rsz;

    public static ReDataFile Read(string targetFile, bool justReadHashes = false) {
        var       file   = new ReDataFile();
        using var reader = new BinaryReader(File.OpenRead(targetFile));
        file.magic         = (Magic) reader.ReadUInt32();
        file.resourceCount = reader.ReadInt32();
        var userDataCount = reader.ReadInt32();
        file.infoCount      = reader.ReadInt32();
        file.resourceOffset = reader.ReadUInt64();
        var userDataOffset = reader.ReadUInt64();
        var dataOffset     = reader.ReadUInt64();

        reader.BaseStream.Seek((long) userDataOffset, SeekOrigin.Begin);
        file.userDataInfo = new(userDataCount);
        for (var i = 0; i < userDataCount; i++) {
            file.userDataInfo.Add(UserDataInfo.Read(reader, file, i));
        }

        reader.BaseStream.Seek((long) dataOffset, SeekOrigin.Begin);
        file.rsz = RSZ.Read(reader, justReadHashes);

        return file;
    }

    public void Write(string targetFile, bool testWritePosition = false, bool forGp = true) {
        using var writer = new BinaryWriter(File.OpenWrite(targetFile), Encoding.Unicode);
        Write(writer, testWritePosition, forGp);
    }

    public void Write(BinaryWriter writer, bool testWritePosition = false, bool forGp = true) {
        writer.Write((uint) magic);
        writer.Write(resourceCount);
        writer.Write(userDataInfo.Count);
        writer.Write(infoCount);
        writer.Write(resourceOffset);
        var userDataOffsetPos = writer.BaseStream.Position;
        writer.Write(0ul);
        var dataOffsetPos = writer.BaseStream.Position;
        writer.Write(0ul);

        bool PadTill16() => writer.BaseStream.Position % 16 != 0;

        writer.PadTill(PadTill16);
        writer.WriteValueAtOffset((ulong) writer.BaseStream.Position, userDataOffsetPos);
        foreach (var userData in userDataInfo) {
            userData.Write(writer);
        }
        foreach (var userData in userDataInfo) {
            userData.UpdateWrite(writer);
        }

        var dataOffset = (ulong) writer.BaseStream.Position;
        writer.WriteValueAtOffset(dataOffset, dataOffsetPos);
        rsz.Write(writer, dataOffset, testWritePosition, forGp);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum Magic {
        id_SCN  = 5129043,
        id_PFB  = 4343376,
        id_USR  = 5395285,
        id_RCOL = 1280262994,
        id_mfs2 = 846423661,
        id_BHVT = 1414940738,
        id_uvar = 1918989941
    }
}