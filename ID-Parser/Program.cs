using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Generated.Enums;
using Newtonsoft.Json;

namespace MHR_Editor.ID_Parser;

public static class Program {
    public const string BASE_PROJ_PATH   = @"..\..\..";
    public const string STRUCT_JSON_PATH = @"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json";
    public const string PAK_FOLDER_PATH  = @"V:\MHR\re_chunk_000";

    private static readonly List<Tuple<string, string>> NAME_DESC = new() {
        new("Name", "NAME"),
        new("Explain", "DESC"),
    };

    private const string MR = "{{{MR}}}"; // To find/replace with either nothing or `_MR` when parsing paired files.

    public static void Main() {
        ParseStructInfo();
        ExtractItemInfo();
        ExtractArmorInfo();
        ExtractArmorSeriesInfo();
        ExtractSkillInfo();
        ExtractWeaponInfo();
        ExtractDecorationInfo();
        ExtractDangoInfo();
        ExtractDogInfo();
        ExtractCatInfo();
        ExtractCatDogWeaponInfo();
    }

    private static void ParseStructInfo() {
        var structJson = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(STRUCT_JSON_PATH))!;
        var structInfo = new Dictionary<uint, StructJson>();
        foreach (var (key, value) in structJson) {
            var hash = uint.Parse(key, NumberStyles.HexNumber);
            structInfo[hash] = value;
        }
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\STRUCT_INFO.json", JsonConvert.SerializeObject(structInfo, Formatting.Indented));
    }

    private static Dictionary<Global.LangIndex, Dictionary<uint, string>> GetMergedMrTexts(string path, SubCategoryType type, bool startAtOne, uint offsetToAdd, bool addAfter = false) {
        var baseList = MSG.Read(path.Replace(MR, ""))
                          .GetLangIdMap(type, startAtOne);
        var mrList = MSG.Read(path.Replace(MR, "_MR"))
                        .GetLangIdMap(type, startAtOne, addAfter ? (uint) baseList[0].Count : offsetToAdd);
        var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(2) {baseList, mrList};
        try {
            return msgLists.MergeDictionaries();
        } catch (ArgumentException) {
            foreach (var pair in baseList) {
                var firstId  = msgLists[1][pair.Key].Keys.First();
                var toRemove = pair.Value.Where(kvp => kvp.Key >= firstId).Select(x => x.Key).ToList();
                foreach (var o in toRemove) {
                    pair.Value.Remove(o);
                }
            }
            return msgLists.MergeDictionaries();
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractItemInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var result = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\System\ContentsIdSystem\Item\Normal\Item{@in}{MR}.msg.17", SubCategoryType.I_Normal, true, 2001);

            if (@in == "Name") {
                var potion = result[Global.LangIndex.eng][0x4100006];
                Debug.Assert(potion == "Potion");
            }

            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\ITEM_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
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
                msgLists.Add(GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Armor\{type}\A_{type}_{@in}{MR}.msg.17", enumType, false, 300));
            }
            var result = msgLists.MergeDictionaries();

            if (@in == "Name") {
                var kamuraHeadScarf = result[Global.LangIndex.eng][0xC100000];
                Debug.Assert(kamuraHeadScarf == "Kamura Head Scarf");
            }

            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\ARMOR_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractArmorSeriesInfo() {
        var msg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Armor\ArmorSeries_Hunter_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 300);
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\ARMOR_SERIES_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractSkillInfo() {
        var msg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Skill\PlEquipSkill\PlayerSkill_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 112);

        var attackBoost = msg[Global.LangIndex.eng][1];
        Debug.Assert(attackBoost == "Attack Boost");

        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        // TODO: Figure out how the rest of these merge into the ID lists.

        msg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Skill\PlHyakuryuSkill\HyakuryuSkill_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\RAMPAGE_SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        msg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Skill\PlKitchenSkill\KitchenSkill_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\DANGO_SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractWeaponInfo() {
        var types = new List<string> {"Bow", "ChargeAxe", "DualBlades", "GreatSword", "GunLance", "Hammer", "HeavyBowgun", "Horn", "InsectGlaive", "Lance", "LightBowgun", "LongSword", "ShortSword", "SlashAxe"};
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(types.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in types) {
                var enumType = Enum.Parse<SubCategoryType>($"W_{type}");
                msgLists.Add(GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Weapon\{type}\{type}_{@in}{MR}.msg.17", enumType, false, 300));
            }
            if (@in == "Name") {
                msgLists.Add(GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Weapon\Insect\IG_Insect_{@in}{MR}.msg.17", SubCategoryType.W_Insect, false, 100));
            }
            var result = msgLists.MergeDictionaries();

            if (@in == "Name") {
                var busterSword1 = result[Global.LangIndex.eng][0x8100000];
                Debug.Assert(busterSword1 == "Buster Sword I");
            }

            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\WEAPON_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDecorationInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Equip\Decorations\Decorations_{@in}{MR}.msg.17", SubCategoryType.C_Unclassified, false, 109);
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\DECORATION_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDangoInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Lobby\Facility\Kitchen\Dango_{@in}{MR}.msg.17", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\DANGO_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDogInfo() {
        var types = new List<string> {"Chest", "Head"};
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(types.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in types) {
                var enumType = Enum.Parse<SubCategoryType>($"OtArmor_Dog_{type}");
                msgLists.Add(GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Otomo\Equip\Armor\OtDogArmor_{type}_{@in}{MR}.msg.17", enumType, false, 200));
            }
            var result = msgLists.MergeDictionaries();

            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\DOG_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractCatInfo() {
        var types = new List<string> {"Chest", "Head"};
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(types.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in types) {
                var enumType = Enum.Parse<SubCategoryType>($"OtArmor_Airou_{type}");
                msgLists.Add(GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Otomo\Equip\Armor\OtAirouArmor_{type}_{@in}{MR}.msg.17", enumType, false, 200));
            }
            var result = msgLists.MergeDictionaries();

            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\CAT_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractCatDogWeaponInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var catMsg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Otomo\Equip\Weapon\OtDogWeapon_{@in}{MR}.msg.17", SubCategoryType.OtWeapon_Dog, false, 200);
            var dogMsg = GetMergedMrTexts($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Otomo\Equip\Weapon\OtAirouWeapon_{@in}{MR}.msg.17", SubCategoryType.OtWeapon_Airou, false, 200);
            var result = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>> {catMsg, dogMsg}.MergeDictionaries();
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\CAT_DOG_WEAPON_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}