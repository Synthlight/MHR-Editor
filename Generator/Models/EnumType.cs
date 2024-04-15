namespace RE_Editor.Generator.Models;

public class EnumType(string name, string originalName, string type) {
    public readonly string  name         = name;
    public readonly string  originalName = originalName;
    public          string  type         = type;
    public          int     useCount;
    public          bool    isFlags;
    private         string? contents;
    public string? Contents {
        get => contents;
        set =>
            contents = value?.Replace("        ", "    ")
                            .Replace("    }", "}");
    }
}