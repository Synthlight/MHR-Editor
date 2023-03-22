using System.IO;

namespace RE_Editor.Common.Models;

public interface ICustomReadWrite {
    void Read(BinaryReader  reader);
    void Write(BinaryWriter writer);
}