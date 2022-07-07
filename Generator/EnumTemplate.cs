using MHR_Editor.Generator.Models;

namespace MHR_Editor.Generator;

public class EnumTemplate {
    public readonly EnumType enumType;

    public EnumTemplate(EnumType enumType) {
        this.enumType = enumType;
    }

    public void Generate() {
        var       filename = $@"{Program.ENUM_GEN_PATH}\{enumType.name}.cs";
        using var file     = new StreamWriter(File.Open(filename, FileMode.Create, FileAccess.Write));

        file.WriteLine("namespace MHR_Editor.Models.Enums;");
        file.WriteLine("");
        file.Write($"public enum {enumType.name} : {enumType.type} ");
        file.Write(enumType.Contents);
        file.Close();
    }
}