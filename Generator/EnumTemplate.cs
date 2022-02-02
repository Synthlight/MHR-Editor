namespace MHR_Editor.Generator;

public class EnumTemplate {
    public readonly  string       name;
    public readonly  string       contents;
    private readonly StreamWriter file;

    public EnumTemplate(string name, string contents) {
        this.contents = contents.Replace("        ", "    ")
                                .Replace("    }", "}");
        this.name = name.ToConvertedTypeName();

        var filename = $@"{Program.ENUM_GEN_PATH}\{this.name}.cs";
        file = new(File.Open(filename, FileMode.Create, FileAccess.Write));
    }

    public void Generate() {
        file.WriteLine("namespace MHR_Editor.Models.Enums;");
        file.WriteLine("");
        file.Write($"public enum {name} : {Program.ENUM_TYPES[name]} ");
        file.Write(contents);
        file.Close();

        Program.ENUM_NAMES.Add(name);
    }
}