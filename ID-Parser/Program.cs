using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Generated.Enums;
using Newtonsoft.Json;

namespace MHR_Editor.ID_Parser;

public static class Program {
    public const string BASE_PROJ_PATH = @"..\..\..";

    private static readonly List<Tuple<string, string>> NAME_DESC = new() {
        new("Name", "NAME"),
        new("Explain", "DESC"),
    };

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
            var result = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\System\ContentsIdSystem\Item\Normal\Item{@in}{MR}.msg.17", SubCategoryType.I_Normal, true, 2001);

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
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Armor\{type}\A_{type}_{@in}{MR}.msg.17", enumType, false, 300));
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
        var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Armor\ArmorSeries_Hunter_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 300);
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\ARMOR_SERIES_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        CreateConstantsFile(msg[Global.LangIndex.eng], "ArmorConstants");
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractSkillInfo() {
        var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlEquipSkill\PlayerSkill_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 112);

        var engSkills = msg[Global.LangIndex.eng];
        Debug.Assert(engSkills[1] == "Attack Boost");

        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlHyakuryuSkill\HyakuryuSkill_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\RAMPAGE_SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlKitchenSkill\KitchenSkill_Name{MR}.msg.17", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\DANGO_SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        CreateConstantsFile(engSkills, "SkillConstants");
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractWeaponInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(Global.WEAPON_TYPES.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in Global.WEAPON_TYPES) {
                var enumType = Enum.Parse<SubCategoryType>($"W_{type}");
                var msg      = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Weapon\{type}\{type}_{@in}{MR}.msg.17", enumType, false, 300);
                msgLists.Add(msg);
                if (@in == "Name") {
                    CreateConstantsFile(msg[Global.LangIndex.eng], $"{type}Constants", true);
                }
            }
            if (@in == "Name") {
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Weapon\Insect\IG_Insect_{@in}{MR}.msg.17", SubCategoryType.W_Insect, true, 100));
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
            var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Equip\Decorations\Decorations_{@in}{MR}.msg.17", SubCategoryType.C_Unclassified, false, 109);
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\DECORATION_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Equip\HyakuryuDeco\HyakuryuDeco_{@in}_MR.msg.17").GetLangIdMap(SubCategoryType.C_Normal, false);
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\RAMPAGE_DECORATION_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDangoInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Lobby\Facility\Kitchen\Dango_{@in}{MR}.msg.17", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\DANGO_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractCatDogArmorInfo() {
        var types = new List<string> {"Chest", "Head"};
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(types.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in types) {
                var catEnumType = Enum.Parse<SubCategoryType>($"OtArmor_Airou_{type}");
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Armor\OtAirouArmor_{type}_{@in}{MR}.msg.17", catEnumType, false, 200));
                var dogEnumType = Enum.Parse<SubCategoryType>($"OtArmor_Dog_{type}");
                msgLists.Add(GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Armor\OtDogArmor_{type}_{@in}{MR}.msg.17", dogEnumType, false, 200));
            }
            var result = msgLists.MergeDictionaries();

            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\CAT_DOG_ARMOR_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractCatDogWeaponInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var catMsg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Weapon\OtDogWeapon_{@in}{MR}.msg.17", SubCategoryType.OtWeapon_Dog, true, 200);
            var dogMsg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Otomo\Equip\Weapon\OtAirouWeapon_{@in}{MR}.msg.17", SubCategoryType.OtWeapon_Airou, true, 200);
            var result = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>> {catMsg, dogMsg}.MergeDictionaries();
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\CAT_DOG_WEAPON_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractPetalaceInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\data\System\ContentsIdSystem\LvBuffCage\Normal\LvBuffCage_{@in}.msg.17").GetLangIdMap(SubCategoryType.LvC_Normal, false);
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\PETALACE_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
            if (@in == "Name") {
                CreateConstantsFile(msg[Global.LangIndex.eng], "PetalaceConstants", true);
            }
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractSwitchSkillInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = GetMergedMrTexts($@"{PathHelper.CHUNK_PATH}\natives\STM\data\Define\Player\Skill\PlSwitchAction\PlayerSwitchAction_{@in}{MR}.msg.17", SubCategoryType.C_Unclassified, false, 0, addAfter: true);
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\SWITCH_SKILL_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }

    private static void CreateConstantsFile(Dictionary<uint, string> engDict, string className, bool asHex = false) {
        using var writer = new StreamWriter(File.Create($@"{BASE_PROJ_PATH}\Constants\{className}.cs"));
        writer.WriteLine("using System.Diagnostics.CodeAnalysis;");
        writer.WriteLine("");
        writer.WriteLine("namespace MHR_Editor.Constants;");
        writer.WriteLine("");
        writer.WriteLine("[SuppressMessage(\"ReSharper\", \"InconsistentNaming\")]");
        writer.WriteLine("[SuppressMessage(\"ReSharper\", \"UnusedMember.Global\")]");
        writer.WriteLine("[SuppressMessage(\"ReSharper\", \"IdentifierTypo\")]");
        writer.WriteLine($"public static class {className} {{");
        var namesUsed = new List<string?>(engDict.Count);
        foreach (var (key, name) in engDict) {
            if (name.ToLower() == "#rejected#") continue;
            var constName = name.ToUpper()
                                .Replace("'", "")
                                .Replace("\"", "")
                                .Replace(".", "")
                                .Replace("(", "")
                                .Replace(")", "")
                                .Replace("/", "_")
                                .Replace("&", "AND")
                                .Replace("+", "_PLUS")
                                .Replace('-', '_')
                                .Replace(' ', '_');
            if (namesUsed.Contains(constName)) continue;
            namesUsed.Add(constName);
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (asHex) {
                writer.WriteLine($"    public const uint {constName} = 0x{key:X8};");
            } else {
                writer.WriteLine($"    public const uint {constName} = {key};");
            }
        }
        writer.WriteLine("}");
    }
}