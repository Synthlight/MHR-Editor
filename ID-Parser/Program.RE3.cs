﻿using System.Text.RegularExpressions;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models.Enums;

namespace RE_Editor.ID_Parser;

public static partial class Program {
    public static void Main() {
        ExtractItemInfo();
        ExtractWeaponInfo();
    }

    private static void ExtractItemInfo() {
        var regex = new Regex("Item_(sm\\d+_[^_]+?)$");
        var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\Escape\Message\Mes_Item.msg.{Global.MSG_VERSION}")
                     .GetLangIdMap(name => {
                         var match = regex.Match(name);
                         var value = match.Groups[1].Value.ToLower();
                         try {
                             return (uint) (int) Enum.Parse(typeof(Offline_gamemastering_Item_ID), value);
                         } catch (Exception) {
                             throw new MSG.SkipReadException();
                         }
                     });
        CreateAssetFile(msg, "ITEM_NAME_LOOKUP");
        CreateConstantsFile(msg[Global.LangIndex.eng].Flip(), "ItemConstants");
    }

    private static void ExtractWeaponInfo() {
        var regex = new Regex("Weapon_(wp\\d+)$");
        var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\Escape\Message\Mes_Weapon.msg.{Global.MSG_VERSION}", true)
                     .GetLangIdMap(name => {
                         var match = regex.Match(name);
                         var value = match.Groups[1].Value.ToUpper();
                         try {
                             return (uint) (int) Enum.Parse(typeof(Offline_EquipmentDefine_WeaponType), value);
                         } catch (Exception) {
                             throw new MSG.SkipReadException();
                         }
                     });
        CreateAssetFile(msg, "WEAPON_NAME_LOOKUP");

        var dict = new Dictionary<Offline_EquipmentDefine_WeaponType, string>();
        foreach (var (id, name) in msg[Global.LangIndex.eng]) {
            dict[(Offline_EquipmentDefine_WeaponType) id] = name;
        }
        CreateConstantsFile(dict.Flip(), "WeaponConstants");
    }
}