using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using MHR_Editor.Data;
using MHR_Editor.Models.List_Wrappers;
using MHR_Editor.Models.Structs;

namespace MHR_Editor.Models;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
[SuppressMessage("ReSharper", "NotAccessedField.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RszObject : OnPropertyChangedBase {
    public             StructJson                 structInfo;
    protected readonly Dictionary<int, FieldData> fieldData = new();
    public             int                        Index { get; set; }

    public static RszObject Read(BinaryReader reader, uint hash) {
        var       structInfo = DataHelper.STRUCT_INFO[hash];
        RszObject rszObject;
        if (hash == Armor.HASH) rszObject           = new Armor();
        else if (hash == Decoration.HASH) rszObject = new Decoration();
        else if (hash == GreatSword.HASH) rszObject = new GreatSword();
        else if (hash == Item.HASH) rszObject       = new Item();
        else rszObject                              = new();
        rszObject.structInfo = structInfo;

        for (var i = 0; i < structInfo.fields.Count; i++) {
            var field = structInfo.fields[i];

            while (reader.BaseStream.Position % field.align != 0) {
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
            writer.PadTill(() => writer.BaseStream.Position % data.fieldInfo.align != 0);
            if (data.isArray) writer.Write(data.arrayCount);
            writer.Write(data.data);
        }
    }

    public override string ToString() {
        return structInfo.name;
    }

    public class FieldData {
        public byte[]           data;
        public bool             isArray;
        public int              arrayCount;
        public long             position;
        public StructJson.Field fieldInfo;

        public override string ToString() {
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
                var getDataAs = typeof(Extensions).GetMethod(nameof(Extensions.GetDataAs), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.MakeGenericMethod(genericType);
                var data      = getDataAs?.Invoke(null, new object[] {sub}) ?? throw new("sub.GetDataAs failure.");
                var wrapper   = (T) Activator.CreateInstance(typeof(T), i, data);
                list.Add(wrapper);
            } else {
                list.Add(sub.GetDataAs<T>());
            }
        }

        return list;
    }

    public static void SetDataFromList<T>(this RszObject.FieldData fieldData, IList<T> list) {
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

    public static RszObject.FieldData getFieldByName(this Dictionary<int, RszObject.FieldData> fieldData, string name) {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var value in fieldData.Values) {
            if (value.fieldInfo.name == name) return value;
        }
        return null;
    }
}