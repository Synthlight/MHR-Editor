using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MHR_Editor.Data;
using MHR_Editor.Models.Structs;

namespace MHR_Editor.Models;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
[SuppressMessage("ReSharper", "NotAccessedField.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RszObject : INotifyPropertyChanged {
    public             StructJson                 structInfo;
    protected readonly Dictionary<int, FieldData> fieldData = new();
    public             int                        Index { get; set; }

#pragma warning disable CS0067 // The event is never used
    public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

    public static RszObject Read(BinaryReader reader, uint hash) {
        var       structInfo = DataHelper.STRUCT_INFO[hash];
        RszObject rszObject;
        if (hash == Armor.HASH) rszObject           = new Armor();
        else if (hash == Decoration.HASH) rszObject = new Decoration();
        else if (hash == GreatSword.HASH) rszObject = new GreatSword();
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
        var list = new List<T>(fieldData.fieldInfo.size);
        var size = fieldData.fieldInfo.size;

        for (var i = 0; i < fieldData.arrayCount; i++) {
            var startPos = i * size;
            var sub      = fieldData.data.Subsequence(startPos, size);
            var asType   = sub.GetDataAs<T>();
            list.Add(asType);
        }

        return list;
    }

    public static void SetDataFromList<T>(this RszObject.FieldData fieldData, List<T> list) {
        var size      = fieldData.fieldInfo.size;
        var byteCount = list.Count * size;
        var bytes     = new List<byte>(byteCount);

        foreach (var entry in list) {
            var data = entry.GetBytes();
            bytes.AddRange(data);
        }

        if (bytes.Count != byteCount) throw new InvalidOperationException($"Resultant byte data array size is unexpected: found: `{bytes.Count}`, expected: `{byteCount}`.");

        fieldData.arrayCount = list.Count;
        fieldData.data       = bytes.ToArray();
    }

    public static RszObject.FieldData getFieldByName(this Dictionary<int, RszObject.FieldData> fieldData, string name) {
        foreach (var value in fieldData.Values) {
            if (value.fieldInfo.name == name) return value;
        }
        return null;
    }
}