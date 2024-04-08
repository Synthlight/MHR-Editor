#nullable enable
using System;
using System.IO;
using RE_Editor.Models.Enums;

namespace RE_Editor.Models;

public static class Dd2HidePartsBase {
    public static void WriteHideParts(string name, string version, string path, Action<StreamWriter> go) {
        using var writer = new StreamWriter(File.Create(path));
        writer.WriteLine($"-- {name}");
        writer.WriteLine("-- By LordGregory");
        writer.WriteLine("-- Big thanks to alphaZomega who's script this is bastardized from.");
        writer.WriteLine("");
        writer.WriteLine($"local version = \"{version}\"");
        writer.WriteLine($"log.info(\"Initializing `{name}` v\"..version)");
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
        writer.WriteLine("-- Female");
        writer.WriteLine($"for enum_id, entry in pairs(get_dict(chr_edit_mgr._TopsDB[{(uint) App_Gender.Female}])) do");
        go.Invoke(writer);
        writer.Write("end");
    }
}