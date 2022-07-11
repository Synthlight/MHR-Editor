using System.Diagnostics;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models;
using MHR_Editor.Generator.Models;

namespace MHR_Editor.Generator;

public class StructTemplate {
    public readonly  string                  hash;
    public readonly  StructJson              structInfo;
    private readonly string                  className;
    private readonly Dictionary<string, int> usedNames = new();
    private          int                     sortOrder = 1000;

    public StructTemplate(StructType structType) {
        hash       = structType.hash;
        structInfo = structType.structInfo;
        className  = structType.name;
    }

    public void Generate() {
        var       filename = $@"{Program.STRUCT_GEN_PATH}\{className}.cs";
        using var file     = new StreamWriter(File.Open(filename, FileMode.Create, FileAccess.Write));
        WriteUsings(file);
        WriteClassHeader(file);
        if (structInfo.fields != null) {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structInfo.fields) {
                if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
                WriteProperty(file, field);
            }
        }
        WriteClassFooter(file);
        file.Flush();
        file.Close();
    }

    private void WriteUsings(TextWriter file) {
        file.WriteLine("using System.Collections.ObjectModel;");
        file.WriteLine("using System.ComponentModel;");
        file.WriteLine("using System.Diagnostics.CodeAnalysis;");
        file.WriteLine("using System.Globalization;");
        file.WriteLine("using MHR_Editor.Common;");
        file.WriteLine("using MHR_Editor.Common.Attributes;");
        file.WriteLine("using MHR_Editor.Common.Data;");
        file.WriteLine("using MHR_Editor.Common.Models;");
        file.WriteLine("using MHR_Editor.Common.Models.List_Wrappers;");
        file.WriteLine("using MHR_Editor.Common.Structs;");
        file.WriteLine("using MHR_Editor.Models.Enums;");
    }

    private void WriteClassHeader(TextWriter file) {
        file.WriteLine("");
        file.WriteLine("#pragma warning disable CS8600");
        file.WriteLine("#pragma warning disable CS8601");
        file.WriteLine("#pragma warning disable CS8602");
        file.WriteLine("#pragma warning disable CS8603");
        file.WriteLine("#pragma warning disable CS8618");
        file.WriteLine("");
        file.WriteLine("namespace MHR_Editor.Models.Structs;");
        file.WriteLine("");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"InconsistentNaming\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"UnusedMember.Global\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"ClassNeverInstantiated.Global\")]");
        file.WriteLine("[MhrStruct]");
        file.WriteLine($"public partial class {className} : RszObject {{");
        file.WriteLine($"    public const uint HASH = 0x{hash};");
    }

    private void WriteProperty(TextWriter file, StructJson.Field field) {
        var     newName        = field.name?.ToConvertedFieldName()!;
        var     primitiveName  = field.GetCSharpType();
        var     typeName       = field.originalType!.ToConvertedTypeName();
        var     isPrimitive    = primitiveName != null;
        var     isEnumType     = typeName != null && Program.ENUM_TYPES.ContainsKey(typeName);
        var     buttonType     = GetButtonType(field);
        var     isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
        var     isObjectType   = field.type == "Object";
        string? viaType        = null;

        if (isNonPrimitive && !isObjectType) {
            // This makes sure we've implemented the via type during generation.
            viaType = field.type!.GetViaType() ?? throw new NotImplementedException($"Hard-coded type '{field.type}' not implemented.");
        }

        if (usedNames.ContainsKey(newName)) {
            usedNames[newName]++;
            newName += usedNames[newName].ToString();
        } else {
            usedNames[newName] = 1;
        }

        file.WriteLine("");

        if (newName == "RotOffset") {
            Debug.Write("");
        }

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
                file.WriteLine($"    public ObservableCollection<DataSourceWrapper<{primitiveName}>> {newName} {{ get; set; }}");
            } else if (isObjectType) {
                file.WriteLine($"    public ObservableCollection<{typeName}> {newName} {{ get; set; }}");
            } else if (isNonPrimitive) {
                file.WriteLine($"    public ObservableCollection<{viaType}> {newName} {{ get; set; }}");
            } else if (isEnumType) {
                file.WriteLine($"    public ObservableCollection<GenericWrapper<{typeName}>> {newName} {{ get; set; }}");
            } else if (isPrimitive) {
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
                file.WriteLine($"    public string {newName}_button => DataHelper.{lookupName}[Global.locale].TryGet((uint) Convert.ChangeType({newName}, TypeCode.UInt32)).ToStringWithId({newName}{(buttonType == DataSourceType.ITEMS ? ", true" : "")});");
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

    private void WriteClassFooter(TextWriter file) {
        file.Write("}");
    }

    private static DataSourceType? GetButtonType(StructJson.Field field) {
        return field.originalType?.Replace("[]", "") switch {
            "snow.data.ContentsIdSystem.ItemId" => DataSourceType.ITEMS,
            "snow.data.DataDef.PlEquipSkillId" => DataSourceType.SKILLS,
            "snow.data.DataDef.PlHyakuryuSkillId" => DataSourceType.RAMPAGE_SKILLS,
            "snow.data.DataDef.PlKitchenSkillId" => DataSourceType.DANGO_SKILLS,
            _ => null
        };
    }

    private static List<string> GetAdditionalAttributesForDataSourceType(DataSourceType? dataSourceType) {
        return dataSourceType switch {
            DataSourceType.ITEMS => new() {"[ButtonIdAsHex]"},
            _ => new()
        };
    }

    public static string GetLookupForDataSourceType(DataSourceType? dataSourceType) {
        return dataSourceType switch {
            DataSourceType.DANGO_SKILLS => nameof(DataHelper.DANGO_SKILL_NAME_LOOKUP),
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
            DataSourceType.SKILLS => nameof(DataHelper.SKILL_NAME_LOOKUP),
            DataSourceType.RAMPAGE_SKILLS => nameof(DataHelper.RAMPAGE_SKILL_NAME_LOOKUP),
            _ => throw new ArgumentOutOfRangeException(dataSourceType.ToString())
        };
    }

    public class ListForInit {
        public readonly string           newName;
        public readonly StructJson.Field field;
        public readonly DataSourceType?  buttonType;
        public readonly string?          enumType;
        public readonly string           typeName;
        public readonly bool             isObjectType;

        public ListForInit(string newName, StructJson.Field field, DataSourceType? buttonType, string? enumType, string typeName, bool isObjectType) {
            this.newName      = newName;
            this.field        = field;
            this.buttonType   = buttonType;
            this.enumType     = enumType;
            this.typeName     = typeName;
            this.isObjectType = isObjectType;
        }
    }
}