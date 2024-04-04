﻿using System.Text.RegularExpressions;
using RE_Editor.Common;
using RE_Editor.Common.Models;

namespace RE_Editor.ID_Parser;

public static partial class Program {
    public static void Main() {
        ExtractItemInfo();
        ExtractShopNames();
    }

    private static void ExtractItemInfo() {
        var regex = new Regex(@"item_name_(\d+)");
        var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\message\ui\ItemName.msg.{Global.MSG_VERSION}")
                     .GetLangIdMap(name => {
                         var match = regex.Match(name);
                         var value = match.Groups[1].Value;
                         try {
                             return (uint) int.Parse(value);
                         } catch (Exception) {
                             throw new MSG.SkipReadException();
                         }
                     });
        CreateAssetFile(msg, "ITEM_NAME_LOOKUP");
        CreateConstantsFile(msg[Global.LangIndex.eng], "ItemConstants");
    }

    private static void ExtractShopNames() {
        var msg = MSG.Read($@"{PathHelper.CHUNK_PATH}\natives\STM\message\ui\AILocalAreaName.msg.{Global.MSG_VERSION}").GetLangGuidMap();
        CreateAssetFile(msg, "SHOP_NAME_LOOKUP");
        CreateConstantsFile(msg[Global.LangIndex.eng], "ShopConstants");
    }
}