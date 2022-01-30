using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models;
using MHR_Editor.Common.Models.List_Wrappers;

namespace MHR_Editor.Generator;

public class StructTemplate {
    public readonly  string                  hash;
    public readonly  StructJson              structInfo;
    private readonly string                  className;
    private readonly string                  filename;
    private readonly StreamWriter            file;
    private readonly Dictionary<string, int> usedNames    = new();
    private readonly List<ListForInit>       fieldsToInit = new();
    private          int                     sortOrder    = 1000;

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
        file.WriteLine("using System.ComponentModel;");
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
        var buttonType = GetButtonType(field, structInfo.name);

        while (newName.StartsWith('_')) newName = newName[1..]; // Remove the leading '_'.
        while (newName.EndsWith('_')) newName   = newName[..1]; // Remove the trailing '_'.

        newName = newName.ToUpperFirstLetter()
                         .Replace("Cariable", "Carryable")
                         .Replace("Evalution", "Evaluation");
        if (newName == "Index") newName = "_Index";

        if (usedNames.ContainsKey(newName)) {
            usedNames[newName]++;
            newName += usedNames[newName].ToString();
        } else {
            usedNames[newName] = 1;
        }

        if (field.array) {
            var listWrapperType = GetListWrapperForButtonType(buttonType);
            file.WriteLine($"    [SortOrder({sortOrder})]");
            file.WriteLine($"    public ObservableCollection<{listWrapperType}<{typeName}>> {newName} {{ get; set; }}");
            fieldsToInit.Add(new(newName, field, typeName, listWrapperType));
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
            "snow.data.ArmorBuildupData.TableTypes" => "ArmorBuildupData",
            "snow.data.DataDef.ItemTypes" => "ItemTypes",
            "snow.data.DataDef.RankTypes" => "RankTypes",
            "snow.data.DataDef.RareTypes" => "RareTypes",
            "snow.data.GameItemEnum.IconRank" => "IconRank",
            "snow.data.GameItemEnum.ItemActionType" => "ItemActionType",
            "snow.data.GameItemEnum.SeriesBufType" => "SeriesBufType",
            "snow.data.GameItemEnum.SeType" => "SeType",
            "snow.data.GameItemEnum.SexualEquipableFlag" => "SexualEquipableFlag",
            "snow.data.NormalItemData.ItemGroupTypes" => "ItemGroupTypes",
            "snow.equip.PlWeaponElementTypes" => "PlWeaponElementTypes",
            _ => null
        };
    }

    private static DataSourceType? GetButtonType(StructJson.Field field, string? structName) {
        DataSourceType? type = field.originalType switch {
            "snow.data.ContentsIdSystem.ItemId" => DataSourceType.ITEMS,
            "snow.data.DataDef.PlHyakuryuSkillId" => DataSourceType.RAMPAGE_SKILLS,
            _ => null
        };

        if (structName?.Contains("Dango") == true || structName?.Contains("Kitchen") == true) {
            type = type ?? field.name switch {
                "_SkillIdList" => DataSourceType.DANGO_SKILLS,
                "_SkillList" => DataSourceType.DANGO_SKILLS,
                _ => null
            };
        }

        type = type ?? field.name switch {
            "_HyakuryuSkillIdList" => DataSourceType.RAMPAGE_SKILLS,
            "_HyakuryuSkillList" => DataSourceType.RAMPAGE_SKILLS,
            "_ItemIdList" => DataSourceType.ITEMS,
            "_RecipeItemIdList" => DataSourceType.ITEMS,
            "_SkillIdList" => DataSourceType.SKILLS,
            "_SkillList" => DataSourceType.SKILLS,
            _ => null
        };

        return type;
    }

    private static string GetListWrapperForButtonType(DataSourceType? buttonType) {
        return buttonType switch {
            DataSourceType.DANGO_SKILLS => nameof(DangoSkillId<int>),
            DataSourceType.ITEMS => nameof(ItemId<int>),
            DataSourceType.RAMPAGE_SKILLS => nameof(RampageSkillId<int>),
            DataSourceType.SKILLS => nameof(SkillId<int>),
            _ => "GenericWrapper"
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