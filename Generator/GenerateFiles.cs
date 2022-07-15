using System.CodeDom;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Generator.Models;
using Microsoft.CSharp;
using Newtonsoft.Json;

namespace MHR_Editor.Generator;

public class GenerateFiles {
    public const string BASE_GEN_PATH    = @"..\..\..\Generated"; // @"C:\Temp\Gen"
    public const string BASE_PROJ_PATH   = @"..\..\..";
    public const string ENUM_GEN_PATH    = $@"{BASE_GEN_PATH}\Enums";
    public const string STRUCT_GEN_PATH  = $@"{BASE_GEN_PATH}\Structs";
    public const string STRUCT_JSON_PATH = @"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json";
    public const string ENUM_HEADER_PATH = @"C:\SteamLibrary\common\MonsterHunterRise\Enums_Internal.hpp";

    private static readonly List<string> WHITELIST = new() {
        "Snow_data_ArmorBaseUserData",
        "Snow_data_ContentsIdSystem_ItemId",
        "Snow_data_ContentsIdSystem_SubCategoryType",
        "Snow_data_DangoBaseUserData",
        "Snow_data_DataDef_PlEquipSkillId",
        "Snow_data_DecorationsBaseUserData",
        "Snow_data_ItemUserData",
        "Snow_data_NormalLvBuffCageBaseUserData",
        "Snow_data_OtAirouArmorBaseUserData",
        "Snow_data_OtDogArmorBaseUserData",
        "Snow_data_OtDogWeaponBaseUserData",
        "Snow_data_OtWeaponBaseUserData",
        "Snow_data_PlEquipSkillBaseUserData",
        "Snow_data_PlHyakuryuSkillBaseUserData",
        "Snow_enemy_em134_Em134_00UniqueData", // Nested generics.
        "Snow_envCreature_Ec019Trajectory_TimeEffectSetting", // Nested generics.
        "Snow_equip_BowBaseUserData",
        "Snow_equip_ChargeAxeBaseUserData",
        "Snow_equip_DualBladesBaseUserData",
        "Snow_equip_GreatSwordBaseUserData",
        "Snow_equip_GunLanceBaseUserData",
        "Snow_equip_HammerBaseUserData",
        "Snow_equip_HeavyBowgunBaseUserData",
        "Snow_equip_HornBaseUserData",
        "Snow_equip_InsectBaseUserData",
        "Snow_equip_InsectGlaiveBaseUserData",
        "Snow_equip_LanceBaseUserData",
        "Snow_equip_LightBowgunBaseUserData",
        "Snow_equip_LongSwordBaseUserData",
        "Snow_equip_OtOverwearBaseUserData_Param",
        "Snow_equip_OtOverwearRecipeUserData_Param",
        "Snow_equip_PlOverwearBaseUserData",
        "Snow_equip_ShortSwordBaseUserData",
        "Snow_equip_SlashAxeBaseUserData",
        "Snow_fallingObject_FallingObjectPlayerHeavyBowgunExtraCartridgeUserData",
        "Snow_npc_fsm_action_NpcFsmAction_StopNavigation", // Via_vec3? This one skipped the via type checking.
        "Snow_player_PlayerUserDataBow",
    };

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly List<string> UNSUPPORTED_DATA_TYPES = new() { // TODO: Implement support for these.
        "AABB",
        "Area",
        "Capsule",
        "Cylinder",
        "Ellipsoid",
        "Frustum",
        "KeyFrame",
        "LineSegment",
        "Mat4",
        "OBB",
        "Rect",
        "Rect3D",
        "Sphere",
        "Torus",
        "Triangle",
        "UserData",
    };

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly List<string> UNSUPPORTED_OBJECT_TYPES = new() { // TODO: Implement support for these.
        "snow.camera.CameraUtility.BufferingParam`",
        "snow.data.StmKeyconfigSystem.ConfigCodeSet`",
        "snow.shell.Em", // There's one for each monster variant. Exclude the whole shebang for now.
        "snow.enemy.EnemyCarryChangeTrack`",
        "snow.enemy.EnemyEditStepActionData`",
        "snow.envCreature.EnvironmentCreatureActionController`",
        "snow.eventcut.EventPlayerMediator.FaceMaterialConfig`",
        "snow.StmDefaultKeyconfigData.EnumSet2`",
        "snow.StmGuiKeyconfigData.EnumItemSystemMessage`",
        "snow.StmGuiKeyconfigData.EnumMessage`",
        "System.Collections.Generic.Dictionary`",
        "System.Collections.Generic.List`1<snow.enemy.em134.Em", // Nested generics.
        "System.Collections.Generic.Queue`1<System.Tuple`", // Nested generics.
        "System.Collections.Generic.Queue`1<via.vec3>", // Because this breaks generation and I need a better way of handling generics.
    };

    public readonly  Dictionary<string, EnumType>   enumTypes   = new();
    public readonly  Dictionary<string, StructType> structTypes = new();
    private readonly Dictionary<string, StructJson> structJson  = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(STRUCT_JSON_PATH))!;

    public void Go(string[] args) {
        var useWhitelist = args.Length > 0 && args.Contains("useWhitelist");
        var dryRun       = args.Length > 0 && args.Contains("dryRun");

        Console.WriteLine("Finding enum placeholders in the struct json.");
        FindAllEnumUnderlyingTypes();

        if (!dryRun) {
            Console.WriteLine("Removing existing generated files.");
            CleanupGeneratedFiles(ENUM_GEN_PATH);
            CleanupGeneratedFiles(STRUCT_GEN_PATH);
        }

        Console.WriteLine("Parsing enums.");
        ParseEnums();
        Console.WriteLine("Parsing structs.");
        ParseStructs();

        if (useWhitelist) {
            FilterWhitelisted();
            UpdateUsingCounts();
            RemoveUnusedTypes();
        }

        Console.WriteLine($"Generating {enumTypes.Count} enums, {structTypes.Count} structs.");
        GenerateEnums(dryRun);
        Console.WriteLine("Enums written.");
        GenerateStructs(dryRun);
        Console.WriteLine("Structs written.");
        WriteStructInfo(dryRun);
        Console.WriteLine("Struct info written.");
    }

    private void WriteStructInfo(bool dryRun) {
        var structInfo = new Dictionary<uint, StructJson>();
        foreach (var (hashString, @struct) in structJson) {
            var hash = uint.Parse(hashString, NumberStyles.HexNumber);
            structInfo[hash] = @struct;
        }
        if (!dryRun) {
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\STRUCT_INFO.json", JsonConvert.SerializeObject(structInfo, Formatting.Indented));
        }
    }

    /**
     * Parses the struct json looking for placeholder 'structs' which are most likely enums.
     */
    private void FindAllEnumUnderlyingTypes() {
        var compiler  = new CSharpCodeProvider();
        var enumTypes = new Dictionary<string, EnumType>();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var (_, structInfo) in structJson) {
            if (structInfo.name == null
                || structInfo.name.Contains('<')
                || structInfo.name.Contains('`')
                || structInfo.name.StartsWith("System")
                || structInfo.fields?.Count == 0) continue;

            var field = structInfo.fields?[0];

            // We only want the enum placeholders.
            if (structInfo.fields?.Count != 1 || field?.name != "value__") continue;

            var name       = structInfo.name.ToConvertedTypeName()!;
            var boxedType  = GetTypeForName(field.originalType!);
            var type       = new CodeTypeReference(boxedType);
            var typeString = compiler.GetTypeOutput(type);

            enumTypes[name] = new(name, typeString);
        }

        foreach (var key in enumTypes.Keys.OrderBy(s => s)) {
            this.enumTypes[key] = enumTypes[key];
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

    private void ParseEnums() {
        PrepTemplatesForFoundTypes();
        CleanupEnumsWithMissingTemplates();
    }

    /**
     * Goes through the enum header file and preps templates to generate the enum code file.
     */
    private void PrepTemplatesForFoundTypes() {
        var enumHpp = File.ReadAllText(ENUM_HEADER_PATH);
        var regex   = new Regex(@"namespace ((?:snow::[^ ]+|snow|via::[^ ]+|via)) {\s+enum ([^ ]+) ({[^}]+})", RegexOptions.Singleline);
        var matches = regex.Matches(enumHpp);
        foreach (Match match in matches) {
            var hppName = $"{match.Groups[1].Value}::{match.Groups[2].Value}";
            if (hppName.Contains('<') || hppName.Contains('`')) continue;
            var name     = hppName.ToConvertedTypeName();
            var contents = match.Groups[3].Value;
            if (name != null && enumTypes.ContainsKey(name)) {
                enumTypes[name].Contents = contents;
            }
        }
    }

    /**
     * Removes all the remaining enums that didn't get a generation template.
     */
    private void CleanupEnumsWithMissingTemplates() {
        enumTypes.Keys
                 .Where(key => enumTypes[key].Contents == null)
                 .ToList()
                 .ForEach(key => enumTypes.Remove(key));
    }

    private void ParseStructs() {
        var structTypes = new Dictionary<string, StructType>();

        foreach (var (hash, structInfo) in structJson) {
            if (!IsStructNameValid(structInfo)) continue;
            // Also ignore structs that are just enum placeholders.
            if (structInfo.fields?.Count == 1 && structInfo.fields[0].name == "value__") continue;
            // Ignore the 'via.thing' placeholders.
            if (structInfo.name!.GetViaType() != null) continue;
            var name       = structInfo.name.ToConvertedTypeName()!;
            var structType = new StructType(name, hash, structInfo);
            structTypes[name] = structType;
        }

        foreach (var key in structTypes.Keys.OrderBy(s => s)) {
            this.structTypes[key] = structTypes[key];
        }
    }

    private static bool IsStructNameValid(StructJson structInfo) {
        return !(structInfo.name == null
                 || structInfo.name.Contains('<')
                 || structInfo.name.Contains('>')
                 || structInfo.name.Contains('`')
                 || structInfo.name.Contains('[')
                 || structInfo.name.Contains(']')
                 || structInfo.name.Contains("List`")
                 || structInfo.name.Contains("Culture=neutral")
                 || structInfo.name.StartsWith("System")
                 || !structInfo.name.StartsWith("snow") && !structInfo.name.StartsWith("via"));
    }

    /**
     * Increases the useCount of enums/structs marked as whitelisted.
     */
    private void FilterWhitelisted() {
        // Whitelist more commonly used things.
        foreach (var name in WHITELIST.ToList()) {
            WHITELIST.Add(name + "_Param");
        }
        // Make sure to keep whitelisted enums/structs.
        enumTypes.Keys
                 .Where(IsWhitelisted)
                 .ToList()
                 .ForEach(key => enumTypes[key].useCount++);
        structTypes.Keys
                   .Where(IsWhitelisted)
                   .ToList()
                   .ForEach(key => structTypes[key].useCount++);
    }

    private static bool IsWhitelisted(string key) {
        return WHITELIST.Contains(key)
               || key.ContainsIgnoreCase("ContentsIdSystem")
               || key.ContainsIgnoreCase("Snow_data_DataDef")
               || key.ContainsIgnoreCase("ProductUserData")
               || key.ContainsIgnoreCase("ChangeUserData")
               || key.ContainsIgnoreCase("ProcessUserData")
               || key.ContainsIgnoreCase("RecipeUserData")
               || key.ContainsIgnoreCase("PlayerUserData")
               || key.ContainsIgnoreCase("Snow_equip")
               || key.ContainsIgnoreCase("Snow_data")
               || key.ContainsIgnoreCase("Snow_player");
    }

    /**
     * Goes through structs and increases useCounts of enums/structs referenced by other structs.
     */
    private void UpdateUsingCounts() {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var structType in structTypes.Values) {
            if (structType.useCount > 0) {
                structType.UpdateUsingCounts(this);
            }
        }
    }

    /**
     * Goes through enums/structs and removes ones that aren't referenced by any struct.
     */
    private void RemoveUnusedTypes() {
        enumTypes.Keys
                 .Where(key => enumTypes[key].useCount == 0)
                 .ToList()
                 .ForEach(key => enumTypes.Remove(key));
        structTypes.Keys
                   .Where(key => structTypes[key].useCount == 0)
                   .ToList()
                   .ForEach(key => structTypes.Remove(key));
        // Remove the struct data we're not using from the struct info.
        RemoveStructs(from entry in structJson
                      where IsStructNameValid(entry.Value)
                      let name = entry.Value.name
                      where !string.IsNullOrEmpty(name)
                      let typeName = name?.ToConvertedTypeName()
                      where !enumTypes.ContainsKey(typeName) && !structTypes.ContainsKey(typeName)
                      select entry.Key);
        // Now again to remove invalid/ignored when parsing structs. (Like those which are empty named as some generic list.)
        RemoveStructs(from entry in structJson
                      where !IsStructNameValid(entry.Value)
                      select entry.Key);
    }

    private void RemoveStructs(IEnumerable<string> enumerable) {
        foreach (var key in enumerable.ToList()) {
            structJson.Remove(key);
        }
    }

    private void GenerateEnums(bool dryRun) {
        foreach (var enumType in enumTypes.Values) {
            new EnumTemplate(enumType).Generate(dryRun);
        }
    }

    private void GenerateStructs(bool dryRun) {
        foreach (var structType in structTypes.Values) {
            new StructTemplate(this, structType).Generate(dryRun);
        }
    }
}