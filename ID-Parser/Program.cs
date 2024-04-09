﻿using System.CodeDom;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using Newtonsoft.Json;
using RE_Editor.Common;

namespace RE_Editor.ID_Parser;

public static partial class Program {
    public const  string BASE_PROJ_PATH = @"..\..\..";
    private const string CONSTANTS_DIR  = $@"{BASE_PROJ_PATH}\RE-Editor\Constants\{PathHelper.CONFIG_NAME}";
    private const string ASSETS_DIR     = $@"{BASE_PROJ_PATH}\RE-Editor\Data\{PathHelper.CONFIG_NAME}\Assets";

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

    private static void CreateAssetFile(object msg, string filename) {
        Directory.CreateDirectory(ASSETS_DIR);
        File.WriteAllText($@"{ASSETS_DIR}\{filename}.json", JsonConvert.SerializeObject(msg, Formatting.Indented));
    }

    private static void CreateConstantsFile<T>(Dictionary<string, T> engDict, string className, bool asHex = false) where T : notnull {
        Directory.CreateDirectory(CONSTANTS_DIR);
        using var writer = new StreamWriter(File.Create($@"{CONSTANTS_DIR}\{className}.cs"));
        writer.WriteLine("// ReSharper disable All");
        writer.WriteLine("using System;");
        writer.WriteLine("using System.Diagnostics.CodeAnalysis;");
        writer.WriteLine("using RE_Editor.Models.Enums;");
        writer.WriteLine("");
        writer.WriteLine("namespace RE_Editor.Constants;");
        writer.WriteLine("");
        writer.WriteLine("[SuppressMessage(\"ReSharper\", \"InconsistentNaming\")]");
        writer.WriteLine("[SuppressMessage(\"ReSharper\", \"UnusedMember.Global\")]");
        writer.WriteLine("[SuppressMessage(\"ReSharper\", \"IdentifierTypo\")]");
        writer.WriteLine($"public static class {className} {{");
        var compiler  = new CSharpCodeProvider();
        var regex     = new Regex(@"^\d");
        var namesUsed = new List<string?>(engDict.Count);
        engDict = engDict.Sort(pair => pair.Key);
        foreach (var (name, value) in engDict) {
            if (name.ToLower().Contains("#rejected#")
                || name.ToLower().Contains('?')) {
                continue;
            }
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
            if (regex.Match(constName).Success) constName = $"_{constName}";
            if (namesUsed.Contains(constName)) continue;
            namesUsed.Add(constName);
            if (typeof(T) == typeof(Guid)) {
                writer.WriteLine($"    public static readonly Guid {constName} = Guid.Parse(\"{value}\");");
            } else if (typeof(T) == typeof(string)) {
                writer.WriteLine($"    public const string {constName} = \"{value}\";");
            } else if (typeof(T).IsEnum) {
                writer.WriteLine($"    public const {typeof(T).Name} {constName} = {typeof(T).Name}.{value};");
            } else {
                var type     = new CodeTypeReference(typeof(T));
                var typeName = compiler.GetTypeOutput(type);
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (asHex) {
                    writer.WriteLine($"    public const {typeName} {constName} = 0x{value:X8};");
                } else {
                    writer.WriteLine($"    public const {typeName} {constName} = {value};");
                }
            }
        }
        writer.WriteLine("}");
    }
}