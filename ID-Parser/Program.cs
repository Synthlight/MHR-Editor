using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Generated.Enums;
using Newtonsoft.Json;

namespace MHR_Editor.ID_Parser;

public static class Program {
    private static readonly List<Tuple<string, string>> NAME_DESC = new() {
        new("Name", "NAME"),
        new("Explain", "DESC"),
    };

    public static void Main() {
        ParseStructInfo();
        ExtractItemInfo();
        ExtractArmorInfo();
        ExtractSkillInfo();
        ExtractWeaponInfo();
        ExtractDecorationInfo();
        ExtractDangoInfo();
    }

    private static void ParseStructInfo() {
        var structJson = JsonConvert.DeserializeObject<Dictionary<string, StructJson>>(File.ReadAllText(@"R:\Games\Monster Hunter Rise\RE_RSZ\rszmhrise.json"))!;
        var structInfo = new Dictionary<uint, StructJson>();
        foreach (var (key, value) in structJson) {
            var hash = uint.Parse(key, NumberStyles.HexNumber);
            structInfo[hash] = value;
        }
        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\STRUCT_INFO.json", JsonConvert.SerializeObject(structInfo));
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractItemInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = MSG.Read($@"V:\MHR\re_chunk_000\natives\STM\data\System\ContentsIdSystem\Item\Normal\Item{@in}.msg.17")
                         .GetLangIdMap(SubCategoryType.I_Normal, true);

            if (@in == "Name") {
                var potion = msg[Global.LangIndex.eng][0x4100006];
                Debug.Assert(potion == "Potion");
            }

            File.WriteAllText($@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\ITEM_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
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
                var msg = MSG.Read($@"V:\MHR\re_chunk_000\natives\STM\data\Define\Player\Armor\{type}\A_{type}_{@in}.msg.17")
                             .GetLangIdMap(enumType, false);
                msgLists.Add(msg);
            }
            var result = msgLists.MergeDictionaries();

            if (@in == "Name") {
                var kamuraHeadScarf = result[Global.LangIndex.eng][0xC100000];
                Debug.Assert(kamuraHeadScarf == "Kamura Head Scarf");
            }

            File.WriteAllText($@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\ARMOR_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractSkillInfo() {
        var msg = MSG.Read(@"V:\MHR\re_chunk_000\natives\STM\data\Define\Player\Skill\PlEquipSkill\PlayerSkill_Name.msg.17")
                     .GetLangIdMap(SubCategoryType.C_Unclassified, false);

        var attackBoost = msg[Global.LangIndex.eng][1];
        Debug.Assert(attackBoost == "Attack Boost");

        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        msg = MSG.Read(@"V:\MHR\re_chunk_000\natives\STM\data\Define\Player\Skill\PlHyakuryuSkill\HyakuryuSkill_Name.msg.17")
                 .GetLangIdMap(SubCategoryType.C_Unclassified, false);
        File.WriteAllText(@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\RAMPAGE_SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

        msg = MSG.Read($@"V:\MHR\re_chunk_000\natives\STM\data\Define\Player\Skill\PlKitchenSkill\KitchenSkill_Name.msg.17")
                 .GetLangIdMap(SubCategoryType.C_Unclassified, false);
        File.WriteAllText($@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\DANGO_SKILL_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractWeaponInfo() {
        var types = new List<string> {"Bow", "ChargeAxe", "DualBlades", "GreatSword", "GunLance", "Hammer", "HeavyBowgun", "Horn", "InsectGlaive", "Lance", "LightBowgun", "LongSword", "ShortSword", "SlashAxe"};
        foreach (var (@in, @out) in NAME_DESC) {
            var msgLists = new List<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(types.Count);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in types) {
                var enumType = Enum.Parse<SubCategoryType>($"W_{type}");
                var msg = MSG.Read($@"V:\MHR\re_chunk_000\natives\STM\data\Define\Player\Weapon\{type}\{type}_{@in}.msg.17")
                             .GetLangIdMap(enumType, false);
                msgLists.Add(msg);
            }
            if (@in == "Name") {
                var msg = MSG.Read($@"V:\MHR\re_chunk_000\natives\STM\data\Define\Player\Weapon\Insect\IG_Insect_{@in}.msg.17")
                             .GetLangIdMap(SubCategoryType.W_Insect, false);
                msgLists.Add(msg);
            }
            var result = msgLists.MergeDictionaries();

            if (@in == "Name") {
                var busterSword1 = result[Global.LangIndex.eng][0x8100000];
                Debug.Assert(busterSword1 == "Buster Sword I");
            }

            File.WriteAllText($@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\WEAPON_{@out}_LOOKUP.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDecorationInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = MSG.Read($@"V:\MHR\re_chunk_000\natives\STM\data\Define\Player\Equip\Decorations\Decorations_{@in}.msg.17")
                         .GetLangIdMap(SubCategoryType.C_Unclassified, false);
            File.WriteAllText($@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\DECORATION_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractDangoInfo() {
        foreach (var (@in, @out) in NAME_DESC) {
            var msg = MSG.Read($@"V:\MHR\re_chunk_000\natives\STM\data\Define\Lobby\Facility\Kitchen\Dango_{@in}.msg.17")
                         .GetLangIdMap(SubCategoryType.C_Unclassified, false);
            File.WriteAllText($@"R:\Games\Monster Hunter Rise\MHR-Editor\Data\Assets\DANGO_{@out}_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }
}