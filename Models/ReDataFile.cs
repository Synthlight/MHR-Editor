using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MHR_Editor.Models;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ReDataFile {
    public Magic magic;
    public int   resourceCount;
    public int   userDataCount;
    public int   infoCount;
    public ulong resourceInfoTable;
    public ulong userDataTable;
    public ulong dataOffset;
    public RSZ   rsz;

    public static ReDataFile Read(string targetFile) {
        var       file   = new ReDataFile();
        using var reader = new BinaryReader(File.OpenRead(targetFile));
        file.magic             = (Magic) reader.ReadUInt32();
        file.resourceCount     = reader.ReadInt32();
        file.userDataCount     = reader.ReadInt32();
        file.infoCount         = reader.ReadInt32();
        file.resourceInfoTable = reader.ReadUInt64();
        file.userDataTable     = reader.ReadUInt64();
        file.dataOffset        = reader.ReadUInt64();
        reader.BaseStream.Seek((long) file.dataOffset, SeekOrigin.Begin);
        file.rsz = RSZ.Read(reader);
        return file;
    }

    public void Write(string targetFile) {
        using var writer = new BinaryWriter(File.OpenWrite(targetFile));

        writer.Write((uint) magic);
        writer.Write(resourceCount);
        writer.Write(userDataCount);
        writer.Write(infoCount);
        writer.Write(resourceInfoTable);
        writer.Write(userDataTable);
        writer.Write(dataOffset);

        writer.PadTill(dataOffset);

        rsz.Write(writer, dataOffset);
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