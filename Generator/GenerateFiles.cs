﻿using System.CodeDom;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using Newtonsoft.Json;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Generator.Models;

namespace RE_Editor.Generator;

public partial class GenerateFiles {
    public const  string BASE_GEN_PATH    = @"..\..\..\Generated"; // @"C:\Temp\Gen"
    public const  string BASE_PROJ_PATH   = @"..\..\..";
    public const  string STRUCT_JSON_PATH = $@"{BASE_PROJ_PATH}\Dump-Parser\Output\{PathHelper.CONFIG_NAME}\rsz{PathHelper.CONFIG_NAME}.json";
    public const  string ENUM_GEN_PATH    = $@"{BASE_GEN_PATH}\Enums\{PathHelper.CONFIG_NAME}";
    public const  string STRUCT_GEN_PATH  = $@"{BASE_GEN_PATH}\Structs\{PathHelper.CONFIG_NAME}";
    private const string ASSETS_DIR       = $@"{BASE_PROJ_PATH}\RE-Editor\Data\{PathHelper.CONFIG_NAME}\Assets";
    public const  string ENUM_REGEX       = $@"namespace ((?:{ROOT_STRUCT_NAMESPACE}::[^ ]+|{ROOT_STRUCT_NAMESPACE}|via::[^ ]+|via)) {{\s+(?:\/\/ (\[Flags\])\s+)?enum ([^ ]+) ({{[^}}]+}})"; //language=regexp

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    public static readonly List<string> UNSUPPORTED_DATA_TYPES = [ // TODO: Implement support for these.
        "AABB",
        "Area",
        "Capsule",
        "Cylinder",
        "Ellipsoid",
        "Frustum",
        "KeyFrame",
        "LineSegment",
        "OBB",
        "Rect",
        "Rect3D",
        "Sphere",
        "Torus",
        "Triangle",
    ];

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    public static readonly List<string> UNSUPPORTED_OBJECT_TYPES = [ // TODO: Implement support for these.
        "System.Collections.Generic.Dictionary`",
        "System.Collections.Generic.Queue`1<System.Tuple`", // Nested generics.
        "System.Collections.Generic.Queue`1<via.vec3>", // Because this breaks generation and I need a better way of handling generics.
        "via.gui.Panel", // Too long, skip it for now.
#if DD2
        "AISituation",
        "app.anime.EquipAdjustUserData.SizeData`",
        "app.ClassSelector`",
        "app.FilterSettingMediator`",
        "app.GUICharaEditData.PatternParam`",
        "app.lod.LODProcessDefine.ClassSelector`",
#elif MHR
        "snow.data.StmKeyconfigSystem.ConfigCodeSet`",
        "snow.enemy.EnemyCarryChangeTrack`",
        "snow.enemy.EnemyEditStepActionData`",
        "snow.envCreature.EnvironmentCreatureActionController`",
        "snow.shell.EnemyShellManagerBase`",
        "snow.StmDefaultKeyconfigData.EnumSet2`",
        "snow.StmGuiKeyconfigData.EnumItemSystemMessage`",
        "snow.StmGuiKeyconfigData.EnumMessage`",
        "System.Collections.Generic.List`1<snow.enemy.em134.Em", // Nested generics.
#elif RE2
        "app.ropeway.camera.CameraCurveUserData.CurveParamTable`",
        "app.ropeway.CorrespondGroup`",
#elif RE3
        "offline.camera.CameraCurveUserData.CurveParamTable`",
        "offline.CorrespondGroup`",
#elif RE4
        "app.",
        "chainsaw.CameraCurveUserDataParam.CurveParamTable`", // Winds up with `ObservableCollection<System_String>` instead of `ObservableCollection<string>`.
#endif
    ];

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    public static readonly List<string> ALLOWED_GENERIC_TYPES = [
#if DD2
        ".Element<",
        "app.AppEventCatalogBase`",
        "app.BodyScaleFormulaBase`",
        "app.BodyWeightDataBase`",
        "app.Ch221Parameter.StateChangeParameter.ElementDataBase`",
        "app.Ch221Parameter.StateChangeParameter.ElementDataColor`",
        "app.Ch221Parameter.StateChangeParameter.ElementDataFloat`",
        "app.CharacterVariationDataContainer`",
        "app.charaedit.ch000.useredit.LimitSpeciesAndGenderData`",
        "app.charaedit.EditInfluenceMapBase`",
        "app.CutSceneIKModule.EquipmentOffset`",
        "app.JobUniqueParameter.AreaParameterList`",
        "app.JobUniqueParameter.CustomSkillLevelParameter`",
        "app.LocalWindSettings`",
        "app.MaterialInterpolation.Variable`",
        "app.ModuleParametersUserData`",
        "app.retarget.RetargetLodJointSettingBase`",
        "app.SimpleFlightPathTracer`",
        "app.StringUtil.NameHash`",
        "soundlib.SoundSwitchApp`",
#elif MHR
        "snow.camera.CameraUtility.BufferingParam`",
        "snow.eventcut.EventPlayerMediator.FaceMaterialConfig`",
#elif RE2
        "app.ropeway.enemy.userdata.MotionUserDataBase.MotionInfo`",
        "app.ropeway.enemy.userdata.MotionUserDataBase.MotionTable`",
        "app.ropeway.BitFlag`",
        "app.ropeway.DampingSetting`",
        "app.ropeway.ParseBitFlag`",
        "app.ropeway.Percentable`",
        "app.ropeway.PercentValueTable`",
        "app.ropeway.weapon.generator.BombGeneratorUserDataBase`",
        "app.ropeway.weapon.shell.ShellPrefabSetting`",
        "app.ropeway.weapon.shell.RadiateShellUserDataBase`",
#elif RE3
        "offline.DampingSetting`",
        "offline.enemy.userdata.MotionUserDataBase.MotionInfo`",
        "offline.enemy.userdata.MotionUserDataBase.MotionTable`",
        "offline.ParseBitFlag`",
        "offline.Percentable`",
        "offline.PercentValueTable`",
        "offline.weapon.generator.BombGeneratorUserDataBase`",
        "offline.weapon.shell.RadiateShellUserDataBase`",
        "offline.weapon.shell.ShellPrefabSetting`",
#elif RE4
        "chainsaw.AIMapNodeScore.Param`",
        "chainsaw.AppEventCatalogBase`",
        "chainsaw.Ch1b7z0ParamUserData.NumData`",
        "chainsaw.Ch1e0z0ParamUserData.NumData`",
        "chainsaw.Ch1f1z0ParamUserData.NumData`",
        "chainsaw.Ch1f7z0ParamUserData.NumData`",
        "chainsaw.Ch1f8z0ParamUserData.NumData`",
        "chainsaw.Ch1fcz0ParamUserData.NumData`",
        "chainsaw.Ch1fdz0ParamUserData.NumData`",
        "chainsaw.Ch4fez0ParamUserData.NumData`",
        "chainsaw.CustomConditionBase`",
        "chainsaw.NetworkRankingSettingUserdata.BoardNameTable`",
        "chainsaw.SwitchFeatureParameter`",
        "soundlib.SoundStateApp`",
        "soundlib.SoundSwitchApp`",
#endif
    ];

    private static readonly List<uint> GREYLIST = []; // Hashes used in a given location.

    public readonly  Dictionary<string, EnumType>   enumTypes      = [];
    public readonly  Dictionary<string, StructType> structTypes    = [];
    private readonly Dictionary<string, StructJson> structJson     = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(STRUCT_JSON_PATH))!;
    public readonly  Dictionary<uint, uint>         gpCrcOverrides = []; // Because the GP version uses the same hashes, but different CRCs.

    public void Go(string[] args) {
        var useWhitelist = args.Length > 0 && args.Contains("useWhitelist");
        var useGreylist  = args.Length > 0 && args.Contains("useGreylist");
        var dryRun       = args.Length > 0 && args.Contains("dryRun");

        Log("Finding enum placeholders in the struct json.");
        FindAllEnumUnderlyingTypes();

        if (!dryRun) {
            Log("Creating directories.");
            Directory.CreateDirectory(ENUM_GEN_PATH);
            Directory.CreateDirectory(STRUCT_GEN_PATH);
            Directory.CreateDirectory(ASSETS_DIR);

            Log("Removing existing generated files.");
            CleanupGeneratedFiles(ENUM_GEN_PATH);
            CleanupGeneratedFiles(STRUCT_GEN_PATH);
        }

        Log("Parsing enums.");
        ParseEnums();
        Log("Parsing structs.");
        ParseStructs();

        if (useWhitelist) {
            FilterWhitelisted();
        }
        if (useGreylist) {
            FindAllHashesBeingUsed();
            FilterGreylisted();
            FindCrcOverrides();
        }
        if (useWhitelist || useGreylist) {
            UpdateUsingCounts();
            RemoveUnusedTypes();
            UpdateButtons();
        }

        Log($"Generating {enumTypes.Count} enums, {structTypes.Count} structs.");
        GenerateEnums(dryRun);
        Log("Enums written.");
        GenerateStructs(dryRun);
        Log("Structs written.");
        WriteStructInfo(dryRun);
        Log("Struct info written.");
    }

    private void WriteStructInfo(bool dryRun) {
        var structInfo = new Dictionary<uint, StructJson>();
        foreach (var (hashString, @struct) in structJson) {
            var hash = uint.Parse(hashString, NumberStyles.HexNumber);
            structInfo[hash] = @struct;
        }
        if (!dryRun) {
            Directory.CreateDirectory(ASSETS_DIR);
            File.WriteAllText($@"{ASSETS_DIR}\STRUCT_INFO.json", JsonConvert.SerializeObject(structInfo, Formatting.Indented));
            File.WriteAllText($@"{ASSETS_DIR}\GP_CRC_OVERRIDE_INFO.json", JsonConvert.SerializeObject(gpCrcOverrides, Formatting.Indented));
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

            enumTypes[name] = new(name, structInfo.name, typeString);
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
        var enumHpp = File.ReadAllText(PathHelper.ENUM_HEADER_PATH);
        var regex   = new Regex(ENUM_REGEX, RegexOptions.Singleline);
        var matches = regex.Matches(enumHpp);
#pragma warning disable IDE0220 // Add explicit cast
        foreach (Match match in matches) {
            var hppName = $"{match.Groups[1].Value}::{match.Groups[3].Value}";
            if (hppName.Contains('<') || hppName.Contains('`')) continue;
            var name     = hppName.ToConvertedTypeName();
            var contents = match.Groups[4].Value;
            var isFlags  = match.Groups[2].Success;
            if (name != null && enumTypes.TryGetValue(name, out var enumType)) {
                if (contents.Contains("= -")) {
                    enumType.type = enumType.type switch {
                        "ushort" => "short",
                        "uint" => "int",
                        "ulong" => "long",
                        "byte" => "sbyte",
                        _ => enumType.type
                    };
                }
                enumType.Contents = contents;
                enumType.isFlags  = isFlags;
            }
        }
#pragma warning restore IDE0220 // Add explicit cast
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
            if (structInfo.name?.StartsWith("System.Action`") == true) {
            }

            // ReSharper disable once GrammarMistakeInComment
            // The dump has empty array entries like `.Element[[`, but we transform the field type to `.Element<` which bypasses these empty structs.
            if (structInfo.name?.Contains("[[") == true) continue;

            if (!IsStructNameValid(structInfo.name)) continue;
            // Also ignore structs that are just enum placeholders.
            if (structInfo.fields is [{name: "value__"}]) continue;

            if (structInfo.name!.ToLower() == "via.prefab") {
                structInfo.fields![0].name = "Enabled";
                structInfo.fields![1].name = "Name";
            }

            // Ignore the 'via.thing' placeholders.
            if (structInfo.name!.GetViaType() != null) continue;
            var     name   = structInfo.name.ToConvertedTypeName()!;
            string? parent = null;
            if (IsStructNameValid(structInfo.parent) && structInfo.parent?.StartsWith("via") != true) {
                parent = structInfo.parent.ToConvertedTypeName();
            }
            var structType = new StructType(name, parent, hash, structInfo);
            structTypes[name] = structType;
        }

        foreach (var key in structTypes.Keys.OrderBy(s => s)) {
            this.structTypes[key] = structTypes[key];
        }
    }

    private static bool IsStructNameValid(string? structName) {
        var isBadName = structName == null
                        || structName.Contains('<')
                        || structName.Contains('>')
                        || structName.Contains('`')
                        || structName.Contains('[')
                        || structName.Contains(']')
                        || structName.Contains("List`")
                        || structName.Contains("Culture=neutral")
                        || structName.StartsWith("System");
        if (structName != null) {
            var isAllowed = structName.StartsWith(ROOT_STRUCT_NAMESPACE)
                            || structName.StartsWith("via")
                            || structName.StartsWith("share")
                            || structName.StartsWith("ace")
                            || structName.StartsWith("soundlib");
            isBadName = !(!isBadName && isAllowed);
            if (isBadName && ALLOWED_GENERIC_TYPES.Any(structName.StartsWith)) isBadName = false;
        }
        return !isBadName;
    }

    /**
     * Increases the useCount of enums/structs marked as whitelisted.
     */
    private void FilterWhitelisted() {
        // Whitelist more commonly used things.
        foreach (var name in WHITELIST.ToList()) {
            WHITELIST.Add(name + "_Param");
            WHITELIST.Add(name + "_Data");
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
#if MHR
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
#elif RE4
        return WHITELIST.Contains(key)
               || key.ContainsIgnoreCase("ItemDefinitionUserData")
               || key.ContainsIgnoreCase("ItemUseResult")
               || key.ContainsIgnoreCase("ItemID");
#else
        return WHITELIST.Contains(key);
#endif
    }

    /**
     * Goes through structs and increases useCounts of enums/structs referenced by other structs.
     */
    private void UpdateUsingCounts() {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var structType in structTypes.Values) {
            if (structType.name.GetViaType() != null) continue;
            if (structType.useCount > 0) {
                structType.UpdateUsingCounts(this, new(structTypes.Count));
            }
        }
    }

    /**
     * Goes through structs and sets button types. Ideally should be done on parents. Call after types are filtered for best performance.
     * Definitely call after updating using counts since this ignores structs not being used.
     */
    private void UpdateButtons() {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var structType in structTypes.Values) {
            if (structType.name.GetViaType() != null) continue;
            if (structType.useCount > 0) {
                structType.UpdateButtons(this, new(structTypes.Count));
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
                      where IsStructNameValid(entry.Value.name)
                      let name = entry.Value.name
                      where !string.IsNullOrEmpty(name)
                      where name.GetViaType() == null
                      let typeName = name?.ToConvertedTypeName()
                      where !enumTypes.ContainsKey(typeName) && !structTypes.ContainsKey(typeName)
                      select entry.Key);
        // Now again to remove invalid/ignored when parsing structs. (Like those which are empty named as some generic list.)
        RemoveStructs(from entry in structJson
                      where !IsStructNameValid(entry.Value.name)
                      select entry.Key);
    }

    private void RemoveStructs(IEnumerable<string> enumerable) {
        foreach (var key in enumerable.ToList()) {
            structJson.Remove(key);
        }
    }

    private static void FindAllHashesBeingUsed() {
        foreach (var path in PathHelper.TEST_PATHS) {
            Console.WriteLine($"Finding all files in: {PathHelper.CHUNK_PATH + path}");
        }

        var allUserFiles = PathHelper.GetCachedFileList(FileListCacheType.USER);
        var count        = allUserFiles.Count;
        Console.WriteLine($"Found {count} files.");

        var now = DateTime.Now;
        Console.WriteLine("");

        for (var i = 0; i < allUserFiles.Count; i++) {
            var newNow = DateTime.Now;
            if (newNow > now.AddSeconds(1)) {
                try {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                } catch (Exception) {
                    // Breaks tests so just ignore for those.
                }
                Console.WriteLine($"Parsed {i}/{count}.");
                now = newNow;
            }

            var file = allUserFiles[i];
            var rsz  = ReDataFile.Read(file, justReadHashes: true);
            var hashes = from instanceInfo in rsz.rsz.instanceInfo
                         select instanceInfo.hash;
            hashes = hashes.Distinct();
            foreach (var hash in hashes) {
                if (GREYLIST.Contains(hash)) continue;
                GREYLIST.Add(hash);
            }
        }

        try {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        } catch (Exception) {
            // Breaks tests so just ignore for those.
        }
        Console.WriteLine($"Parsed {count}/{count}.");
    }

    /**
     * Increases the useCount of enums/structs marked as whitelisted.
     */
    private void FilterGreylisted() {
        // Make a list of structs by hash.
        var structsByHash = new Dictionary<uint, StructType>();
        foreach (var (_, structType) in structTypes) {
            if (structType.name.GetViaType() != null) continue;
            structsByHash[uint.Parse(structType.hash, NumberStyles.HexNumber)] = structType;
        }

        // Include structs for all the target files.
        foreach (var hash in GREYLIST.Where(structsByHash.ContainsKey)) {
            structsByHash[hash].useCount++;
        }
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var structType in structTypes.Values) {
            if (structType.useCount == 0) continue;
            if (structType.name.GetViaType() != null) continue;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structType.structInfo.fields!) {
                if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;

                if (UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
                if (UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;

                var typeName = field.originalType!.ToConvertedTypeName();
                if (typeName == null) continue;

                if (enumTypes.TryGetValue(typeName, out var enumType)) {
                    enumType.useCount++;
                }
            }
        }

#if MHR
        // Because this one doesn't appear in the fields, but we still use it.
        enumTypes["Snow_data_ContentsIdSystem_SubCategoryType"].useCount++;
#endif
    }

    private void FindCrcOverrides() {
        foreach (var path in PathHelper.TEST_PATHS) {
            Console.WriteLine($"Finding all GP files in: {(PathHelper.CHUNK_PATH + path).Replace("STM", "MSG")}");
        }

        var allGpUserFiles = PathHelper.GetCachedFileList(FileListCacheType.USER, msg: true);
        var count          = allGpUserFiles.Count;
        Console.WriteLine($"Found {count} files.");

        var now = DateTime.Now;
        Console.WriteLine("");

        for (var i = 0; i < allGpUserFiles.Count; i++) {
            var newNow = DateTime.Now;
            if (newNow > now.AddSeconds(1)) {
                try {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                } catch (Exception) {
                    // Breaks tests so just ignore for those.
                }
                Console.WriteLine($"Parsed {i}/{count}.");
                now = newNow;
            }

            var file = allGpUserFiles[i];
            var rsz  = ReDataFile.Read(file, justReadHashes: true);
            var instanceInfos = from instanceInfo in rsz.rsz.instanceInfo
                                select instanceInfo;
            instanceInfos = instanceInfos.Distinct();
            foreach (var (hash, crc) in instanceInfos) {
                gpCrcOverrides.TryAdd(hash, crc);
            }
        }

        try {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        } catch (Exception) {
            // Breaks tests so just ignore for those.
        }
        Console.WriteLine($"Parsed {count}/{count}.");
        Console.WriteLine($"Created {gpCrcOverrides.Count} CRC overrides.");
    }

    private void GenerateEnums(bool dryRun) {
        foreach (var enumType in enumTypes.Values) {
            new EnumTemplate(enumType).Generate(dryRun);
        }
    }

    private void GenerateStructs(bool dryRun) {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var structType in structTypes.Values) {
            if (structType.name.GetViaType() != null) continue;
            new StructTemplate(this, structType).Generate(dryRun);
        }
    }

    private static void Log(string msg) {
        Console.WriteLine(msg);
        Debug.WriteLine(msg);
    }
}