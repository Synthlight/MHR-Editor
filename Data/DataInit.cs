using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;
using RE_Editor.Common.Models;

namespace RE_Editor.Data;

public static class DataInit {
    public static void Init() {
        Assembly.Load(nameof(Common));
        Assembly.Load(nameof(Generated));
        InitStructTypeInfo();

        DataHelper.STRUCT_INFO = LoadDict<uint, StructJson>(Assets.STRUCT_INFO);

        DataHelper.ITEM_NAME_LOOKUP   = new();
        DataHelper.WEAPON_NAME_LOOKUP = new();

        foreach (var variant in Global.VARIANTS) {
            DataHelper.ITEM_NAME_LOOKUP[variant]   = LoadDict<Global.LangIndex, Dictionary<uint, string>>(GetAsset($"{variant}_ITEM_NAME_LOOKUP"));
            DataHelper.WEAPON_NAME_LOOKUP[variant] = LoadDict<Global.LangIndex, Dictionary<uint, string>>(GetAsset($"{variant}_WEAPON_NAME_LOOKUP"));
        }

        foreach (var lang in Enum.GetValues<Global.LangIndex>()) {
            if (!Global.TRANSLATION_MAP.ContainsKey(lang)) Global.TRANSLATION_MAP[lang] = new();
        }
    }

    public static void InitStructTypeInfo() {
        var mhrStructs = AppDomain.CurrentDomain.GetAssemblies()
                                  .SelectMany(t => t.GetTypes())
                                  .Where(type => type.GetCustomAttribute<MhrStructAttribute>() != null);
        foreach (var type in mhrStructs) {
            var hashField = type.GetField("HASH", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
            var value     = (uint) hashField.GetValue(null)!;
            DataHelper.RE_STRUCTS[value] = type;
        }
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