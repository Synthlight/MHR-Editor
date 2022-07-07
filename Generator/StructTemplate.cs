﻿using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models;
using MHR_Editor.Generator.Models;

namespace MHR_Editor.Generator;

public class StructTemplate {
    public readonly  string                  hash;
    public readonly  StructJson              structInfo;
    private readonly string                  className;
    private readonly Dictionary<string, int> usedNames    = new();
    private readonly List<ListForInit>       fieldsToInit = new();
    private          int                     sortOrder    = 1000;

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
        foreach (var field in structInfo.fields!) {
            if (field.name == null) continue;
            var primitiveName = field.GetCSharpType();
            if (primitiveName != null) {
                WriteProperty(file, field, primitiveName);
                continue;
            }
            var enumType = field.GetEnumType(); // No discernible difference between struct and enum here.
            if (enumType != null && (Program.ENUM_TYPES.ContainsKey(enumType) || Program.STRUCT_TYPES.ContainsKey(enumType))) {
                WriteProperty(file, field, enumType);
            }
        }
        WriteInit(file);
        WritePreWrite(file);
        WriteClassFooter(file);
        file.Close();

        if (usedNames.Count == 0) File.Delete(filename);
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
        file.WriteLine($"    public static readonly uint HASH = uint.Parse(\"{hash}\", NumberStyles.HexNumber);");
    }

    private void WriteProperty(TextWriter file, StructJson.Field field, string typeName) {
        var newName      = field.name!;
        var enumType     = field.GetEnumType()?.Replace("[]", "");
        var buttonType   = GetButtonType(field);
        var isObjectType = field.type == "Object";

        while (newName.StartsWith('_')) newName = newName[1..]; // Remove the leading '_'.
        while (newName.EndsWith('_')) newName   = newName[..1]; // Remove the trailing '_'.

        newName = newName.ToConvertedTypeName(true);
        if (newName == "Index") newName = "_Index";

        if (usedNames.ContainsKey(newName)) {
            usedNames[newName]++;
            newName += usedNames[newName].ToString();
        } else {
            usedNames[newName] = 1;
        }

        file.WriteLine("");

        if (field.name!.ToLower() == "_id") {
            file.WriteLine("    [ShowAsHex]");
        }

        if (field.array) {
            file.WriteLine($"    [SortOrder({sortOrder})]");
            if (buttonType != null) {
                file.WriteLine($"    [DataSource(DataSourceType.{buttonType})]");
                foreach (var additionalAttributes in GetAdditionalAttributesForDataSourceType(buttonType)) {
                    file.WriteLine($"    {additionalAttributes}");
                }
                file.WriteLine($"    public ObservableCollection<DataSourceWrapper<{typeName}>> {newName} {{ get; set; }}");
            } else if (isObjectType) {
                file.WriteLine($"    public ObservableCollection<{enumType}> {newName} {{ get; set; }}");
            } else {
                file.WriteLine($"    public ObservableCollection<GenericWrapper<{enumType ?? typeName}>> {newName} {{ get; set; }}");
            }
            fieldsToInit.Add(new(newName, field, buttonType, enumType, typeName, isObjectType));
        } else {
            if (buttonType != null && field.name != "_Id") {
                var lookupName = GetLookupForDataSourceType(buttonType);
                file.WriteLine($"    [SortOrder({sortOrder + 10})]");
                file.WriteLine($"    [DataSource(DataSourceType.{buttonType})]");
                file.WriteLine($"    public {typeName} {newName} {{");
                file.WriteLine($"        get => fieldData.getFieldByName(\"{field.name}\").data.GetData<{typeName}>();");
                file.WriteLine("        set {");
                file.WriteLine($"            if (EqualityComparer<{typeName}>.Default.Equals({newName}, value)) return;");
                file.WriteLine($"            fieldData.getFieldByName(\"{field.name}\").data = value.GetBytes();");
                file.WriteLine($"            OnPropertyChanged(nameof({newName}));");
                file.WriteLine($"            OnPropertyChanged(nameof({newName}_button));");
                file.WriteLine("        }");
                file.WriteLine("    }");
                file.WriteLine("");
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    [DisplayName(\"{newName}\")]");
                file.WriteLine($"    public string {newName}_button => DataHelper.{lookupName}[Global.locale].TryGet((uint) Convert.ChangeType({newName}, TypeCode.UInt32)).ToStringWithId({newName}{(buttonType == DataSourceType.ITEMS ? ", true" : "")});");
            } else {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {enumType ?? typeName} {newName} {{");
                file.WriteLine($"        get => ({enumType ?? typeName}) fieldData.getFieldByName(\"{field.name}\").data.GetData<{typeName}>();");
                file.WriteLine($"        set => fieldData.getFieldByName(\"{field.name}\").data = (({typeName}) value).GetBytes();");
                file.WriteLine("    }");
            }
        }

        sortOrder += 100;
    }

    private void WriteInit(TextWriter file) {
        if (fieldsToInit.Count == 0) return;
        file.WriteLine("");
        file.WriteLine("    protected override void Init() {");
        file.WriteLine("        base.Init();");
        file.WriteLine("");
        foreach (var field in fieldsToInit) {
            if (field.buttonType != null) {
                file.WriteLine($"        {field.newName} = new(fieldData.getFieldByName(\"{field.field.name}\").GetDataAsList<DataSourceWrapper<{field.typeName}>>());");
            } else if (field.isObjectType) {
                file.WriteLine($"        {field.newName} = new(fieldData.getFieldByName(\"{field.field.name}\").GetDataAsList<{field.enumType}>());");
            } else {
                file.WriteLine($"        {field.newName} = new(fieldData.getFieldByName(\"{field.field.name}\").GetDataAsList<GenericWrapper<{field.enumType ?? field.typeName}>>());");
            }
        }
        file.WriteLine("    }");
    }

    private void WritePreWrite(TextWriter file) {
        if (fieldsToInit.Count == 0) return;
        file.WriteLine("");
        file.WriteLine("    protected override void PreWrite() {");
        file.WriteLine("        base.PreWrite();");
        file.WriteLine("");
        foreach (var field in fieldsToInit) {
            file.WriteLine($"        fieldData.getFieldByName(\"{field.field.name}\").SetDataFromList({field.newName});");
        }
        file.WriteLine("    }");
    }

    private void WriteClassFooter(TextWriter file) {
        file.WriteLine("}");
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