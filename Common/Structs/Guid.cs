using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[TypeConverter(typeof(GuidTypeConverter))]
public class Guid : RszObject, ISimpleViaType {
    public System.Guid Value { get; set; }

    public void Read(BinaryReader reader) {
        Value = new(reader.ReadBytes(16));
    }

    public void Write(BinaryWriter writer) {
        writer.Write(Value.ToByteArray());
    }

    public Guid Copy() {
        return new() {
            Value = Value
        };
    }

    public override string ToString() {
        return Value.ToString();
    }
}

public class GuidTypeConverter : TypeConverter {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
        return true;
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) {
        return true;
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {
        return new Guid {Value = System.Guid.Parse((string) value)};
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {
        return value?.ToString();
    }
}