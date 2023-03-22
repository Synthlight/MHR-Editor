using System.IO;

namespace RE_Editor.Common.Models;

public interface IViaType {
    void Read(BinaryReader  reader);
    void Write(BinaryWriter writer);
}