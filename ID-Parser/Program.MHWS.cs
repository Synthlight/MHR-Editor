using System.Text.RegularExpressions;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using MSG = RE_Editor.Common.Models.MSG;

namespace RE_Editor.ID_Parser;

public static partial class Program {
    public static void Main() {
        //ExtractItemInfoByName();
        //ExtractItemInfoByGuid();
        //ExtractArmorInfoByGuid();
        ExtractWeaponInfoByGuid();
    }

    private static void ExtractItemInfoByName() {
        var regex = new Regex(@"Item_IT_(\d+)");
        var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\GameDesign\Text\Excel_Data\Item.msg.{Global.MSG_VERSION}")
                     .GetLangIdMap(name => {
                         var match = regex.Match(name);
                         var value = match.Groups[1].Value;
                         try {
                             return (uint) int.Parse(value);
                         } catch (Exception) {
                             throw new MSG.SkipReadException();
                         }
                     });
        DataHelper.ITEM_NAME_LOOKUP = msg;
        CreateAssetFile(msg, "ITEM_NAME_LOOKUP");
        CreateConstantsFile(msg[Global.LangIndex.eng].Flip(), "ItemConstants");

        regex = new(@"Item_IT_EXP_(\d+)");
        msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\GameDesign\Text\Excel_Data\Item.msg.{Global.MSG_VERSION}")
                 .GetLangIdMap(name => {
                     var match = regex.Match(name);
                     var value = match.Groups[1].Value;
                     try {
                         return (uint) int.Parse(value);
                     } catch (Exception) {
                         throw new MSG.SkipReadException();
                     }
                 });
        DataHelper.ITEM_DESC_LOOKUP = msg;
        CreateAssetFile(msg, "ITEM_DESC_LOOKUP");
    }

    private static void ExtractItemInfoByGuid() {
        var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\GameDesign\Text\Excel_Data\Item.msg.{Global.MSG_VERSION}")
                     .GetLangRawMap(name => {
                         try {
                             return name.id1;
                         } catch (Exception) {
                             throw new MSG.SkipReadException();
                         }
                     });
        DataHelper.ITEM_INFO_LOOKUP_BY_GUID = msg;
        CreateAssetFile(msg, "ITEM_INFO_LOOKUP_BY_GUID");
    }

    private static void ExtractArmorInfoByGuid() {
        var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\GameDesign\Text\Excel_Equip\Armor.msg.{Global.MSG_VERSION}")
                     .GetLangRawMap(name => {
                         try {
                             return name.id1;
                         } catch (Exception) {
                             throw new MSG.SkipReadException();
                         }
                     });
        DataHelper.ARMOR_INFO_LOOKUP_BY_GUID = msg;
        CreateAssetFile(msg, "ARMOR_INFO_LOOKUP_BY_GUID");

        // Get only the names, no descriptions.
        var regex = new Regex(@"Armor_ID(\d+)");
        msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\GameDesign\Text\Excel_Equip\Armor.msg.{Global.MSG_VERSION}")
                 .GetLangRawMap(name => {
                     if (!regex.Match(name.first).Success) {
                         throw new MSG.SkipReadException();
                     }
                     try {
                         return name.id1;
                     } catch (Exception) {
                         throw new MSG.SkipReadException();
                     }
                 });
        CreateConstantsFile(msg[Global.LangIndex.eng].Flip(), "ArmorConstants");
    }

    private static void ExtractWeaponInfoByGuid() {
        var allMsgs      = new List<Dictionary<Global.LangIndex, Dictionary<Guid, string>>>();
        var nameOnlyMsgs = new List<Dictionary<Global.LangIndex, Dictionary<Guid, string>>>();
        foreach (var weaponType in Global.WEAPON_TYPES) {
            var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\GameDesign\Text\Excel_Equip\{weaponType}.msg.{Global.MSG_VERSION}")
                         .GetLangRawMap(name => {
                             try {
                                 return name.id1;
                             } catch (Exception) {
                                 throw new MSG.SkipReadException();
                             }
                         });
            allMsgs.Add(msg);

            var regex = new Regex($@"{weaponType}_(\d+)");
            msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\GameDesign\Text\Excel_Equip\{weaponType}.msg.{Global.MSG_VERSION}")
                     .GetLangRawMap(name => {
                         if (!regex.Match(name.first).Success) {
                             throw new MSG.SkipReadException();
                         }
                         try {
                             return name.id1;
                         } catch (Exception) {
                             throw new MSG.SkipReadException();
                         }
                     });
            nameOnlyMsgs.Add(msg);
        }
        var mergedMsg = allMsgs.MergeDictionaries();
        CreateAssetFile(mergedMsg, "WEAPON_INFO_LOOKUP_BY_GUID");
        mergedMsg = nameOnlyMsgs.MergeDictionaries();
        CreateConstantsFile(mergedMsg[Global.LangIndex.eng].Flip(), "WeaponConstants");
    }
}