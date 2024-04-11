#nullable enable
using System.Collections.Generic;
using System.IO;
using RE_Editor.Common;
using RE_Editor.Models.Enums;
using static RE_Editor.Models.SwapDbTweak;

namespace RE_Editor.Models;

public static class Dd2HidePartsBase {
    public static void WriteHideParts(IEnumerable<SwapDbTweak> tweaks, string modFolderName) {
        foreach (var tweak in tweaks) {
            WriteLuaFile(tweak, modFolderName);
        }
    }

    private static void WriteLuaFile(SwapDbTweak tweak, string modFolderName) {
        var luaPath = $@"{PathHelper.MODS_PATH}\{modFolderName}\{tweak.LuaName}";
        tweak.AdditionalFiles!.Add($@"reframework\autorun\{tweak.LuaName}", luaPath);

        using var writer = new StreamWriter(File.Create(luaPath));
        writer.WriteLine($"-- {tweak.Name}");
        writer.WriteLine("-- By LordGregory");
        writer.WriteLine("-- Big thanks to alphaZomega who's script this is bastardized from.");
        writer.WriteLine("");
        writer.WriteLine($"local version = \"{tweak.Version}\"");
        writer.WriteLine($"log.info(\"Initializing `{tweak.Name}` v\"..version)");
        writer.WriteLine("");
        writer.WriteLine("-- Gets a dictionary from a RE Managed Object.");
        writer.WriteLine("local function get_dict(dict, as_array, sort_fn)");
        writer.WriteLine("	local output = {}");
        writer.WriteLine("	if not dict._entries then return output end");
        writer.WriteLine("	if as_array then");
        writer.WriteLine("		for i, value_obj in pairs(dict._entries) do");
        writer.WriteLine("			output[i] = value_obj.value");
        writer.WriteLine("		end");
        writer.WriteLine("		if sort_fn then");
        writer.WriteLine("			table.sort(output, sort_fn)");
        writer.WriteLine("		end");
        writer.WriteLine("	else");
        writer.WriteLine("		for i, value_obj in pairs(dict._entries) do");
        writer.WriteLine("			if value_obj.value ~= nil then");
        writer.WriteLine("				output[value_obj.key] = output[value_obj.key] or value_obj.value");
        writer.WriteLine("			end");
        writer.WriteLine("		end");
        writer.WriteLine("	end");
        writer.WriteLine("	return output");
        writer.WriteLine("end");
        writer.WriteLine("");
        writer.WriteLine("local chr_edit_mgr = sdk.get_managed_singleton(\"app.CharacterEditManager\")");
        writer.WriteLine("");

        var changesGroupedByDatabaseAndGender = new Dictionary<string, Dictionary<App_Gender, List<Change>>>();
        foreach (var change in tweak.Changes) {
            if (!changesGroupedByDatabaseAndGender.ContainsKey(change.Database)) changesGroupedByDatabaseAndGender[change.Database]                               = [];
            if (!changesGroupedByDatabaseAndGender[change.Database].ContainsKey(change.Gender)) changesGroupedByDatabaseAndGender[change.Database][change.Gender] = [];
            changesGroupedByDatabaseAndGender[change.Database][change.Gender].Add(change);
        }

        foreach (var (database, genderGroup) in changesGroupedByDatabaseAndGender) {
            foreach (var (gender, changes) in genderGroup) {
                writer.WriteLine($"for enum_id, entry in pairs(get_dict(chr_edit_mgr._{database}[{(uint) gender}])) do");
                foreach (var change in changes) {
                    change.Action(writer);
                }
                writer.WriteLine("end");
            }
        }
    }
}