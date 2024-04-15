using System.Text.RegularExpressions;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models.Enums;

namespace RE_Editor.ID_Parser;

public static partial class Program {
    public static void Main() {
        ExtractItemInfo();
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
}