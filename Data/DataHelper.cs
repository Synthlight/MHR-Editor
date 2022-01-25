using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MHR_Editor.Models;
using Newtonsoft.Json;

namespace MHR_Editor.Data;

public static class DataHelper {
    private static readonly bool                         LOAD_FROM_JSON = !Debugger.IsAttached;
    public static readonly  Dictionary<uint, StructJson> STRUCT_INFO;
    public static readonly  Dictionary<uint, string>     ARMOR_NAME_LOOKUP;
    public static readonly  Dictionary<uint, string>     ITEM_NAME_LOOKUP;
    public static readonly  Dictionary<byte, string>     SKILL_NAME_LOOKUP;

    static DataHelper() {
        if (LOAD_FROM_JSON) {
            STRUCT_INFO       = LoadDict<uint, StructJson>(Assets.STRUCT_INFO);
            ARMOR_NAME_LOOKUP = LoadDict<uint, string>(Assets.ARMOR_NAME_LOOKUP);
            ITEM_NAME_LOOKUP  = LoadDict<uint, string>(Assets.ITEM_NAME_LOOKUP);
            SKILL_NAME_LOOKUP = LoadDict<byte, string>(Assets.SKILL_NAME_LOOKUP);
        } else {
            STRUCT_INFO       = new();
            ARMOR_NAME_LOOKUP = new();
            ITEM_NAME_LOOKUP  = new();
            SKILL_NAME_LOOKUP = new();

            ParseFromWiki();
        }
    }

    private static void ParseFromWiki() {
        ParseStructInfo();
        ParseArmorNames();
        ParseItemNames();
        ParseSkillNames();
    }

    private static void ParseStructInfo() {
        var structJson = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(@"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json"));
        Debug.Assert(structJson != null, nameof(structJson) + " != null");
        foreach (var (key, value) in structJson) {
            var hash = uint.Parse(key, NumberStyles.HexNumber);
            STRUCT_INFO[hash] = value;
        }
        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\STRUCT_INFO.json", JsonConvert.SerializeObject(STRUCT_INFO));
    }

    private static void ParseItemNames() {
        var lines = File.ReadAllLines(@"R:\Games\Monster Hunter Rise\MonsterHunterRiseModding.wiki\Item-IDs.md");
        var regex = new Regex(@"^\| \d+ \| (0[a-fA-F0-9]+) \| [^\|]+ \| [^\|]+ \| (?:<COLOR [a-fA-F0-9]+>#Rejected#<\/COLOR>)?([^\|]+) \|");

        foreach (var line in lines) {
            var match = regex.Match(line);
            if (!match.Success) continue;
            var id   = Convert.ToUInt32(match.Groups[1].Value, 16);
            var name = match.Groups[2].Value;
            if (ITEM_NAME_LOOKUP.ContainsKey(id)) continue;
            ITEM_NAME_LOOKUP[id] = name;
        }
        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\ITEM_NAME_LOOKUP.json", JsonConvert.SerializeObject(ITEM_NAME_LOOKUP));
    }

    private static void ParseArmorNames() {
        var lines = File.ReadAllLines(@"R:\Games\Monster Hunter Rise\MonsterHunterRiseModding.wiki\Armor-IDs.md");
        var regex = new Regex(@"^\| (0[a-fA-F0-9]+) \| ([^\|]+) \|");

        foreach (var line in lines) {
            var match = regex.Match(line);
            if (!match.Success) continue;
            var id   = Convert.ToUInt32(match.Groups[1].Value, 16);
            var name = match.Groups[2].Value;
            if (ARMOR_NAME_LOOKUP.ContainsKey(id)) continue;
            ARMOR_NAME_LOOKUP[id] = name;
        }
        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\ARMOR_NAME_LOOKUP.json", JsonConvert.SerializeObject(ARMOR_NAME_LOOKUP));
    }

    private static void ParseSkillNames() {
        var lines = File.ReadAllLines(@"R:\Games\Monster Hunter Rise\MonsterHunterRiseModding.wiki\Skill-IDs.md");
        var regex = new Regex(@"^\| (\d+) \| Pl_EquipSkill_\d+ \| ([^\|]+) \|");

        SKILL_NAME_LOOKUP[0] = "None";

        foreach (var line in lines) {
            var match = regex.Match(line);
            if (!match.Success) continue;
            var id   = Convert.ToByte(match.Groups[1].Value, 10);
            var name = match.Groups[2].Value;
            if (SKILL_NAME_LOOKUP.ContainsKey(id)) continue;
            SKILL_NAME_LOOKUP[id] = name;
        }
        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(SKILL_NAME_LOOKUP));
    }

    private static T Load<T>(byte[] data) {
        var json = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(json);
    }

    private static List<T> LoadList<T>(byte[] data) {
        var json = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<List<T>>(json);
    }

    private static Dictionary<K, V> LoadDict<K, V>(byte[] data) {
        var json = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<Dictionary<K, V>>(json);
    }

    private static byte[] GetAsset(string assetName) {
        return (byte[]) Assets.ResourceManager.GetObject(assetName);
    }
}