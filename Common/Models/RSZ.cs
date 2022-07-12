using System.Diagnostics.CodeAnalysis;
using System.IO;

#pragma warning disable CS8618

namespace MHR_Editor.Common.Models;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class RSZ {
    public long                  position;
    public uint                  magic; // We only support: 0x52535A00
    public uint                  version; // We only support: 16
    public uint                  reserved; // Might be padding?
    public List<uint>            objectInfo; // Outermost data type.
    public List<InstanceInfo>    instanceInfo; // Array type info. (Contents of the 'outermost' data type, given the outermost type is an array.)
    public List<RszUserDataInfo> userDataInfo; // String names of other files being referenced. MUST MATCH THE OUTER ONE IN `ReDataFile`.
    public List<RszObject>       objectData; // Array data.

    public static RSZ Read(BinaryReader reader) {
        var rsz = new RSZ();
        rsz.position = reader.BaseStream.Position;
        rsz.magic    = reader.ReadUInt32();
        rsz.version  = reader.ReadUInt32();
        var objectInfoCount = reader.ReadInt32();
        var instanceCount   = reader.ReadInt32();
        var userDataCount   = reader.ReadInt32();
        rsz.reserved = reader.ReadUInt32();
        var instanceOffset = reader.ReadUInt64();
        var dataOffset     = reader.ReadUInt64();
        var userDataOffset = reader.ReadUInt64();

        rsz.objectInfo = new(objectInfoCount);
        for (var i = 0; i < objectInfoCount; i++) {
            rsz.objectInfo.Add(reader.ReadUInt32());
        }

        reader.BaseStream.Seek(rsz.position + (long) instanceOffset, SeekOrigin.Begin);
        rsz.instanceInfo = new(instanceCount);
        for (var i = 0; i < instanceCount; i++) {
            rsz.instanceInfo.Add(InstanceInfo.Read(reader));
        }

        reader.BaseStream.Seek(rsz.position + (long) userDataOffset, SeekOrigin.Begin);
        rsz.userDataInfo = new(userDataCount);
        for (var i = 0; i < userDataCount; i++) {
            rsz.userDataInfo.Add(RszUserDataInfo.Read(reader, rsz));
        }

        reader.BaseStream.Seek(rsz.position + (long) dataOffset, SeekOrigin.Begin);
        rsz.objectData = new(instanceCount);
        for (var index = 0; index < instanceCount; index++) {
            var hash = rsz.instanceInfo[index].hash;
            if (hash == 0) continue;
            var userDataRef = userDataCount >= index ? index - 1 : -1; // index - 1 due to the first instance info always being null.
            var obj         = RszObject.Read(reader, hash, rsz, userDataRef);
            obj.Index = index;
            rsz.objectData.Add(obj);
        }

        if (reader.BaseStream.Position != reader.BaseStream.Length) throw new("Finished reading the file but the position is not at the end.");

        return rsz;
    }

    public void Write(BinaryWriter writer, ulong rszOffsetStart) {
        writer.Write(magic);
        writer.Write(version);
        writer.Write(objectInfo.Count);
        writer.Write(instanceInfo.Count);
        writer.Write(userDataInfo.Count);
        writer.Write(reserved);

        var instanceOffsetPos = writer.BaseStream.Position;
        writer.Write(0ul);
        var dataOffsetPos = writer.BaseStream.Position;
        writer.Write(0ul);
        var userDataOffsetPos = writer.BaseStream.Position;
        writer.Write(0ul);

        foreach (var obj in objectInfo) {
            writer.Write(obj);
        }

        writer.WriteValueAtOffset((ulong) writer.BaseStream.Position - rszOffsetStart, instanceOffsetPos);
        foreach (var info in instanceInfo) {
            info.Write(writer);
        }

        writer.PadTill(() => writer.BaseStream.Position % 16 != 0);
        writer.WriteValueAtOffset((ulong) writer.BaseStream.Position - rszOffsetStart, userDataOffsetPos);
        foreach (var userData in userDataInfo) {
            userData.Write(writer);
        }
        foreach (var userData in userDataInfo) {
            userData.UpdateWrite(writer, rszOffsetStart);
        }

        writer.PadTill(() => writer.BaseStream.Position % 16 != 0);
        writer.WriteValueAtOffset((ulong) writer.BaseStream.Position - rszOffsetStart, dataOffsetPos);
        foreach (var obj in objectData) {
            obj.Write(writer);
        }
    }
}