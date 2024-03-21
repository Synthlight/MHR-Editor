using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using RE_Editor.Data.MHR;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;

namespace RE_Editor.Data;

public static partial class DataInit {
    // ReSharper disable once IdentifierTypo
    private static void LoadDicts() {
        DataHelper.ARMOR_NAME_LOOKUP              = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ARMOR_NAME_LOOKUP);
        DataHelper.ARMOR_DESC_LOOKUP              = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ARMOR_DESC_LOOKUP);
        DataHelper.ARMOR_SERIES_LOOKUP            = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ARMOR_SERIES_LOOKUP);
        DataHelper.CAT_DOG_ARMOR_NAME_LOOKUP      = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_DOG_ARMOR_NAME_LOOKUP);
        DataHelper.CAT_DOG_ARMOR_DESC_LOOKUP      = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_DOG_ARMOR_DESC_LOOKUP);
        DataHelper.CAT_DOG_WEAPON_NAME_LOOKUP     = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_DOG_WEAPON_NAME_LOOKUP);
        DataHelper.CAT_DOG_WEAPON_DESC_LOOKUP     = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.CAT_DOG_WEAPON_DESC_LOOKUP);
        DataHelper.DANGO_NAME_LOOKUP              = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DANGO_NAME_LOOKUP);
        DataHelper.DANGO_DESC_LOOKUP              = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DANGO_DESC_LOOKUP);
        DataHelper.DANGO_SKILL_NAME_LOOKUP        = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DANGO_SKILL_NAME_LOOKUP);
        DataHelper.DECORATION_NAME_LOOKUP         = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DECORATION_NAME_LOOKUP);
        DataHelper.DECORATION_DESC_LOOKUP         = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.DECORATION_DESC_LOOKUP);
        DataHelper.GC_TITLE_NAME_LOOKUP           = LoadDict<Global.LangIndex, Dictionary<string, string>>(Assets.GC_TITLE_NAME_LOOKUP);
        DataHelper.GC_TITLE_DESC_LOOKUP           = LoadDict<Global.LangIndex, Dictionary<string, string>>(Assets.GC_TITLE_DESC_LOOKUP);
        DataHelper.ITEM_NAME_LOOKUP               = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ITEM_NAME_LOOKUP);
        DataHelper.ITEM_DESC_LOOKUP               = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.ITEM_DESC_LOOKUP);
        DataHelper.PETALACE_NAME_LOOKUP           = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.PETALACE_NAME_LOOKUP);
        DataHelper.PETALACE_DESC_LOOKUP           = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.PETALACE_DESC_LOOKUP);
        DataHelper.RAMPAGE_DECORATION_NAME_LOOKUP = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.RAMPAGE_DECORATION_NAME_LOOKUP);
        DataHelper.RAMPAGE_DECORATION_DESC_LOOKUP = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.RAMPAGE_DECORATION_DESC_LOOKUP);
        DataHelper.RAMPAGE_SKILL_NAME_LOOKUP      = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.RAMPAGE_SKILL_NAME_LOOKUP);
        DataHelper.SKILL_NAME_LOOKUP              = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.SKILL_NAME_LOOKUP);
        DataHelper.SWITCH_SKILL_NAME_LOOKUP       = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.SWITCH_SKILL_NAME_LOOKUP);
        DataHelper.SWITCH_SKILL_DESC_LOOKUP       = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.SWITCH_SKILL_DESC_LOOKUP);
        DataHelper.WEAPON_NAME_LOOKUP             = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.WEAPON_NAME_LOOKUP);
        DataHelper.WEAPON_DESC_LOOKUP             = LoadDict<Global.LangIndex, Dictionary<uint, string>>(Assets.WEAPON_DESC_LOOKUP);
    }

    private static void CreateTranslationsForSkillEnumNameColumns() {
        var skillEnumLookup = LoadDict<Global.LangIndex, Dictionary<Snow_data_DataDef_PlEquipSkillId, string>>(Assets.SKILL_ENUM_NAME_LOOKUP);
        var regex           = new Regex(@"^EquipSkill_(\d+)");
        foreach (var propertyInfo in typeof(Snow_player_EquipSkillParameter).GetProperties()) {
            var propName = propertyInfo.Name;
            var match    = regex.Match(propName);
            if (!match.Success) continue;
            var enumName  = match.Groups[0].Value;
            var enumValue = Enum.Parse<Snow_data_DataDef_PlEquipSkillId>($"Pl_{enumName}");
            foreach (var lang in Enum.GetValues<Global.LangIndex>()) {
                var skillMap = skillEnumLookup[lang];
                if (!skillMap.ContainsKey(enumValue) || skillMap[enumValue] == "#Rejected#") continue;
                var skillName = skillMap[enumValue];
                var newName   = propName.Replace(enumName, skillName);
                Global.TRANSLATION_MAP[lang][propName] = newName;
            }
        }
    }
}