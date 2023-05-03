using System.CodeDom;
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

public class GenerateFiles {
    public const string BASE_GEN_PATH         = @"..\..\..\Generated"; // @"C:\Temp\Gen"
    public const string BASE_PROJ_PATH        = @"..\..\..";
    public const string ENUM_GEN_PATH         = $@"{BASE_GEN_PATH}\Enums";
    public const string STRUCT_GEN_PATH       = $@"{BASE_GEN_PATH}\Structs";
    public const string ROOT_STRUCT_NAMESPACE = "";
    public const string ENUM_REGEX            = $@"namespace ((?:{ROOT_STRUCT_NAMESPACE}::[^ ]+|{ROOT_STRUCT_NAMESPACE}|via::[^ ]+|via)) {{\s+enum ([^ ]+) ({{[^}}]+}})"; //language=regexp

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static readonly List<string> WHITELIST = new() {
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
        "OBB",
        "Rect",
        "Rect3D",
        "Sphere",
        "Torus",
        "Triangle",
    };

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly List<string> UNSUPPORTED_OBJECT_TYPES = new() { // TODO: Implement support for these.
        "System.Collections.Generic.Dictionary`",
        "System.Collections.Generic.Queue`1<System.Tuple`", // Nested generics.
        "System.Collections.Generic.Queue`1<via.vec3>", // Because this breaks generation and I need a better way of handling generics.
    };

    private static readonly List<uint> GREYLIST = new(); // Hashes used in a given location.

    public readonly  Dictionary<string, EnumType>   enumTypes      = new();
    public readonly  Dictionary<string, StructType> structTypes    = new();
    private readonly Dictionary<string, StructJson> structJson     = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(PathHelper.STRUCT_JSON_PATH))!;
    public readonly  Dictionary<uint, uint>         gpCrcOverrides = new(); // Because the GP version uses the same hashes, but different CRCs.

    public void Go(string[] args) {
        var useWhitelist = args.Length > 0 && args.Contains("useWhitelist");
        var useGreylist  = args.Length > 0 && args.Contains("useGreylist");
        var dryRun       = args.Length > 0 && args.Contains("dryRun");

        Log("Finding enum placeholders in the struct json.");
        FindAllEnumUnderlyingTypes();

        if (!dryRun) {
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
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\STRUCT_INFO.json", JsonConvert.SerializeObject(structInfo, Formatting.Indented));
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\GP_CRC_OVERRIDE_INFO.json", JsonConvert.SerializeObject(gpCrcOverrides, Formatting.Indented));
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
        var enumHpp = File.ReadAllText(PathHelper.ENUM_HEADER_PATH);
        var regex   = new Regex(ENUM_REGEX, RegexOptions.Singleline);
        var matches = regex.Matches(enumHpp);
        foreach (Match match in matches) {
            var hppName = $"{match.Groups[1].Value}::{match.Groups[2].Value}";
            if (hppName.Contains('<') || hppName.Contains('`')) continue;
            var name     = hppName.ToConvertedTypeName();
            var contents = match.Groups[3].Value;
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
            if (structInfo.fields is [{name: "value__"}]) continue;
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
                 || !structInfo.name.StartsWith(ROOT_STRUCT_NAMESPACE) && !structInfo.name.StartsWith("via"));
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
        return WHITELIST.Contains(key);
    }

    /**
     * Goes through structs and increases useCounts of enums/structs referenced by other structs.
     */
    private void UpdateUsingCounts() {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var structType in structTypes.Values) {
            if (structType.name.GetViaType() != null) continue;
            if (structType.useCount > 0) {
                structType.UpdateUsingCounts(this, new());
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
                      where name.GetViaType() == null
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

    private static void FindAllHashesBeingUsed() {
        foreach (var path in PathHelper.TEST_PATHS) {
            Console.WriteLine($"Finding all files in: {PathHelper.CHUNK_PATH + path}");
        }

        var allUserFiles = (from basePath in PathHelper.TEST_PATHS
                            from file in Directory.EnumerateFiles(PathHelper.CHUNK_PATH + basePath, "*.user.2", SearchOption.AllDirectories)
                            where File.Exists(file)
                            select file).ToList();

        var count = allUserFiles.Count;
        Console.WriteLine($"Found {count} files.");

        var now = DateTime.Now;
        Console.WriteLine("");

        for (var i = 0; i < allUserFiles.Count; i++) {
            var newNow = DateTime.Now;
            if (newNow > now.AddSeconds(1)) {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
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

        Console.SetCursorPosition(0, Console.CursorTop - 1);
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
    }

    private void FindCrcOverrides() {
        foreach (var path in PathHelper.TEST_PATHS) {
            Console.WriteLine($"Finding all GP files in: {(PathHelper.CHUNK_PATH + path).Replace("STM", "MSG")}");
        }

        var allGpUserFiles = (from basePath in PathHelper.TEST_PATHS
                              let path = (PathHelper.CHUNK_PATH + basePath).Replace("STM", "MSG")
                              where Directory.Exists(path)
                              from file in Directory.EnumerateFiles(path, "*.user.2", SearchOption.AllDirectories)
                              where File.Exists(file)
                              select file).ToList();

        var count = allGpUserFiles.Count;
        Console.WriteLine($"Found {count} files.");

        var now = DateTime.Now;
        Console.WriteLine("");

        for (var i = 0; i < allGpUserFiles.Count; i++) {
            var newNow = DateTime.Now;
            if (newNow > now.AddSeconds(1)) {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine($"Parsed {i}/{count}.");
                now = newNow;
            }

            var file = allGpUserFiles[i];
            var rsz  = ReDataFile.Read(file, justReadHashes: true);
            var instanceInfos = from instanceInfo in rsz.rsz.instanceInfo
                                select instanceInfo;
            instanceInfos = instanceInfos.Distinct();
            foreach (var (hash, crc) in instanceInfos) {
                if (gpCrcOverrides.ContainsKey(hash)) continue;
                gpCrcOverrides[hash] = crc;
            }
        }

        Console.SetCursorPosition(0, Console.CursorTop - 1);
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