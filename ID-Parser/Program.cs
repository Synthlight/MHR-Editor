using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using MHR_Editor.Models;
using Newtonsoft.Json;

public static class Program {
    public static readonly Dictionary<uint, StructJson> STRUCT_INFO        = new();
    public static readonly Dictionary<uint, string>     ARMOR_NAME_LOOKUP  = new();
    public static readonly Dictionary<uint, string>     ITEM_NAME_LOOKUP   = new();
    public static readonly Dictionary<byte, string>     SKILL_NAME_LOOKUP  = new();
    public static readonly Dictionary<uint, string>     WEAPON_NAME_LOOKUP = new();

    public static void Main(string[] args) {
        ParseStructInfo();
        ParseArmorNames();
        ParseItemNames();
        ParseSkillNames();
        ParseWeaponNames();
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

    private static void ParseWeaponNames() {
        var lines = File.ReadAllLines(@"R:\Games\Monster Hunter Rise\MonsterHunterRiseModding.wiki\Weapon-IDs.md");
        var regex = new Regex(@"^\| (0[a-fA-F0-9]+) \| [^\|]+ \| [^\|]+ \| [^\|]+ \| [^\|]+ \| ([^\|]+) \|");

        foreach (var line in lines) {
            var match = regex.Match(line);
            if (!match.Success) continue;
            var id   = Convert.ToUInt32(match.Groups[1].Value, 16);
            var name = match.Groups[2].Value;
            if (WEAPON_NAME_LOOKUP.ContainsKey(id)) continue;
            WEAPON_NAME_LOOKUP[id] = name;
        }
        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\WEAPON_NAME_LOOKUP.json", JsonConvert.SerializeObject(WEAPON_NAME_LOOKUP));
    }
}