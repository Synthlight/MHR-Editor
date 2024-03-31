using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[UsedImplicitly]
public class Quaternion : RszObject, IViaType {
    private uint x;
    public float X {
        get => x;
        set => x = (uint) value;
    }
    private uint y;
    public float Y {
        get => y;
        set => y = (uint) value;
    }
    private uint z;
    public float Z {
        get => z;
        set => z = (uint) value;
    }
    private uint w;
    public float W {
        get => w;
        set => w = (uint) value;
    }

    public void Read(BinaryReader reader) {
        x = reader.ReadUInt32();
        y = reader.ReadUInt32();
        z = reader.ReadUInt32();
        w = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer) {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(w);
    }

    public Quaternion Copy() {
        var obj = base.Copy<Quaternion>();
        obj.x = x;
        obj.y = y;
        obj.z = z;
        obj.w = w;
        return obj;
    }
}