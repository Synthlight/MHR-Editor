using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RE_Editor.Common.Models;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class InstanceInfo {
    public uint hash;
    public uint crc;

    public static InstanceInfo Read(BinaryReader reader) {
        return new() {
            hash = reader.ReadUInt32(),
            crc  = reader.ReadUInt32()
        };
    }

    public void Write(BinaryWriter writer) {
        writer.Write(hash);
        writer.Write(crc);
    }

    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public void Deconstruct(out uint hash, out uint crc) {
        hash = this.hash;
        crc  = this.crc;
    }

    protected bool Equals(InstanceInfo other) {
        return hash == other.hash && crc == other.crc;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((InstanceInfo) obj);
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() {
        return HashCode.Combine(hash, crc);
    }

    public static bool operator ==(InstanceInfo? left, InstanceInfo? right) {
        return Equals(left, right);
    }

    public static bool operator !=(InstanceInfo? left, InstanceInfo? right) {
        return !Equals(left, right);
    }
}