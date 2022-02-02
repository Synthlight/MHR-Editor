namespace MHR_Editor.Generator;

public static class Extensions {
    public static string ToUpperFirstLetter(this string source) {
        if (string.IsNullOrEmpty(source)) return string.Empty;
        var letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new(letters);
    }

    public static string ToConvertedTypeName(this string source, bool fixTypes = false) {
        var name = source.ToUpperFirstLetter()
                         .Replace(".", "_")
                         .Replace("::", "_");
        if (fixTypes) {
            name = name.Replace("Cariable", "Carryable")
                       .Replace("Evalution", "Evaluation");
        }
        return name;
    }
}