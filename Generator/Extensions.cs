using MHR_Editor.Common.Models;

namespace MHR_Editor.Generator;

public static class Extensions {
    public static string ToUpperFirstLetter(this string source) {
        if (string.IsNullOrEmpty(source)) return string.Empty;
        var letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new(letters);
    }

    public static string ToConvertedTypeName(this string source, bool fixTypos = false) {
        var name = source.ToUpperFirstLetter()
                         .Replace(".", "_")
                         .Replace("::", "_");

        if (int.TryParse(name[0].ToString(), out _)) name = "_" + name; // If it starts with a number.
        while (name.EndsWith("k__BackingField")) name     = name.Substring(1, name.LastIndexOf('>') - 1); // Remove the k__BackingField.

        if (fixTypos) {
            name = name.Replace("Cariable", "Carryable")
                       .Replace("Evalution", "Evaluation");
        }
        return name;
    }

    public static string? GetCSharpType(this StructJson.Field field) {
        return field.type switch {
            "Bool" => "bool",
            "S8" => "sbyte",
            "U8" => "byte",
            "S16" => "short",
            "U16" => "ushort",
            "S32" => "int",
            "U32" => "uint",
            "S64" => "long",
            "U64" => "ulong",
            "F32" => "float",
            "F64" => "double",
            "String" => null,
            "Data" => null,
            // TODO: Generate properties for the object type classes.
            // The problem with this is some of them lead to `via.~` classes which we're skipping so that could be a problem.
            //"Object" => GetEnumType(field),
            //"UserData" => GetEnumType(field),
            //"Color" => GetEnumType(field),
            _ => null
        };
    }

    public static string? GetEnumType(this StructJson.Field field) {
        if (field.name == "_Id"
            || Program.ENUM_TYPES.Keys.Contains(field.name!)
            || field.originalType == null
            || field.originalType.Contains('<')
            || field.originalType.Contains('`')
            || field.originalType.Contains('[')
            || field.originalType.StartsWith("System")
            || (!field.originalType.StartsWith("snow") && !field.originalType.StartsWith("via"))) return null;

        return field.originalType.ToConvertedTypeName();
    }
}