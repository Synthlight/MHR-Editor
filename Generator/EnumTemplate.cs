using RE_Editor.Generator.Models;

namespace RE_Editor.Generator;

public class EnumTemplate(EnumType enumType) {
    public readonly EnumType enumType = enumType;

    public void Generate(bool dryRun) {
        var       filename = $@"{GenerateFiles.ENUM_GEN_PATH}\{enumType.name}.cs";
        using var file     = new StreamWriter(dryRun ? new MemoryStream() : File.Open(filename, FileMode.Create, FileAccess.Write));

        file.WriteLine("// ReSharper disable All");
        file.WriteLine("namespace RE_Editor.Models.Enums;");
        file.WriteLine("");
        if (enumType.isFlags) {
            file.Write("[Flags]");
        }
        file.Write($"public enum {enumType.name} : {enumType.type} ");
        file.Write(enumType.Contents);
        file.Close();
    }
}