using System.Text.RegularExpressions;
using MHR_Editor.Common.Models;
using Newtonsoft.Json;

namespace MHR_Editor.Generator;

public static class Program {
    public static readonly List<string> ENUM_NAMES = new();

    public static void Main(string[] args) {
        CleanupGeneratedFiles(@"R:\Games\Monster Hunter Rise\MHR-Editor\Generated\Enums");
        CleanupGeneratedFiles(@"R:\Games\Monster Hunter Rise\MHR-Editor\Generated\Structs");
        GenerateEnums();
        GenerateStructs();
    }

    private static void CleanupGeneratedFiles(string path) {
        var files = Directory.EnumerateFiles(path, "Snow*", SearchOption.TopDirectoryOnly);
        foreach (var file in files) {
            File.Delete(file);
        }
    }

    private static void GenerateEnums() {
        var enumHpp = File.ReadAllText(@"C:\SteamLibrary\common\MonsterHunterRise\Enums_Internal.hpp");
        var regex   = new Regex(@"namespace ((?:snow::[^ ]+|snow)) {\s+enum ([^ ]+) ({[^}]+})", RegexOptions.Singleline);
        var matches = regex.Matches(enumHpp);
        foreach (Match match in matches) {
            var name     = $"{match.Groups[1].Value}::{match.Groups[2].Value}";
            var contents = match.Groups[3].Value;
            if (name.Contains('<')
                || name.Contains('`')) continue;
            new EnumTemplate(name, contents).Generate();
        }
    }

    private static void GenerateStructs() {
        var structJson = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(@"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json"))!;
        foreach (var (hash, structInfo) in structJson) {
            if (structInfo.name == null
                || structInfo.name.Contains('<')
                || structInfo.name.Contains('`')
                || structInfo.name.StartsWith("System")
                || !structInfo.name.StartsWith("snow")
                || structInfo.fields?.Count == 0) continue;
            // Also ignore structs that are just enum placeholders.
            if (structInfo.fields?.Count == 1 && structInfo.fields[0].name == "value__") continue;
            new StructTemplate(hash, structInfo).Generate();
        }
    }
}