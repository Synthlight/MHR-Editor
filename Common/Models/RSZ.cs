using System.Diagnostics.CodeAnalysis;
using System.IO;

#pragma warning disable CS8618

namespace MHR_Editor.Common.Models;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "NotAccessedField.Global")]
[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class RSZ {
    private long               position;
    public  uint               magic; // We only support: 0x52535A00
    public  uint               version; // We only support: 16
    public  uint               reserved; // Might be padding?
    public  List<uint>         objectInfo; // Outermost data type.
    public  List<InstanceInfo> instanceInfo; // Array type info. (Contents of the 'outermost' data type, given the outermost type is an array.)
    public  List<byte>         userDataInfo = new(); // Unknown, usually the same as userData though.
    public  List<RszObject>    objectData   = new(); // Array data.

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

        rsz.instanceInfo = new(instanceCount);
        for (var i = 0; i < instanceCount; i++) {
            rsz.instanceInfo.Add(InstanceInfo.Read(reader));
        }

        reader.BaseStream.Seek(rsz.position + (long) dataOffset, SeekOrigin.Begin);

        for (var index = 0; index < instanceCount; index++) {
            var hash = rsz.instanceInfo[index].hash;
            if (hash == 0) continue;
            var obj = RszObject.Read(reader, hash);
            obj.Index = index;
            rsz.objectData.Add(obj);
        }

        return rsz;
    }

    public void Write(BinaryWriter writer, ulong rszOffsetStart) {
        writer.Write(magic);
        writer.Write(version);
        writer.Write(objectInfo.Count);
        writer.Write(instanceInfo.Count);
        writer.Write(userDataInfo.Count);
        writer.Write(reserved);

        var instanceOffset = writer.BaseStream.Position;
        writer.Write(0ul);
        var dataOffset = writer.BaseStream.Position;
        writer.Write(0ul);
        var userDataOffset = writer.BaseStream.Position;
        writer.Write(0ul);

        foreach (var obj in objectInfo) {
            writer.Write(obj);
        }

        writer.WriteValueAtOffset((ulong) writer.BaseStream.Position - rszOffsetStart, instanceOffset);

        foreach (var info in instanceInfo) {
            info.Write(writer);
        }

        writer.PadTill(() => writer.BaseStream.Position % 16 != 0);

        writer.WriteValueAtOffset((ulong) writer.BaseStream.Position - rszOffsetStart, dataOffset);
        writer.WriteValueAtOffset((ulong) writer.BaseStream.Position - rszOffsetStart, userDataOffset);

        foreach (var obj in objectData) {
            obj.Write(writer);
        }
    }
}