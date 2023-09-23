using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models.Enums;

namespace RE_Editor.ID_Parser;

public static class Program {
    public const string BASE_PROJ_PATH = @"..\..\..";
    public const string MSG_VERSION    = "22";

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
                           $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name.msg.{MSG_VERSION}",
                           $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name_Misc.msg.{MSG_VERSION}",
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
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\{variant}_ITEM_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

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
                          $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name.msg.{MSG_VERSION}",
                          $@"{PathHelper.CHUNK_PATH}\natives\STM\{folder}\Message\Mes_Main_Item\{variant}_Mes_Main_Item_Name_Misc.msg.{MSG_VERSION}",
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
            File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\{variant}_WEAPON_NAME_LOOKUP.json", JsonConvert.SerializeObject(msg, Formatting.Indented));

            CreateConstantsFile(msg[Global.LangIndex.eng], $"WeaponConstants_{variant}");
        }
    }

    public static uint ParseEnum(Type enumType, string value) {
        return (uint) Convert.ChangeType(Enum.Parse(enumType, value), typeof(uint));
    }

    /**
     * Aids in finding the enum value *in the enum name itself* to get the value of the last entry before the `Max` entry.
     */
    public static int GetOneBelowMax<T>(string toFind) where T : struct, Enum {
        var names = Enum.GetNames<T>();
        for (var i = 0; i < names.Length; i++) {
            if (names[i] == toFind) {
                var target = names[i - 1];
                var regex  = new Regex(@"(\d+)");
                var match  = regex.Match(target);
                return int.Parse(match.Groups[1].Value);
            }
        }
        throw new KeyNotFoundException($"Cannot find `{toFind}` in the enum `{typeof(T)}`.");
    }

    private static void CreateConstantsFile(Dictionary<uint, string> engDict, string className, bool asHex = false) {
        using var writer = new StreamWriter(File.Create($@"{BASE_PROJ_PATH}\Constants\{className}.cs"));
        writer.WriteLine("using System.Diagnostics.CodeAnalysis;");
        writer.WriteLine("");
        writer.WriteLine("namespace RE_Editor.Constants;");
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
                                .Replace(' ', '_')
                                .Replace(':', '_');
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