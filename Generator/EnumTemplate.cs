namespace MHR_Editor.Generator;

public class EnumTemplate {
    public readonly  string       name;
    public readonly  string       contents;
    private readonly StreamWriter file;

    public EnumTemplate(string name, string contents) {
        this.contents = contents.Replace("        ", "    ")
                                .Replace("    }", "}");
        this.name = name.Replace("::", "_")
                        .ToUpperFirstLetter();

        var filename = $@"R:\Games\Monster Hunter Rise\MHR-Editor\Generated\Enums\{this.name}.cs";
        file = new(File.Open(filename, FileMode.Create, FileAccess.Write));
    }

    public void Generate() {
        file.WriteLine("namespace MHR_Editor.Models.Enums;");
        file.WriteLine("");
        var type = contents.Contains(" -") ? "long" : "ulong";
        file.Write($"public enum {name} : {type} ");
        file.Write(contents);
        file.Close();

        Program.ENUM_NAMES.Add(name);
    }
}