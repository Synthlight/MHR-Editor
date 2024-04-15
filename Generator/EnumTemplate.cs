using RE_Editor.Generator.Models;

namespace RE_Editor.Generator;

public class EnumTemplate(EnumType enumType) {
    public readonly EnumType enumType = enumType;

    public void Generate(bool dryRun) {
        var       filename = $@"{GenerateFiles.ENUM_GEN_PATH}\{enumType.name}.cs";
        using var file     = new StreamWriter(dryRun ? new MemoryStream() : File.Open(filename, FileMode.Create, FileAccess.Write));

        file.WriteLine("// ReSharper disable All");
        file.WriteLine("using System.Diagnostics.Contracts;");
        file.WriteLine("");
        file.WriteLine("namespace RE_Editor.Models.Enums;");
        file.WriteLine("");
        if (enumType.isFlags) {
            file.WriteLine("[Flags]");
        }
        file.WriteLine($"// {enumType.originalName}");
        file.Write($"public enum {enumType.name} : {enumType.type} ");
        file.Write(enumType.Contents);
        if (enumType.isFlags) {
            WriteExtensions(file);
        }
        file.Close();
    }

    private void WriteExtensions(TextWriter file) {
        file.WriteLine("");
        file.WriteLine("");
        file.WriteLine($"public static class {enumType.name}Extensions {{");
        file.WriteLine("    [Pure]");
        file.WriteLine($"    public static IEnumerable<{enumType.name}> GetFlags(this {enumType.name} values) {{");
        file.WriteLine($"        return from {enumType.name} entry in Enum.GetValues(typeof({enumType.name}))");
        file.WriteLine("               where (values & entry) != 0");
        file.WriteLine("               select entry;");
        file.WriteLine("    }");
        file.Write("}");
    }
}