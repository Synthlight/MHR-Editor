#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using RE_Editor.Common;

namespace RE_Editor.Models;

public static class ItemDbTweakWriter {
    public static void WriteTweak(ItemDbTweak tweak, string modFolderName) {
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
        writer.WriteLine("local itemManager = sdk.get_managed_singleton(\"app.ItemManager\")");
        writer.WriteLine("");
        writer.WriteLine("for id, entry in pairs(get_dict(itemManager._ItemDataDict)) do");

        var groupedChanges = new Dictionary<ItemDbTweak.Target, List<ItemDbTweak.Change>>();
        foreach (var change in tweak.Changes) {
            if (!groupedChanges.ContainsKey(change.Target)) groupedChanges[change.Target] = [];
            groupedChanges[change.Target].Add(change);
        }

        foreach (var (target, changes) in groupedChanges) {
            writer.WriteLine($"    if entry:GetType() == {GetTargetType(target)} then");
            foreach (var change in changes) {
                change.Action(writer);
            }
            writer.WriteLine("    end");
        }

        writer.WriteLine("end");
    }

    private static string GetTargetType(ItemDbTweak.Target target) {
        return target switch {
            ItemDbTweak.Target.ARMOR => "sdk.typeof(\"app.ItemArmorParam\")",
            ItemDbTweak.Target.ITEM => "sdk.typeof(\"app.ItemDataParam\")",
            ItemDbTweak.Target.WEAPON => "sdk.typeof(\"app.ItemWeaponParam\")",
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };
    }
}