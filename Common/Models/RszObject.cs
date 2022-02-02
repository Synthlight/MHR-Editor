using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using MHR_Editor.Common.Attributes;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models.List_Wrappers;

#pragma warning disable CS8600
#pragma warning disable CS8618

namespace MHR_Editor.Common.Models;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class RszObject : OnPropertyChangedBase {
    public             StructJson                 structInfo;
    protected readonly Dictionary<int, FieldData> fieldData = new();
    public             RSZ                        rsz;
    public             int                        userDataRef = -1;

    [SortOrder(int.MaxValue - 1000)]
    public int Index { get; set; }

    public static RszObject Read(BinaryReader reader, uint hash, RSZ rsz, int userDataRef) {
        if (userDataRef > -1) {
            return new UserDataShell() {
                userDataRef = userDataRef,
                rsz         = rsz
            };
        }

        var structInfo = DataHelper.STRUCT_INFO[hash];
        var rszType    = DataHelper.MHR_STRUCTS.TryGet(hash, typeof(RszObject));
        var rszObject  = (RszObject) Activator.CreateInstance(rszType) ?? new RszObject();
        rszObject.structInfo = structInfo;
        rszObject.rsz        = rsz;

        for (var i = 0; i < structInfo.fields!.Count; i++) {
            var field = structInfo.fields[i];

            // Be careful with lists. The 'align' in them refers to their contents, not their count themselves, which is always a 4-aligned int.
            var align = field.array ? 4 : field.align;
            while (reader.BaseStream.Position % align != 0) {
                reader.BaseStream.Seek(1, SeekOrigin.Current);
            }

            var fieldData = new FieldData {
                position  = reader.BaseStream.Position,
                fieldInfo = field
            };

            if (field.array) {
                fieldData.isArray    = true;
                fieldData.arrayCount = reader.ReadInt32();
                fieldData.data       = reader.ReadBytes(field.size * fieldData.arrayCount);
            } else {
                fieldData.data = reader.ReadBytes(field.size);
            }
            rszObject.fieldData[i] = fieldData;
        }
        rszObject.Init();
        return rszObject;
    }

    protected virtual void Init() {
    }

    protected virtual void PreWrite() {
    }

    public void Write(BinaryWriter writer) {
        PreWrite();

        foreach (var data in fieldData.Values) {
            writer.PadTill(() => {
                var align = data.fieldInfo.array ? 4 : data.fieldInfo.align;
                return writer.BaseStream.Position % align != 0;
            });
            if (data.isArray) writer.Write(data.arrayCount);
            writer.Write(data.data);
        }
    }

    public override string? ToString() {
        return structInfo.name;
    }

    public class FieldData {
        public byte[]           data;
        public bool             isArray;
        public int              arrayCount;
        public long             position;
        public StructJson.Field fieldInfo;

        public override string? ToString() {
            return fieldInfo.ToString();
        }
    }
}

public static class RszObjectExtensions {
    public static List<T> GetDataAsList<T>(this RszObject.FieldData fieldData) {
        var genericArgs = typeof(T).GenericTypeArguments;
        var isGeneric   = genericArgs.Length != 0;
        var genericType = isGeneric ? genericArgs[0] : null;
        var list        = new List<T>(fieldData.fieldInfo.size);
        var size        = fieldData.fieldInfo.size;

        for (var i = 0; i < fieldData.arrayCount; i++) {
            var startPos = i * size;
            var sub      = fieldData.data.Subsequence(startPos, size);
            if (isGeneric) {
                var getDataAs = typeof(Extensions).GetMethod(nameof(Extensions.GetDataAs), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.MakeGenericMethod(genericType!);
                var data      = getDataAs?.Invoke(null, new object[] {sub}) ?? throw new("sub.GetDataAs failure.");
                T   wrapper;
                if (typeof(T).GetGenericTypeDefinition().Is(typeof(DataSourceWrapper<>))) {
                    wrapper = (T) Activator.CreateInstance(typeof(T), i, data, fieldData.fieldInfo)!;
                } else {
                    wrapper = (T) Activator.CreateInstance(typeof(T), i, data)!;
                }
                list.Add(wrapper);
            } else {
                list.Add(sub.GetDataAs<T>());
            }
        }

        return list;
    }

    public static void SetDataFromList<T>(this RszObject.FieldData fieldData, IList<T> list) where T : notnull {
        var size      = fieldData.fieldInfo.size;
        var byteCount = list.Count * size;
        var bytes     = new List<byte>(byteCount);

        foreach (var entry in list) {
            byte[] data;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (entry.GetType().IsGeneric(typeof(IListWrapper<>))) {
                var  value     = ((dynamic) entry).Value;
                Type valueType = value.GetType();
                var  getBytes  = typeof(Extensions).GetMethod(nameof(Extensions.GetBytes), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.MakeGenericMethod(valueType);
                data = (byte[]) (getBytes?.Invoke(null, new object[] {value}) ?? throw new("sub.GetDataAs failure."));
            } else {
                data = entry.GetBytes();
            }
            bytes.AddRange(data);
        }

        if (bytes.Count != byteCount) throw new InvalidOperationException($"Resultant byte data array size is unexpected: found: `{bytes.Count}`, expected: `{byteCount}`.");

        fieldData.arrayCount = list.Count;
        fieldData.data       = bytes.ToArray();
    }

    public static RszObject.FieldData? getFieldByName(this Dictionary<int, RszObject.FieldData> fieldData, string name) {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var value in fieldData.Values) {
            if (value.fieldInfo.name == name) return value;
        }
        return null;
    }
}