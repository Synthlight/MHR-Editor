using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHR_Editor.Common;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models;
using Newtonsoft.Json;

namespace MHR_Editor.Data;

public static class DataInit {
    public static void Init() {
        Assembly.Load("Common");
        Assembly.Load("Generated");
        var mhrStructs = AppDomain.CurrentDomain.GetAssemblies()
                                  .SelectMany(t => t.GetTypes())
                                  .Where(type => type.GetCustomAttribute<MhrStructAttribute>() != null);
        foreach (var type in mhrStructs) {
            var hashField = type.GetField("HASH", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
            var value     = (uint) hashField.GetValue(null)!;
            DataHelper.MHR_STRUCTS[value] = type;
        }

        DataHelper.STRUCT_INFO                = LoadDict<uint, StructJson>(Assets.STRUCT_INFO);
        DataHelper.ARMOR_NAME_LOOKUP          = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ARMOR_NAME_LOOKUP);
        DataHelper.ARMOR_DESC_LOOKUP          = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ARMOR_DESC_LOOKUP);
        DataHelper.ARMOR_SERIES_LOOKUP        = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ARMOR_SERIES_LOOKUP);
        DataHelper.CAT_NAME_LOOKUP            = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_NAME_LOOKUP);
        DataHelper.CAT_DESC_LOOKUP            = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_DESC_LOOKUP);
        DataHelper.CAT_DOG_WEAPON_NAME_LOOKUP = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_DOG_WEAPON_NAME_LOOKUP);
        DataHelper.CAT_DOG_WEAPON_DESC_LOOKUP = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_DOG_WEAPON_DESC_LOOKUP);
        DataHelper.DANGO_NAME_LOOKUP          = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DANGO_NAME_LOOKUP);
        DataHelper.DANGO_DESC_LOOKUP          = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DANGO_DESC_LOOKUP);
        DataHelper.DANGO_SKILL_NAME_LOOKUP    = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DANGO_SKILL_NAME_LOOKUP);
        DataHelper.DECORATION_NAME_LOOKUP     = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DECORATION_NAME_LOOKUP);
        DataHelper.DECORATION_DESC_LOOKUP     = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DECORATION_DESC_LOOKUP);
        DataHelper.DOG_NAME_LOOKUP            = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DOG_NAME_LOOKUP);
        DataHelper.DOG_DESC_LOOKUP            = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DOG_DESC_LOOKUP);
        DataHelper.ITEM_NAME_LOOKUP           = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ITEM_NAME_LOOKUP);
        DataHelper.ITEM_DESC_LOOKUP           = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ITEM_DESC_LOOKUP);
        DataHelper.RAMPAGE_SKILL_NAME_LOOKUP  = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.RAMPAGE_SKILL_NAME_LOOKUP);
        DataHelper.SKILL_NAME_LOOKUP          = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.SKILL_NAME_LOOKUP);
        DataHelper.WEAPON_NAME_LOOKUP         = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.WEAPON_NAME_LOOKUP);
        DataHelper.WEAPON_DESC_LOOKUP         = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.WEAPON_DESC_LOOKUP);
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