using System.CodeDom;
using System.Globalization;
using System.Text.RegularExpressions;
using MHR_Editor.Common.Models;
using MHR_Editor.Generator.Models;
using Microsoft.CSharp;
using Newtonsoft.Json;

namespace MHR_Editor.Generator;

public static class Program {
    public const            string                         BASE_GEN_PATH    = @"..\..\..\Generated"; // @"C:\Temp\Gen"
    public const            string                         BASE_PROJ_PATH   = @"..\..\..";
    public const            string                         ENUM_GEN_PATH    = $@"{BASE_GEN_PATH}\Enums";
    public const            string                         STRUCT_GEN_PATH  = $@"{BASE_GEN_PATH}\Structs";
    public const            string                         STRUCT_JSON_PATH = @"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json";
    public const            string                         ENUM_HEADER_PATH = @"C:\SteamLibrary\common\MonsterHunterRise\Enums_Internal.hpp";
    public static readonly  Dictionary<string, EnumType>   ENUM_TYPES       = new();
    public static readonly  Dictionary<string, StructType> STRUCT_TYPES     = new();
    private static readonly Dictionary<string, StructJson> STRUCT_JSON      = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(STRUCT_JSON_PATH))!;

    private static readonly List<string> WHITELIST = new() {
        "Snow_data_ArmorBaseUserData_Param",
        "Snow_data_ContentsIdSystem_ItemId",
        "Snow_data_ContentsIdSystem_SubCategoryType",
        "Snow_data_DangoBaseUserData_Param",
        "Snow_data_DataDef_PlEquipSkillId",
        "Snow_data_DecorationsBaseUserData_Param",
        "Snow_data_ItemUserData_Param",
        "Snow_data_NormalLvBuffCageBaseUserData_Param",
        "Snow_data_OtAirouArmorBaseUserData_Param",
        "Snow_data_OtDogArmorBaseUserData_Param",
        "Snow_data_OtDogWeaponBaseUserData_Param",
        "Snow_data_OtWeaponBaseUserData_Param",
        "Snow_data_PlEquipSkillBaseUserData_Param",
        "Snow_equip_BowBaseUserData_Param",
        "Snow_equip_ChargeAxeBaseUserData_Param",
        "Snow_equip_DualBladesBaseUserData_Param",
        "Snow_equip_GreatSwordBaseUserData_Param",
        "Snow_equip_GunLanceBaseUserData_Param",
        "Snow_equip_HammerBaseUserData_Param",
        "Snow_equip_HeavyBowgunBaseUserData_Param",
        "Snow_equip_HornBaseUserData_Param",
        "Snow_equip_InsectBaseUserData_Param",
        "Snow_equip_InsectGlaiveBaseUserData_Param",
        "Snow_equip_LanceBaseUserData_Param",
        "Snow_equip_LightBowgunBaseUserData_Param",
        "Snow_equip_LongSwordBaseUserData_Param",
        "Snow_equip_PlOverwearBaseUserData_Param",
        "Snow_equip_ShortSwordBaseUserData_Param",
        "Snow_equip_SlashAxeBaseUserData_Param",
    };

    public static void Main(string[] args) {
        var useWhitelist     = args.Length > 0 && args.Contains("useWhitelist");
        var ignoreStructInfo = args.Length > 0 && args.Contains("ignoreStructInfo");

        if (!ignoreStructInfo) {
            ParseStructInfo();
        }

        FindAllEnumUnderlyingTypes();

        CleanupGeneratedFiles(ENUM_GEN_PATH);
        CleanupGeneratedFiles(STRUCT_GEN_PATH);

        ParseEnums();
        ParseStructs();

        FilterWhitelisted(useWhitelist);
        UpdateEnumUsingCounts();
        RemoveUnusedEnumTypes();

        GenerateEnums();
        GenerateStructs();
    }

    private static void ParseStructInfo() {
        var structJson = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(STRUCT_JSON_PATH))!;
        var structInfo = new Dictionary<uint, StructJson>();
        foreach (var (key, value) in structJson) {
            var hash = uint.Parse(key, NumberStyles.HexNumber);
            structInfo[hash] = value;
        }
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\STRUCT_INFO.json", JsonConvert.SerializeObject(structInfo, Formatting.Indented));
    }

    /**
     * Parses the struct json looking for placeholder 'structs' which are most likely enums.
     */
    private static void FindAllEnumUnderlyingTypes() {
        var compiler  = new CSharpCodeProvider();
        var enumTypes = new Dictionary<string, EnumType>();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var (_, structInfo) in STRUCT_JSON) {
            if (structInfo.name == null
                || structInfo.name.Contains('<')
                || structInfo.name.Contains('`')
                || structInfo.name.StartsWith("System")
                || structInfo.fields?.Count == 0) continue;

            var field = structInfo.fields?[0];

            // We only want the enum placeholders.
            if (structInfo.fields?.Count != 1 || field?.name != "value__") continue;

            var name       = structInfo.name.ToConvertedTypeName();
            var boxedType  = GetTypeForName(field.originalType!);
            var type       = new CodeTypeReference(boxedType);
            var typeString = compiler.GetTypeOutput(type);

            enumTypes[name] = new(name, typeString);
        }

        foreach (var key in enumTypes.Keys.OrderBy(s => s)) {
            ENUM_TYPES[key] = enumTypes[key];
        }
    }

    private static Type GetTypeForName(string typeName) {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            var type = assembly.GetType(typeName);
            if (type != null) return type;
        }
        throw new InvalidOperationException($"Unable to find type for: {typeName}");
    }

    private static void CleanupGeneratedFiles(string path) {
        var files = Directory.EnumerateFiles(path, "*.cs", SearchOption.TopDirectoryOnly);
        foreach (var file in files) {
            File.Delete(file);
        }
    }

    private static void ParseEnums() {
        PrepTemplatesForFoundTypes();
        CleanupEnumsWithMissingTemplates();
    }

    /**
     * Goes through the enum header file and preps templates to generate the enum code file.
     */
    private static void PrepTemplatesForFoundTypes() {
        var enumHpp = File.ReadAllText(ENUM_HEADER_PATH);
        var regex   = new Regex(@"namespace ((?:snow::[^ ]+|snow|via::[^ ]+|via)) {\s+enum ([^ ]+) ({[^}]+})", RegexOptions.Singleline);
        var matches = regex.Matches(enumHpp);
        foreach (Match match in matches) {
            var hppName = $"{match.Groups[1].Value}::{match.Groups[2].Value}";
            if (hppName.Contains('<') || hppName.Contains('`')) continue;
            var name     = hppName.ToConvertedTypeName();
            var contents = match.Groups[3].Value;
            if (ENUM_TYPES.ContainsKey(name)) {
                ENUM_TYPES[name].Contents = contents;
            }
        }
    }

    /**
     * Removes all the remaining enums that didn't get a generation template.
     */
    private static void CleanupEnumsWithMissingTemplates() {
        ENUM_TYPES.Keys
                  .Where(key => ENUM_TYPES[key].Contents == null)
                  .ToList()
                  .ForEach(key => ENUM_TYPES.Remove(key));
    }

    private static void ParseStructs() {
        var structTypes = new Dictionary<string, StructType>();

        foreach (var (hash, structInfo) in STRUCT_JSON) {
            if (structInfo.name == null
                || structInfo.name.Contains('<')
                || structInfo.name.Contains('`')
                || structInfo.name.Contains('[')
                || structInfo.name.StartsWith("System")
                || !structInfo.name.StartsWith("snow") && !structInfo.name.StartsWith("via")) continue;
            // Also ignore structs that are just enum placeholders.
            if (structInfo.fields?.Count == 1 && structInfo.fields[0].name == "value__") continue;
            var name       = structInfo.name.ToConvertedTypeName();
            var structType = new StructType(name, hash, structInfo);
            structTypes[name] = structType;
        }

        foreach (var key in structTypes.Keys.OrderBy(s => s)) {
            STRUCT_TYPES[key] = structTypes[key];
        }
    }

    /**
     * Goes through structs and adds to the using count of enums.
     */
    private static void UpdateEnumUsingCounts() {
        foreach (var structType in STRUCT_TYPES.Values) {
            structType.UpdateUsingCounts();
        }
    }

    /**
     * Removes structs not in the whitelist if the whitelist is enabled.
     */
    private static void FilterWhitelisted(bool useWhitelist) {
        if (!useWhitelist) return;
        ENUM_TYPES.Keys
                  .Where(key => WHITELIST.Contains(key) || key.Contains("ContentsIdSystem"))
                  .ToList()
                  .ForEach(key => ENUM_TYPES[key].useCount++);
        STRUCT_TYPES.Keys
                    .Where(key => !WHITELIST.Contains(key))
                    .ToList()
                    .ForEach(key => STRUCT_TYPES.Remove(key));
    }

    /**
     * Goes through enums and removes ones that aren't referenced by any struct.
     */
    private static void RemoveUnusedEnumTypes() {
        ENUM_TYPES.Keys
                  .Where(key => ENUM_TYPES[key].useCount == 0)
                  .ToList()
                  .ForEach(key => ENUM_TYPES.Remove(key));
    }

    private static void GenerateEnums() {
        foreach (var enumType in ENUM_TYPES.Values) {
            new EnumTemplate(enumType).Generate();
        }
    }

    private static void GenerateStructs() {
        foreach (var structType in STRUCT_TYPES.Values) {
            new StructTemplate(structType).Generate();
        }
    }
}