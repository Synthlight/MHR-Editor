using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Models;
using RE_Editor.Common.Structs;
using RE_Editor.Generator.Models;

namespace RE_Editor.Generator;

public class StructTemplate {
    private readonly GenerateFiles           generator;
    public readonly  string                  hash;
    public readonly  StructJson              structInfo;
    private readonly string                  className;
    private readonly Dictionary<string, int> usedNames = new();
    private          int                     sortOrder = 1000;
    private readonly string?                 parentClass;

    public StructTemplate(GenerateFiles generator, StructType structType) {
        this.generator = generator;
        hash           = structType.hash;
        structInfo     = structType.structInfo;
        className      = structType.name;
        parentClass    = GetParent();
    }

    public void Generate(bool dryRun) {
        var       filename = $@"{GenerateFiles.STRUCT_GEN_PATH}\{className}.cs";
        using var file     = new StreamWriter(dryRun ? new MemoryStream() : File.Open(filename, FileMode.Create, FileAccess.Write));
        WriteUsings(file);
        WriteClassHeader(file);
        if (structInfo.fields != null) {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structInfo.fields) {
                if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
                WriteProperty(file, field);
            }
        }
        WriteClassCreate(file);
        WriteClassCopy(file);
        WriteClassFooter(file);
        file.Flush();
        file.Close();
    }

    private static void WriteUsings(TextWriter file) {
        file.WriteLine("using System.Collections.ObjectModel;");
        file.WriteLine("using System.ComponentModel;");
        file.WriteLine("using System.Diagnostics.CodeAnalysis;");
        file.WriteLine("using System.Globalization;");
        file.WriteLine("using RE_Editor.Common;");
        file.WriteLine("using RE_Editor.Common.Attributes;");
        file.WriteLine("using RE_Editor.Common.Data;");
        file.WriteLine("using RE_Editor.Common.Models;");
        file.WriteLine("using RE_Editor.Common.Models.List_Wrappers;");
        file.WriteLine("using RE_Editor.Common.Structs;");
        file.WriteLine("using RE_Editor.Models.Enums;");
        file.WriteLine("using DateTime = RE_Editor.Common.Structs.DateTime;");
        file.WriteLine("using Guid = RE_Editor.Common.Structs.Guid;");
        file.WriteLine("using Range = RE_Editor.Common.Structs.Range;");
    }

    private void WriteClassHeader(TextWriter file) {
        file.WriteLine("");
        file.WriteLine("#pragma warning disable CS8600");
        file.WriteLine("#pragma warning disable CS8601");
        file.WriteLine("#pragma warning disable CS8602");
        file.WriteLine("#pragma warning disable CS8603");
        file.WriteLine("#pragma warning disable CS8618");
        file.WriteLine("");
        file.WriteLine("namespace RE_Editor.Models.Structs;");
        file.WriteLine("");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"InconsistentNaming\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"UnusedMember.Global\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"ClassNeverInstantiated.Global\")]");
        file.WriteLine("[MhrStruct]");
        file.WriteLine($"public partial class {className} : {parentClass ?? "RszObject"} {{");
        file.WriteLine($"    public {(parentClass == null ? "const" : "new const")} uint HASH = 0x{hash};");
    }

    private void WriteProperty(TextWriter file, StructJson.Field field) {
        if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) return;
        if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) return;

        var newName        = field.name?.ToConvertedFieldName()!;
        var primitiveName  = field.GetCSharpType();
        var typeName       = field.originalType!.ToConvertedTypeName();
        var isPrimitive    = primitiveName != null;
        var isEnumType     = typeName != null && generator.enumTypes.ContainsKey(typeName);
        var buttonType     = GetButtonType(field);
        var isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
        var isObjectType   = field.type is "Object" or "UserData";
        var viaType        = GetViaType(field, isNonPrimitive, typeName, ref isObjectType);

        if (usedNames.ContainsKey(newName)) {
            usedNames[newName]++;
            newName += usedNames[newName].ToString();
        } else {
            usedNames[newName] = 1;
        }

        file.WriteLine("");

        if (field.name!.ToLower() == "_id") {
            file.WriteLine("    [ShowAsHex]");
            isEnumType = false;
        }

        if (field.array) {
            file.WriteLine($"    [SortOrder({sortOrder})]");
            if (buttonType != null) {
                if (primitiveName == null) {
                    throw new InvalidDataException("Button type found but primitiveName is null.");
                }
                file.WriteLine($"    [DataSource(DataSourceType.{buttonType})]");
                foreach (var additionalAttributes in GetAdditionalAttributesForDataSourceType(buttonType)) {
                    file.WriteLine($"    {additionalAttributes}");
                }
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<DataSourceWrapper<{primitiveName}>> {newName} {{ get; set; }}");
            } else if (isObjectType) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<{typeName}> {newName} {{ get; set; }}");
            } else if (isNonPrimitive) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<{viaType}> {newName} {{ get; set; }}");
            } else if (isEnumType) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<GenericWrapper<{typeName}>> {newName} {{ get; set; }}");
            } else if (isPrimitive) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<GenericWrapper<{primitiveName}>> {newName} {{ get; set; }}");
            } else {
                throw new InvalidDataException("Not a primitive, enum, or object array type.");
            }
        } else {
            if (buttonType != null && field.name != "_Id") {
                var lookupName = GetLookupForDataSourceType(buttonType);
                file.WriteLine($"    [SortOrder({sortOrder + 10})]");
                file.WriteLine($"    [DataSource(DataSourceType.{buttonType})]");
                file.WriteLine($"    public {primitiveName} {newName} {{ get; set; }}");
                file.WriteLine("");
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    [DisplayName(\"{newName}\")]");
                file.WriteLine($"    public string {newName}_button => DataHelper.{lookupName}[Global.locale].TryGet((uint) {newName}).ToStringWithId({newName});");
            } else if (isObjectType) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public ObservableCollection<{typeName}> {newName} {{ get; set; }}");
            } else if (isNonPrimitive) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public ObservableCollection<{viaType}> {newName} {{ get; set; }}");
            } else if (isEnumType) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {typeName} {newName} {{ get; set; }}");
            } else if (isPrimitive) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {primitiveName} {newName} {{ get; set; }}");
            } else {
                throw new InvalidDataException("Not a primitive, enum, or object type.");
            }
        }

        sortOrder += 100;
    }

    private static string? GetViaType(StructJson.Field field, bool isNonPrimitive, string? typeName, ref bool isObjectType) {
        string? viaType = null;

        if (isNonPrimitive && !isObjectType) {
            // This makes sure we've implemented the via type during generation.
            viaType = field.type!.GetViaType() ?? throw new NotImplementedException($"Hard-coded type '{field.type}' not implemented.");
        } else if (typeName == "Via_Prefab") {
            viaType      = nameof(Prefab);
            isObjectType = false;
        }
        return viaType;
    }

    private void WriteClassCreate(TextWriter file) {
        if (className.StartsWith("Via_")) return;

        var modifier = parentClass == null ? "" : "new ";

        file.WriteLine("");
        file.WriteLine($"    public static {modifier}{className} Create(RSZ rsz) {{");
        file.WriteLine($"        var obj = Create<{className}>(rsz, HASH);");

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var field in structInfo.fields!) {
            if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
            if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
            if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;

            var newName        = field.name?.ToConvertedFieldName()!;
            var primitiveName  = field.GetCSharpType();
            var typeName       = field.originalType!.ToConvertedTypeName();
            var isPrimitive    = primitiveName != null;
            var isEnumType     = typeName != null && generator.enumTypes.ContainsKey(typeName);
            var isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
            var isObjectType   = field.type is "Object" or "UserData";
            var viaType        = GetViaType(field, isNonPrimitive, typeName, ref isObjectType);

            if (viaType == nameof(System.Guid)) {
                file.WriteLine($"        obj.{newName} = RE_Editor.Common.Structs.Guid.NewIdInAList;");
            }
        }

        file.WriteLine($"        return obj;");
        file.WriteLine("    }");
    }

    private void WriteClassCopy(TextWriter file) {
        if (className.StartsWith("Via_")) return;

        var modifier = parentClass == null ? "virtual" : "override";

        file.WriteLine("");
        file.WriteLine($"    public {modifier} {className} Copy() {{");
        file.WriteLine($"        var obj = base.Copy<{className}>();");

        foreach (var field in structInfo.fields!) {
            if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
            if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
            if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;

            var newName        = field.name?.ToConvertedFieldName()!;
            var primitiveName  = field.GetCSharpType();
            var typeName       = field.originalType!.ToConvertedTypeName();
            var isPrimitive    = primitiveName != null;
            var isEnumType     = typeName != null && generator.enumTypes.ContainsKey(typeName);
            var buttonType     = GetButtonType(field);
            var isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
            var isObjectType   = field.type is "Object" or "UserData";
            var viaType        = GetViaType(field, isNonPrimitive, typeName, ref isObjectType);

            // TODO: Fix generic/dataSource wrappers.

            if (viaType == nameof(System.Guid)) {
                file.WriteLine($"        obj.{newName} = RE_Editor.Common.Structs.Guid.NewIdInAList;"); // Because the field name might be `Guid`.
            } else if ((field.array || isObjectType || isNonPrimitive) && buttonType == null) {
                file.WriteLine($"        obj.{newName} ??= new();");
                file.WriteLine($"        foreach (var x in {newName}) {{");
                file.WriteLine($"            obj.{newName}.Add({(isEnumType ? "x" : "x.Copy()")});");
                file.WriteLine("        }");
            } else {
                file.WriteLine($"        obj.{newName} = {newName};");
            }
        }

        file.WriteLine("        return obj;");
        file.WriteLine("    }");
    }

    private static void WriteClassFooter(TextWriter file) {
        file.Write("}");
    }

    private static DataSourceType? GetButtonType(StructJson.Field field) {
        return field.originalType?.Replace("[]", "") switch {
            _ => null
        };
    }

    private string? GetParent() {
        return className switch {
            _ => null
        };
    }

    private static List<string> GetAdditionalAttributesForDataSourceType(DataSourceType? dataSourceType) {
        return dataSourceType switch {
            _ => new()
        };
    }

    public static string GetLookupForDataSourceType(DataSourceType? dataSourceType) {
        return dataSourceType switch {
            _ => throw new ArgumentOutOfRangeException(dataSourceType.ToString())
        };
    }
}