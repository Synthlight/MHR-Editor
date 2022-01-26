using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHR_Editor.Attributes;
using MHR_Editor.Models;
using Newtonsoft.Json;

namespace MHR_Editor.Data;

public static class DataHelper {
    public static readonly Dictionary<uint, Type>       MHR_STRUCTS = new();
    public static readonly Dictionary<uint, StructJson> STRUCT_INFO;
    public static readonly Dictionary<uint, string>     ARMOR_NAME_LOOKUP;
    public static readonly Dictionary<uint, string>     ITEM_NAME_LOOKUP;
    public static readonly Dictionary<byte, string>     SKILL_NAME_LOOKUP;
    public static readonly Dictionary<uint, string>     WEAPON_NAME_LOOKUP;

    static DataHelper() {
        var mhrStructs = Assembly.GetCallingAssembly().GetTypes().Where(type => type.GetCustomAttribute<MhrStructAttribute>() != null);
        foreach (var type in mhrStructs) {
            var hashField = type.GetField("HASH", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
            var value     = (uint) hashField.GetValue(null)!;
            MHR_STRUCTS[value] = type;
        }

        STRUCT_INFO        = LoadDict<uint, StructJson>(Assets.STRUCT_INFO);
        ARMOR_NAME_LOOKUP  = LoadDict<uint, string>(Assets.ARMOR_NAME_LOOKUP);
        ITEM_NAME_LOOKUP   = LoadDict<uint, string>(Assets.ITEM_NAME_LOOKUP);
        SKILL_NAME_LOOKUP  = LoadDict<byte, string>(Assets.SKILL_NAME_LOOKUP);
        WEAPON_NAME_LOOKUP = LoadDict<uint, string>(Assets.WEAPON_NAME_LOOKUP);
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