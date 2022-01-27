namespace MHR_Editor.Generator;

public static class Extensions {
    public static string ToUpperFirstLetter(this string source) {
        if (string.IsNullOrEmpty(source)) return string.Empty;
        var letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new(letters);
    }
}