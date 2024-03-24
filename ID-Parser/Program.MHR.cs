﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RE_Editor.Common;
using RE_Editor.Generated.Enums;
using RE_Editor.Models.Enums;
using MSG = RE_Editor.Common.Models.MSG;

namespace RE_Editor.ID_Parser;

public static partial class Program {
    public const string CONFIG_NAME = "MHR";

    private static readonly List<Tuple<string, string>> NAME_DESC = [
        new("Name", "NAME"),
        new("Explain", "DESC")
    ];

    private const string MR = "{{{MR}}}"; // To find/replace with either nothing or `_MR` when parsing paired files.

    public static void Main() {
        ExtractItemInfo();
        ExtractArmorInfo();
        ExtractArmorSeriesInfo();
        ExtractSkillInfo();
        ExtractWeaponInfo();
        ExtractDecorationInfo();
        ExtractDangoInfo();
        ExtractCatDogArmorInfo();
        ExtractCatDogWeaponInfo();
        ExtractPetalaceInfo();
        ExtractSwitchSkillInfo();
        ExtractGuildCardInfo();
    }

    private static Dictionary<Global.LangIndex, Dictionary<uint, string>> GetMergedMrTexts(string path, SubCategoryType type, bool startAtOne, uint offsetToAdd, bool addAfter = false, bool writeNameIds = false) {
        var baseList = MSG.Read(path.Replace(MR, ""), writeNameIds)
                          .GetLangIdMap((uint) type, startAtOne);
        var mrList = MSG.Read(path.Replace(MR, "_MR"))
                        .GetLangIdMap((uint) type, startAtOne, addAfter ? (uint) baseList[0].Count : offsetToAdd);
        var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(2) {baseList, mrList};
        return MergedMrTexts(msgLists);
    }

    private static Dictionary<Global.LangIndex, Dictionary<T, string>> GetMergedMrTexts<T>(string path, Func<string, T> parseName, bool writeNameIds = false, bool ignoreDuplicateKeys = false) where T : notnull {
        var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<T, string>>>(2) {
            MSG.Read(path.Replace(MR, ""), writeNameIds)
               .GetLangIdMap(parseName),
            MSG.Read(path.Replace(MR, "_MR"), writeNameIds)
               .GetLangIdMap(parseName)
        };
        if (ignoreDuplicateKeys && typeof(T) == typeof(uint)) {
            return MergedMrTexts(msgLists);
        } else {
            return msgLists.MergeDictionaries();
        }
    }

    private static Dictionary<Global.LangIndex, Dictionary<T, string>> MergedMrTexts<T>(IReadOnlyList<Dictionary<Global.LangIndex, Dictionary<T, string>>> msgLists) where T : notnull {
        try {
            return msgLists.MergeDictionaries();
        } catch (ArgumentException) {
            foreach (var (langIndex, baseDict) in msgLists[0]) {
                var mrDict = msgLists[1][langIndex];
                foreach (var key in mrDict.Keys.Where(key => baseDict.ContainsKey(key))) {
                    baseDict[key] = mrDict[key];
                    mrDict.Remove(key);
                }
            }
            return msgLists.MergeDictionaries();
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractItemInfo() {
        var regex = new Regex(@"I_(?:(.*)_)?(\d\d\d\d)");

        foreach (var (@in, @out) in NAME_DESC) {
            var result = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\System\ContentsIdSystem\Item\Normal\Item{@in}{MR}.msg.{Global.MSG_VERSION}", name => {
                if (name.StartsWith("I_None")) return (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                var subType                = regex.Match(name).Groups[1].Value;
                if (subType == "") subType = "Normal";
                var value                  = regex.Match(name).Groups[2].Value;
                return (uint) Enum.Parse(typeof(Snow_data_ContentsIdSystem_ItemId), $"I_{subType}_{value}") + (name.EndsWith("_MR") ? 2000 : 0);
            });

            if (@in == "Name") {
                var potion = result[Global.LangIndex.eng][0x4100006];
                Debug.Assert(potion == "Potion");
                // ReSharper disable once IdentifierTypo
                var eurekacorn = result[Global.LangIndex.eng][0x41007D1];
                Debug.Assert(eurekacorn == "Eurekacorn");
            }

            CreateAssetFile(result, $"ITEM_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractArmorInfo() {
        var types = new List<string> {"Arm", "Chest", "Head", "Leg", "Waist"};
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(types.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in types) {
                var enumType = Enum.Parse<SubCategoryType>($"A_{type}");
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Armor\{type}\A_{type}_{@in}{MR}.msg.{Global.MSG_VERSION}", enumType, false, 300));
            }
            var result = msgLists.MergeDictionaries();

            if (@in == "Name") {
                var kamuraHeadScarf = result[Global.LangIndex.eng][0xC100000];
                Debug.Assert(kamuraHeadScarf == "Kamura Head Scarf");
            }

            CreateAssetFile(result, $"ARMOR_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractArmorSeriesInfo() {
        var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Armor\ArmorSeries_Hunter_Name{MR}.msg.{Global.MSG_VERSION}", SubCategoryType.C_Unclassified, false, 300);

        CreateAssetFile(msg, "ARMOR_SERIES_LOOKUP");

        CreateConstantsFile(msg[Global.LangIndex.eng], "ArmorConstants");
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractSkillInfo() {
        var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlEquipSkill\PlayerSkill_Name{MR}.msg.{Global.MSG_VERSION}", name =>
                                       ParseEnum(typeof(Snow_data_DataDef_PlEquipSkillId), name.Replace("PlayerSkill", "Pl_EquipSkill")));

        // Load it again, but get the name this time, so we can use it to create the enum lookup file.
        // Not efficient, but works.
        var skillEnumToIdLookup = new Dictionary<Global.LangIndex, Dictionary<Snow_data_DataDef_PlEquipSkillId, string>>();
        var skillEnumData = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlEquipSkill\PlayerSkill_Name{MR}.msg.{Global.MSG_VERSION}", name => {
            var value = name.Replace("PlayerSkill", "Pl_EquipSkill");
            if (value == "Pl_EquipSkill_None") throw new MSG.SkipReadException();
            return value;
        });
        foreach (var lang in Enum.GetValues<Global.LangIndex>()) {
            skillEnumToIdLookup[lang] = new();
            foreach (var (enumName, id) in skillEnumData[lang]) {
                skillEnumToIdLookup[lang][Enum.Parse<Snow_data_DataDef_PlEquipSkillId>(enumName)] = id;
            }
        }
        CreateAssetFile(skillEnumToIdLookup, "SKILL_ENUM_NAME_LOOKUP");

        var engSkills = msg[Global.LangIndex.eng];
        Debug.Assert(engSkills[1] == "Attack Boost");

        CreateAssetFile(msg, "SKILL_NAME_LOOKUP");

        msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlHyakuryuSkill\HyakuryuSkill_Name{MR}.msg.{Global.MSG_VERSION}", name =>
                                   ParseEnum(typeof(Snow_data_DataDef_PlHyakuryuSkillId), name));
        CreateAssetFile(msg, "RAMPAGE_SKILL_NAME_LOOKUP");

        msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlKitchenSkill\KitchenSkill_Name{MR}.msg.{Global.MSG_VERSION}", name =>
                                   ParseEnum(typeof(Snow_data_DataDef_PlKitchenSkillId), $"Pl_{name}"));
        CreateAssetFile(msg, "DANGO_SKILL_NAME_LOOKUP");

        CreateConstantsFile(engSkills, "SkillConstants");
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractWeaponInfo() {
        var regex       = new Regex(@"IG_Insect_(\d\d\d)");
        var oneBelowMax = GetOneBelowMax<Snow_data_ContentsIdSystem_WeaponId>(nameof(Snow_data_ContentsIdSystem_WeaponId.W_Insect_Max));

        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(Global.WEAPON_TYPES.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in Global.WEAPON_TYPES) {
                var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Weapon\{type}\{type}_{@in}{MR}.msg.{Global.MSG_VERSION}", name =>
                                               ParseEnum(typeof(Snow_data_ContentsIdSystem_WeaponId), name.Replace("_MR", "")), ignoreDuplicateKeys: true);
                msgLists.Add(msg);
                if (@in == "Name") {
                    CreateConstantsFile(msg[Global.LangIndex.eng], $"{type}Constants", true);
                }
            }
            if (@in == "Name") {
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Weapon\Insect\IG_Insect_{@in}{MR}.msg.{Global.MSG_VERSION}", name => {
                    if (name == "IG_Insect_None") throw new MSG.SkipReadException();
                    var value = regex.Match(name).Groups[1].Value;
                    if (int.Parse(value) > oneBelowMax) throw new MSG.SkipReadException();
                    return ParseEnum(typeof(Snow_data_ContentsIdSystem_WeaponId), $"W_Insect_{value}");
                }, writeNameIds: true));
            }
            var result = msgLists.MergeDictionaries();

            if (@in == "Name") {
                var busterSword1 = result[Global.LangIndex.eng][0x8100000];
                Debug.Assert(busterSword1 == "Buster Sword I");
            }

            CreateAssetFile(result, $"WEAPON_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDecorationInfo() {
        var regex = new Regex(@"Deco_(\d?\d\d\d)");

        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Equip\Decorations\Decorations_{@in}{MR}.msg.{Global.MSG_VERSION}", name => {
                var value = name.Replace("Decorations", "Deco");
                if (value == nameof(Snow_equip_DecorationsId.Deco_None)) return (uint) Snow_equip_DecorationsId.Deco_None;
                var match = uint.Parse(regex.Match(value).Groups[1].Value);
                value = $"Deco_{match:D4}";
                try {
                    return ParseEnum(typeof(Snow_equip_DecorationsId), value);
                } catch (Exception) {
                    Debug.WriteLine($"Error reading ${value}.");
                    throw new MSG.SkipReadException();
                }
            });
            CreateAssetFile(msg, $"DECORATION_{@out}_LOOKUP");

            msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Equip\HyakuryuDeco\HyakuryuDeco_{@in}_MR.msg.{Global.MSG_VERSION}")
                     .GetLangIdMap(name => ParseEnum(typeof(Snow_equip_DecorationsId), name));
            CreateAssetFile(msg, $"RAMPAGE_DECORATION_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDangoInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Lobby\Facility\Kitchen\Dango_{@in}{MR}.msg.{Global.MSG_VERSION}", name =>
                                           ParseEnum(typeof(Snow_data_DataDef_DangoId), name));
            CreateAssetFile(msg, $"DANGO_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractCatDogArmorInfo() {
        var regex = new Regex(@"Ot(Airou|Dog)Armor_(Chest|Head)_(\d\d\d)");

        uint Func(string name) {
            var match  = regex.Match(name);
            var animal = match.Groups[1].Value;
            var slot   = match.Groups[2].Value;
            var num    = match.Groups[3].Value;
            var max    = GetOneBelowMax<Snow_data_DataDef_OtArmorId>($"OtArmor_{animal}_{slot}_Max");
            if (int.Parse(num) > max) throw new MSG.SkipReadException();
            return ParseEnum(typeof(Snow_data_DataDef_OtArmorId), $"OtArmor_{animal}_{slot}_{num}");
        }

        var types = new List<string> {"Chest", "Head"};
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(types.Count);
            foreach (var type in types) {
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Armor\OtAirouArmor_{type}_{@in}{MR}.msg.{Global.MSG_VERSION}", Func));
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Armor\OtDogArmor_{type}_{@in}{MR}.msg.{Global.MSG_VERSION}", Func));
            }
            var result = msgLists.MergeDictionaries();

            CreateAssetFile(result, $"CAT_DOG_ARMOR_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractCatDogWeaponInfo() {
        var regex = new Regex(@"Ot(Airou|Dog)Weapon_(\d\d\d|None)");

        uint Func(string name) {
            var match  = regex.Match(name);
            var animal = match.Groups[1].Value;
            var num    = match.Groups[2].Value;
            if (num == "None") throw new MSG.SkipReadException();
            var max = GetOneBelowMax<Snow_data_DataDef_OtWeaponId>($"OtWeapon_{animal}_Max");
            if (int.Parse(num) > max) throw new MSG.SkipReadException();
            return ParseEnum(typeof(Snow_data_DataDef_OtWeaponId), $"OtWeapon_{animal}_{num}");
        }

        foreach (var (@in, @out) in NAME_DESC) {
            var catMsg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Weapon\OtDogWeapon_{@in}{MR}.msg.{Global.MSG_VERSION}", Func, ignoreDuplicateKeys: true);
            var dogMsg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Weapon\OtAirouWeapon_{@in}{MR}.msg.{Global.MSG_VERSION}", Func, ignoreDuplicateKeys: true);
            var result = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>> {catMsg, dogMsg}.MergeDictionaries();
            CreateAssetFile(result, $"CAT_DOG_WEAPON_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractPetalaceInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\data\System\ContentsIdSystem\LvBuffCage\Normal\LvBuffCage_{@in}.msg.{Global.MSG_VERSION}", true)
                         .GetLangIdMap(name => ParseEnum(typeof(Snow_data_ContentsIdSystem_LvBuffCageId), name));
            CreateAssetFile(msg, $"PETALACE_{@out}_LOOKUP");
            if (@in == "Name") {
                CreateConstantsFile(msg[Global.LangIndex.eng], "PetalaceConstants", true);
            }
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractSwitchSkillInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlSwitchAction\PlayerSwitchAction_{@in}{MR}.msg.{Global.MSG_VERSION}", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
            CreateAssetFile(msg, $"SWITCH_SKILL_{@out}_LOOKUP");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractGuildCardInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\Message\GuildCard\GC_Achievement_{@in}{MR}.msg.{Global.MSG_VERSION}", name => name.Replace("GC_Achievement_", ""));
            CreateAssetFile(msg, $"GC_TITLE_{@out}_LOOKUP");
        }
    }
}