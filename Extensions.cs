using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MHR_Editor.Windows;

namespace MHR_Editor;

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

    public static V TryGet<K, V>(this IDictionary<K, V> dict, K key, V defaultValue) {
        if (dict == null) return defaultValue;
        return dict.ContainsKey(key) ? dict[key] : defaultValue;
    }

    public static string TryGet<K>(this IDictionary<K, string> dict, K key, string defaultValue = "Unknown") {
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

    public static Dictionary<K, V> Sort<K, V, O>(this Dictionary<K, V> dict, Func<KeyValuePair<K, V>, O> keySelector) {
        return dict.OrderBy(keySelector)
                   .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static Dictionary<K, V> SortDescending<K, V, O>(this Dictionary<K, V> dict, Func<KeyValuePair<K, V>, O> keySelector) {
        return dict.OrderByDescending(keySelector)
                   .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static Dictionary<K2, V> GetOrCreate<K1, K2, V>(this Dictionary<K1, Dictionary<K2, V>> dict, K1 key) {
        if (dict.ContainsKey(key)) return dict[key];
        dict[key] = new();
        return dict[key];
    }

    public static List<V> GetOrCreate<K, V>(this Dictionary<K, List<V>> dict, K key) {
        if (dict.ContainsKey(key)) return dict[key];
        dict[key] = new();
        return dict[key];
    }

    public static bool IsGeneric(this Type source, Type genericType) {
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

    public static void AddControl(this Grid grid, UIElement control) {
        var rowDefinition = new RowDefinition {Height = GridLength.Auto};
        grid.RowDefinitions.Add(rowDefinition);
        Grid.SetRow(control, grid.RowDefinitions.Count - 1);
        grid.Children.Add(control);
    }

    public static T GetDataAs<T>(this IEnumerable<byte> bytes) {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

        try {
            return (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        } finally {
            if (handle.IsAllocated) {
                handle.Free();
            }
        }
    }

    public static byte[] GetBytes<T>(this T @struct) {
        if (@struct is bool b) {
            return new[] {(byte) (b ? 1 : 0)};
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

    public static string ToStringWithId<T>(this string name, T id) where T : struct {
        return MainWindow.showIdBeforeName ? $"{id}: {name}" : $"{name}: {id}";
    }

    public static Visibility VisibleIfTrue(this bool b) {
        return b ? Visibility.Visible : Visibility.Collapsed;
    }

    public static T GetParent<T>(this DependencyObject d) where T : class {
        while (d != null && d is not T) {
            d = VisualTreeHelper.GetParent(d);
        }

        return d as T;
    }

    public static string ReadNullTermString(this BinaryReader reader) {
        var stringBytes = new List<byte>();
        do {
            stringBytes.Add(reader.ReadByte());
        } while (stringBytes[^1] != 0);

        return Encoding.UTF8.GetString(stringBytes.Subsequence(0, stringBytes.Count).ToArray());
    }

    public static char[] ToNullTermCharArray(this string str) {
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
}