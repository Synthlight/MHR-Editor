using System.Text.RegularExpressions;

namespace RE_Editor.ID_Parser;

public static class Program {
    public const string BASE_PROJ_PATH = @"..\..\..";
    public const string MSG_VERSION    = "";

    public static void Main() {
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