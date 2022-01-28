using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Generator;

public class StructTemplate {
    public readonly  string                  hash;
    public readonly  StructJson              structInfo;
    private readonly string                  className;
    private readonly string                  filename;
    private readonly StreamWriter            file;
    private readonly Dictionary<string, int> usedNames    = new();
    private readonly List<ListForInit>       fieldsToInit = new();
    private          int                     sortOrder    = 100;

    public StructTemplate(string hash, StructJson structInfo) {
        this.hash       = hash;
        this.structInfo = structInfo;

        className = structInfo.name!
                              .ToUpperFirstLetter()
                              .Replace('.', '_');

        filename = $@"R:\Games\Monster Hunter Rise\MHR-Editor\Generated\Structs\{className}.cs";
        file     = new(File.Open(filename, FileMode.Create, FileAccess.Write));
    }

    public void Generate() {
        WriteUsings();
        WriteClassHeader();
        foreach (var field in structInfo.fields!) {
            if (field.name == null) continue;
            var typeName = GetCSharpType(field.type);
            if (typeName == null) continue;
            WriteProperty(field, typeName);
        }
        WriteInit();
        WritePreWrite();
        WriteClassFooter();
        file.Close();

        if (usedNames.Count == 0) File.Delete(filename);
    }

    private void WriteUsings() {
        file.WriteLine("using System.Collections.ObjectModel;");
        file.WriteLine("using System.Diagnostics.CodeAnalysis;");
        file.WriteLine("using System.Globalization;");
        file.WriteLine("using MHR_Editor.Common;");
        file.WriteLine("using MHR_Editor.Common.Attributes;");
        file.WriteLine("using MHR_Editor.Common.Data;");
        file.WriteLine("using MHR_Editor.Common.Models;");
        file.WriteLine("using MHR_Editor.Common.Models.List_Wrappers;");
        file.WriteLine("using MHR_Editor.Generated.Enums;");
    }

    private void WriteClassHeader() {
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

    private void WriteProperty(StructJson.Field field, string typeName) {
        file.WriteLine("");

        if (field.name!.ToLower() == "_id") {
            file.WriteLine("    [ShowAsHex]");
        }

        var newName    = field.name;
        var enumType   = GetEnumType(field.originalType);
        var buttonType = GetButtonType(field.name);

        while (newName.StartsWith('_')) newName = newName[1..]; // Remove the leading '_'.
        while (newName.EndsWith('_')) newName   = newName[..1]; // Remove the trailing '_'.

        newName = newName.ToUpperFirstLetter()
                         .Replace("Cariable", "Carryable")
                         .Replace("Evalution", "Evaluation")
                         .Replace("Hyakuryu", "Rampage");
        if (newName == "Index") newName = "_Index";

        if (usedNames.ContainsKey(newName)) {
            usedNames[newName]++;
            newName += usedNames[newName].ToString();
        } else {
            usedNames[newName] = 1;
        }

        file.WriteLine($"    [SortOrder({sortOrder})]");
        sortOrder += 100;

        if (field.array) {
            var listWrapperType = GetListWrapperForButtonType(buttonType);
            file.WriteLine($"    public ObservableCollection<{listWrapperType}<{typeName}>> {newName} {{ get; set; }}");
            fieldsToInit.Add(new(newName, field, typeName, listWrapperType));
        } else {
            file.WriteLine($"    public {enumType ?? typeName} {newName} {{");
            file.WriteLine($"        get => ({enumType ?? typeName}) fieldData.getFieldByName(\"{field.name}\").data.GetData<{typeName}>();");
            file.WriteLine($"        set => fieldData.getFieldByName(\"{field.name}\").data = (({typeName}) value).GetBytes();");
            file.WriteLine("    }");
        }
    }

    private void WriteInit() {
        if (fieldsToInit.Count == 0) return;
        file.WriteLine("");
        file.WriteLine("    protected override void Init() {");
        file.WriteLine("        base.Init();");
        file.WriteLine("");
        foreach (var field in fieldsToInit) {
            file.WriteLine($"        {field.newName} = new(fieldData.getFieldByName(\"{field.field.name}\").GetDataAsList<{field.listWrapperType}<{field.typeName}>>());");
        }
        file.WriteLine("    }");
    }

    private void WritePreWrite() {
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
            //"String" => "string", // TODO: Handle strings.
            _ => null
        };
    }

    private static string? GetEnumType(string? reOriginalType) {
        return reOriginalType switch {
            "snow.data.DataDef.RareTypes" => "RareTypes",
            "snow.data.GameItemEnum.SexualEquipableFlag" => "SexualEquipableFlag",
            "snow.data.ArmorBuildupData.TableTypes" => "ArmorBuildupData",
            "snow.data.GameItemEnum.SeriesBufType" => "SeriesBufType",
            "snow.equip.PlWeaponElementTypes" => "PlWeaponElementTypes",
            "snow.data.DataDef.ItemTypes" => "ItemTypes",
            "snow.data.GameItemEnum.IconRank" => "IconRank",
            "snow.data.GameItemEnum.SeType" => "SeType",
            "snow.data.GameItemEnum.ItemActionType" => "ItemActionType",
            "snow.data.DataDef.RankTypes" => "RankTypes",
            "snow.data.NormalItemData.ItemGroupTypes" => "ItemGroupTypes",
            _ => null
        };
    }

    private static DataSourceType? GetButtonType(string fieldName) {
        return fieldName switch {
            "_SkillIdList" => DataSourceType.SKILLS,
            "_SkillList" => DataSourceType.SKILLS,
            _ => null
        };
    }

    private static string GetListWrapperForButtonType(DataSourceType? buttonType) {
        return buttonType switch {
            DataSourceType.SKILLS => "SkillId",
            _ => "GenericWrapper"
        };
    }

    public class ListForInit {
        public readonly string           newName;
        public readonly StructJson.Field field;
        public readonly string           typeName;
        public readonly string           listWrapperType;

        public ListForInit(string newName, StructJson.Field field, string typeName, string listWrapperType) {
            this.newName         = newName;
            this.field           = field;
            this.typeName        = typeName;
            this.listWrapperType = listWrapperType;
        }
    }
}