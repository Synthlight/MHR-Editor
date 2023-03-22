using RE_Editor.Generator.Models;

namespace RE_Editor.Generator;

public class EnumTemplate {
    public readonly EnumType enumType;

    public EnumTemplate(EnumType enumType) {
        this.enumType = enumType;
    }

    public void Generate(bool dryRun) {
        var       filename = $@"{GenerateFiles.ENUM_GEN_PATH}\{enumType.name}.cs";
        using var file     = new StreamWriter(dryRun ? new MemoryStream() : File.Open(filename, FileMode.Create, FileAccess.Write));

        file.WriteLine("namespace RE_Editor.Models.Enums;");
        file.WriteLine("");
        file.Write($"public enum {enumType.name} : {enumType.type} ");
        file.Write(enumType.Contents);
        file.Close();
    }
}