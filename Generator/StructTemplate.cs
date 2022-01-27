using MHR_Editor.Common.Models;

namespace MHR_Editor.Generator;

public class StructTemplate {
    public readonly  string                  hash;
    public readonly  StructJson              structInfo;
    private readonly string                  className;
    private readonly string                  filename;
    private readonly StreamWriter            file;
    private readonly Dictionary<string, int> usedNames = new();

    private readonly List<string> hashesToIgnore = new() {
        "6c3de53e", // Armor
        "5ce7e37b", // GreatSword
        "e7bd2c0d", // Decoration
        "ee6b61f7" // Item
    };

    public StructTemplate(string hash, StructJson structInfo) {
        this.hash       = hash;
        this.structInfo = structInfo;

        className = structInfo.name!
                              .ToUpperFirstLetter()
                              .Replace('.', '_');

        filename = $@"R:\Games\Monster Hunter Rise\MHR-Editor\Generated\{className}.cs";
        file     = new(File.Open(filename, FileMode.Create, FileAccess.Write));
    }

    public void Generate() {
        if (hashesToIgnore.Contains(hash)) {
            file.Close();
            File.Delete(filename);
            return;
        }

        WriteUsings();
        WriteClassHeader();
        foreach (var field in structInfo.fields!) {
            if (field.name == null) continue;
            var typeName = GetCSharpType(field.type);
            if (typeName == null) continue;
            WriteProperty(field, typeName);
        }
        WriteClassFooter();
        file.Close();

        if (usedNames.Count == 0) File.Delete(filename);
    }

    private void WriteUsings() {
        file.WriteLine("using System.Collections.ObjectModel;");
        file.WriteLine("using System.Diagnostics.CodeAnalysis;");
        file.WriteLine("using System.Globalization;");
        file.WriteLine("using MHR_Editor.Attributes;");
        file.WriteLine("using MHR_Editor.Data;");
        file.WriteLine("using MHR_Editor.Models.List_Wrappers;");
        file.WriteLine("using MHR_Editor.Models.MHR_Enums;");
    }

    private void WriteClassHeader() {
        file.WriteLine("");
        file.WriteLine("namespace MHR_Editor.Models.Structs;");
        file.WriteLine("");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"InconsistentNaming\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"UnusedMember.Global\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"ClassNeverInstantiated.Global\")]");
        file.WriteLine("[MhrStruct]");
        file.WriteLine($"public partial class {className} : RszObject {{");
        file.WriteLine($"    public static readonly uint HASH = uint.Parse(\"{hash}\", NumberStyles.HexNumber);");
    }

    private void WriteProperty(StructJson.Field field, string typeName) {
        file.WriteLine("");

        if (field.name!.ToLower() == "_id") {
            file.WriteLine("    [ShowAsHex]");
        }

        var newName = field.name;

        while (newName.StartsWith('_')) newName = newName[1..]; // Remove the leading '_'.
        while (newName.EndsWith('_')) newName   = newName[..1]; // Remove the leading '_'.

        newName = newName.ToUpperFirstLetter();
        if (newName == "Index") newName = "_Index";

        if (usedNames.ContainsKey(newName)) {
            usedNames[newName]++;
            newName += usedNames[newName].ToString();
        } else {
            usedNames[newName] = 1;
        }

        file.WriteLine($"    public {typeName} {newName} {{");
        file.WriteLine($"        get => fieldData.getFieldByName(\"{field.name}\").data.GetData<{typeName}>();");
        file.WriteLine($"        set => fieldData.getFieldByName(\"{field.name}\").data = (({typeName}) value).GetBytes();");
        file.WriteLine("    }");
    }

    private void WriteClassFooter() {
        file.WriteLine("}");
    }

    private static string? GetCSharpType(string? reType) {
        return reType switch {
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
            //"String" => "string",
            _ => null
        };
    }
}