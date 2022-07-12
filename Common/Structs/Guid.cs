using System.Diagnostics.CodeAnalysis;
using System.IO;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Guid : RszObject, IViaType {
    public System.Guid Value { get; set; }

    public void Read(BinaryReader reader) {
        Value = new(reader.ReadBytes(16));
    }
}