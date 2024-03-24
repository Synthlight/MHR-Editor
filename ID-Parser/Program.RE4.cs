using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models.Enums;

namespace RE_Editor.ID_Parser;

public static partial class Program {
    public const string CONFIG_NAME = "RE4";

    public static void Main() {
        ExtractItemInfo();
        ExtractWeaponInfo();
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractItemInfo() {
        for (var i = 0; i < Global.VARIANTS.Count; i++) {
            var variant = Global.VARIANTS[i];
            var folder  = Global.FOLDERS[i];
            var regex   = new Regex("(?:CH|MC|AO)_Mes_Main_(WEAPON_NAME|ITEM_NAME)_(.+?)_000");
            var msgs = new List<string> {
                           $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name.msg.{Global.MSG_VERSION}",
                           $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name_Misc.msg.{Global.MSG_VERSION}",
                       }
                       .Where(File.Exists)
                       .Select(file => MSG.Read(file)
                                          .GetLangIdMap(name => {
                                              var match   = regex.Match(name);
                                              var subType = match.Groups[1].Value;
                                              var value   = match.Groups[2].Value.ToLower();
                                              value = subType switch {
                                                  "ITEM_NAME" => $"it_sm{value}",
                                                  "WEAPON_NAME" => $"it_{value}",
                                                  _ => value
                                              };
                                              try {
                                                  return (uint) (int) Enum.Parse(typeof(Chainsaw_ItemID), value);
                                              } catch (Exception) {
                                                  throw new MSG.SkipReadException();
                                              }
                                          }))
                       .ToList();
            var msg = msgs.MergeDictionaries();
            CreateAssetFile(msg, $@"{variant}_ITEM_NAME_LOOKUP");
            CreateConstantsFile(msg[Global.LangIndex.eng], $"ItemConstants_{variant}");
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractWeaponInfo() {
        for (var i = 0; i < Global.VARIANTS.Count; i++) {
            var variant = Global.VARIANTS[i];
            var folder  = Global.FOLDERS[i];
            var regex   = new Regex("(?:CH|MC|AO)_Mes_Main_WEAPON_NAME_([a-zA-Z0-9]+?)_");
            var msg = new List<string> {
                          $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name.msg.{Global.MSG_VERSION}",
                          $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name_Misc.msg.{Global.MSG_VERSION}",
                      }
                      .Where(File.Exists)
                      .Select(file => MSG.Read(file)
                                         .GetLangIdMap(name => {
                                             var match = regex.Match(name);
                                             if (!match.Success) throw new MSG.SkipReadException();
                                             var value = match.Groups[1].Value.ToLower();
                                             try {
                                                 return (uint) (int) Enum.Parse(typeof(Chainsaw_WeaponID), value);
                                             } catch (Exception) {
                                                 throw new MSG.SkipReadException();
                                             }
                                         }))
                      .ToList()
                      .MergeDictionaries();
            CreateAssetFile(msg, $@"{variant}_WEAPON_NAME_LOOKUP");
            CreateConstantsFile(msg[Global.LangIndex.eng], $"WeaponConstants_{variant}");
        }
    }
}