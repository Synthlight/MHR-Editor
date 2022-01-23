using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using MHR_Editor.Models;
using Newtonsoft.Json;

namespace MHR_Editor.Data;

public static class DataHelper {
    public static readonly Dictionary<uint, StructJson> STRUCT_INFO       = new();
    public static readonly Dictionary<uint, string>     ARMOR_NAME_LOOKUP = new();
    public static readonly Dictionary<byte, string>     SKILL_NAME_LOOKUP = new();

    static DataHelper() {
        var structJson = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(@"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json"));
        Debug.Assert(structJson != null, nameof(structJson) + " != null");
        foreach (var (key, value) in structJson) {
            var hash = uint.Parse(key, NumberStyles.HexNumber);
            STRUCT_INFO[hash] = value;
        }

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

        lines = File.ReadAllLines(@"R:\Games\Monster Hunter Rise\MonsterHunterRiseModding.wiki\Skill-IDs.md");
        regex = new Regex(@"^\| (\d+) \| Pl_EquipSkill_\d+ \| ([^\|]+) \|");

        SKILL_NAME_LOOKUP[0] = "None";

        foreach (var line in lines) {
            var match = regex.Match(line);
            if (!match.Success) continue;
            var id   = Convert.ToByte(match.Groups[1].Value, 10);
            var name = match.Groups[2].Value;
            if (SKILL_NAME_LOOKUP.ContainsKey(id)) continue;
            SKILL_NAME_LOOKUP[id] = name;
        }
    }
}