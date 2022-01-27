using MHR_Editor.Common.Models;
using Newtonsoft.Json;

namespace MHR_Editor.Generator;

public static class Program {
    public static void Main(string[] args) {
        var structJson = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(@"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json"))!;
        foreach (var (hash, structInfo) in structJson) {
            if (structInfo.name == null) continue;
            if (structInfo.name.Contains('<')) continue;
            if (structInfo.name.StartsWith("System")) continue;
            if (!structInfo.name.StartsWith("snow")) continue;
            if (structInfo.fields?.Count == 0) continue;
            new StructTemplate(hash, structInfo).Generate();
        }
    }
}