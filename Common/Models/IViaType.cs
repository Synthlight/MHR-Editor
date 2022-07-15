using System.IO;

namespace MHR_Editor.Common.Models;

public interface IViaType {
    void Read(BinaryReader  reader);
    void Write(BinaryWriter writer);
}