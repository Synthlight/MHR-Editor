using System.Diagnostics.CodeAnalysis;
using System.IO;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Common.Structs;

#pragma warning disable CS8618

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DateTime : RszObject, IViaType {
    public long Value { get; set; }

    public void Read(BinaryReader reader) {
        Value = reader.ReadInt64();
    }
}