using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Common;

public static class Extensions {
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
        if (val.CompareTo(min) < 0) {
            return min;
        } else if (val.CompareTo(max) > 0) {
            return max;
        } else {
            return val;
        }
    }

    public static bool Is(this Type source, params Type[] types) {
        return types.Any(type => type.IsAssignableFrom(source));
    }

    public static T[] Subsequence<T>(this IEnumerable<T> arr, int startIndex, int length) {
        return arr.Skip(startIndex).Take(length).ToArray();
    }

    public static bool ContainsIgnoreCase(this IEnumerable<string> arr, string needle) {
        return arr.Any(s => string.Equals(s, needle, StringComparison.CurrentCultureIgnoreCase));
    }

    public static bool ContainsIgnoreCase(this string arr, string needle) {
        return arr.Contains(needle, StringComparison.CurrentCultureIgnoreCase);
    }

    public static V TryGet<K, V>(this IDictionary<K, V>? dict, K key, V defaultValue) {
        if (dict == null) return defaultValue;
        return dict.ContainsKey(key) ? dict[key] : defaultValue;
    }

    public static string TryGet<K>(this IDictionary<K, string>? dict, K key, string defaultValue = "Unknown") {
        if (dict == null) return defaultValue;
        return dict.ContainsKey(key) ? dict[key] : defaultValue;
    }

    public static T GetData<T>(this IEnumerable<byte> bytes) where T : struct {
        return bytes.GetData<T>(0, Marshal.SizeOf(default(T)));
    }

    public static T GetData<T>(this IEnumerable<byte> bytes, int offset) where T : struct {
        return bytes.GetData<T>(offset, Marshal.SizeOf(default(T)));
    }

    public static T GetData<T>(this IEnumerable<byte> bytes, int offset, int size) where T : struct {
        var subsequence = bytes.Subsequence(offset, size);
        var handle      = GCHandle.Alloc(subsequence, GCHandleType.Pinned);

        try {
            var rawDataPtr = handle.AddrOfPinnedObject();
            return (T) Marshal.PtrToStructure(rawDataPtr, typeof(T))!;
        } finally {
            handle.Free();
        }
    }

    public static string ToStringWithId<T>(this string name, T id, bool asHex = false) where T : struct {
        // ReSharper disable once InterpolatedStringExpressionIsNotIFormattable
        var s = asHex ? $"{id:X}" : id.ToString();
        return Global.showIdBeforeName ? $"{s}: {name}" : $"{name}: {s}";
    }

    public static string SHA512(this string fileName) {
        using var file = File.OpenRead(fileName);
        return file.SHA512();
    }

    public static string SHA512(this Stream stream) {
        using var sha512 = System.Security.Cryptography.SHA512.Create();
        var       hash   = sha512.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "");
    }

    public static string SHA512(this byte[] bytes) {
        using var sha512 = System.Security.Cryptography.SHA512.Create();
        var       hash   = sha512.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "");
    }

    public static Dictionary<K, V> Sort<K, V, O>(this Dictionary<K, V> dict, Func<KeyValuePair<K, V>, O> keySelector) where K : notnull {
        return dict.OrderBy(keySelector)
                   .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static Dictionary<K, V> SortDescending<K, V, O>(this Dictionary<K, V> dict, Func<KeyValuePair<K, V>, O> keySelector) where K : notnull {
        return dict.OrderByDescending(keySelector)
                   .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static Dictionary<K2, V> GetOrCreate<K1, K2, V>(this Dictionary<K1, Dictionary<K2, V>> dict, K1 key) where K1 : notnull
                                                                                                                where K2 : notnull {
        if (dict.ContainsKey(key)) return dict[key];
        dict[key] = new();
        return dict[key];
    }

    public static List<V> GetOrCreate<K, V>(this Dictionary<K, List<V>> dict, K key) where K : notnull {
        if (dict.ContainsKey(key)) return dict[key];
        dict[key] = new();
        return dict[key];
    }

    public static bool IsGeneric(this Type? source, Type genericType) {
        while (source != null && source != typeof(object)) {
            var cur = source.IsGenericType ? source.GetGenericTypeDefinition() : source;
            if (genericType == cur) {
                return true;
            }

            if (source.GetInterfaces().Any(@interface => @interface.IsGeneric(genericType))) return true;

            source = source.BaseType;
        }

        return false;
    }

    public static bool IsSigned(this Type type) {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return Type.GetTypeCode(type) switch {
            TypeCode.Byte => false,
            TypeCode.UInt16 => false,
            TypeCode.UInt32 => false,
            TypeCode.UInt64 => false,
            TypeCode.SByte => true,
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static T GetDataAs<T>(this IEnumerable<byte> bytes) {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

        try {
            var type                   = typeof(T);
            if (typeof(T).IsEnum) type = typeof(T).GetEnumUnderlyingType();

            return (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type)!;
        } finally {
            if (handle.IsAllocated) {
                handle.Free();
            }
        }
    }

    public static object GetDataAs(this IEnumerable<byte> bytes, Type type) {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

        try {
            var isEnum                  = type.IsEnum;
            var deserializeType         = type;
            if (isEnum) deserializeType = type.GetEnumUnderlyingType();

            var data = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), deserializeType)!;

            return isEnum ? Enum.ToObject(type, data) : data;
        } finally {
            if (handle.IsAllocated) {
                handle.Free();
            }
        }
    }

    public static byte[] GetBytes<T>(this T @struct) where T : notnull {
        if (@struct is bool b) {
            return new[] {(byte) (b ? 1 : 0)};
        }

        var type = @struct.GetType();
        if (type.IsEnum) {
            var enumType        = type.GetEnumUnderlyingType();
            var valueAsEnumType = Convert.ChangeType(@struct, enumType);
            var getBytes        = typeof(Extensions).GetMethod(nameof(GetBytes), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.MakeGenericMethod(enumType);
            var data            = (byte[]) (getBytes?.Invoke(null, new[] {valueAsEnumType}) ?? throw new("sub.GetDataAs failure."));
            return data;
        }

        var size   = Marshal.SizeOf(@struct);
        var bytes  = new byte[size];
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

        try {
            Marshal.StructureToPtr(@struct, handle.AddrOfPinnedObject(), false);
            return bytes;
        } finally {
            if (handle.IsAllocated) {
                handle.Free();
            }
        }
    }

    public static string ReadNullTermString(this BinaryReader reader) {
        var stringBytes = new List<byte>();
        do {
            stringBytes.Add(reader.ReadByte());
        } while (stringBytes[^1] != 0);

        return Encoding.UTF8.GetString(stringBytes.Subsequence(0, stringBytes.Count).ToArray());
    }

    public static string ReadNullTermWString(this BinaryReader reader, long offsetFromBeginning, bool stripNullTerm = true) {
        var pos = reader.BaseStream.Position;
        reader.BaseStream.Position = offsetFromBeginning;
        var @string = reader.ReadNullTermWString(stripNullTerm);
        reader.BaseStream.Position = pos;
        return @string;
    }

    public static string ReadNullTermWString(this BinaryReader reader, bool stripNullTerm = true) {
        var stringBytes = new List<byte>();
        do {
            stringBytes.AddRange(reader.ReadBytes(2));
        } while (stringBytes[^1] != 0 || stringBytes[^2] != 0);

        return Encoding.Unicode.GetString(stringBytes.Subsequence(0, stringBytes.Count - (stripNullTerm ? 2 : 0)).ToArray());
    }

    public static void WriteNullTermWString(this BinaryWriter writer, string str) {
        var bytes = Encoding.Unicode.GetBytes(str);
        writer.Write(bytes);
        writer.Write(new byte[] {0, 0}); // Null termination of Unicode which is 2 zero bytes.
    }

    public static char[] ToNullTermCharArray(this string? str) {
        str ??= "\0";
        if (!str.EndsWith("\0")) str += "\0";
        return str.ToCharArray();
    }

    public static List<byte> ReadRemainderAsByteArray(this BinaryReader reader) {
        var list = new List<byte>();
        while (reader.BaseStream.Position < reader.BaseStream.Length) {
            list.Add(reader.ReadByte());
        }
        return list;
    }

    public static void PadTill(this BinaryWriter writer, ulong targetPos) {
        while (writer.BaseStream.Position < (long) targetPos) {
            writer.Write((byte) 0);
        }
    }

    public static void PadTill(this BinaryWriter writer, Func<bool> predicate) {
        while (predicate.Invoke()) {
            writer.Write((byte) 0);
        }
    }

    public static void WriteValueAtOffset(this BinaryWriter writer, ulong value, long whereToWrite) {
        var tempPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(whereToWrite, SeekOrigin.Begin);
        writer.Write(value);
        writer.BaseStream.Seek(tempPos, SeekOrigin.Begin);
    }

    public static void WriteCurrentPositionAtOffset(this BinaryWriter writer, long whereToWrite) {
        var tempPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(whereToWrite, SeekOrigin.Begin);
        writer.Write(tempPos);
        writer.BaseStream.Seek(tempPos, SeekOrigin.Begin);
    }

    public static Dictionary<A, B> MergeDictionaries<A, B>(this IEnumerable<Dictionary<A, B>> dictionaries) where A : notnull {
        return dictionaries.SelectMany(dict => dict)
                           .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static Dictionary<A, Dictionary<B, C>> MergeDictionaries<A, B, C>(this IEnumerable<Dictionary<A, Dictionary<B, C>>> dictionaries) where A : notnull
                                                                                                                                             where B : notnull {
        var merged = new Dictionary<A, Dictionary<B, C>>();
        foreach (var dict in dictionaries) {
            foreach (var (key, value) in dict) {
                if (merged.ContainsKey(key)) {
                    merged[key] = (new List<Dictionary<B, C>> {merged[key], value}).MergeDictionaries();
                } else {
                    merged[key] = value;
                }
            }
        }
        return merged;
    }

    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> elements) {
        return elements.Aggregate(source, (current, element) => current.Append(element));
    }

    public static string? GetCSharpType(this StructJson.Field field) {
        return field.type switch {
            "Bool" => "bool",
            "S8" => "sbyte",
            "U8" => "byte",
            "S16" => "short",
            "U16" => "ushort",
            "S32" => "int",
            "U32" => "uint",
            "S64" => "long",
            "U64" => "ulong",
            "F32" => "float",
            "F64" => "double",
            "String" => "string",
            _ => null
        };
    }

    public static string? GetViaType(this string name) {
        switch (name.ToLower()) {
            case "vec2":
            case "via.vec2":
                return "ViaVec2";
            case "vec3":
            case "via.vec3":
                return "ViaVec3";
            case "quaternion":
            case "via.quaternion":
                return "Quaternion";
            case "float2":
            case "via.float2":
                return "Float2";
            case "float3":
            case "via.float3":
                return "Float3";
            default: return null;
        }
    }

    public static string ToUpperFirstLetter(this string source) {
        if (string.IsNullOrEmpty(source)) return string.Empty;
        var letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new(letters);
    }

    public static string? ToConvertedTypeName(this string? source, bool fixTypos = false) {
        if (source == null) return null;
        var name = source.ToUpperFirstLetter()
                         .Replace(".", "_")
                         .Replace("::", "_")
                         .Replace("[]", "");

        if (int.TryParse(name[0].ToString(), out _)) name = "_" + name; // If it starts with a number.
        while (name.EndsWith("k__BackingField")) name     = name.Substring(1, name.LastIndexOf('>') - 1); // Remove the k__BackingField.

        if (fixTypos) {
            name = name.Replace("Cariable", "Carryable")
                       .Replace("Evalution", "Evaluation");
        }

        return name;
    }

    public static string? ToConvertedFieldName(this string? name) {
        if (name == null) return null;
        while (name.StartsWith('_')) name = name[1..]; // Remove the leading '_'.
        while (name.EndsWith('_')) name   = name[..1]; // Remove the trailing '_'.

        name = name.ToConvertedTypeName(true)!;
        if (name == "Index") name = "_Index";
        return name;
    }

    /**
     * Returns a list of all the items in the give list matching the given type.
     * Essentially a type-as-a-variable way to call OfType().
     */
    public static object GetGenericItemsOfType<T>(this IReadOnlyList<T> rszObjectData, Type type) {
        return typeof(Enumerable).GetMethod(nameof(Enumerable.OfType), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                 ?.MakeGenericMethod(type)
                                 .Invoke(null, new object[] {rszObjectData}) ?? throw new("rsz.objectData.OfType failure.");
    }
}