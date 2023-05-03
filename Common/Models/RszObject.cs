using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Data;
using RE_Editor.Common.Models.List_Wrappers;

#pragma warning disable CS8600
#pragma warning disable CS8618

namespace RE_Editor.Common.Models;

[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
public class RszObject : OnPropertyChangedBase {
    public                   StructJson structInfo;
    [DisplayName("")] public RSZ        rsz { get; private set; }
    protected                int        userDataRef = -1;
    public                   int        objectInstanceIndex; // 0 is invalid, field is one based. This is assigned during write so the order is correct.
    private                  long       pos; // Used during testing to make sure read/write without altering anything is written in the same spot.

    [SortOrder(int.MaxValue - 1000)]
    public int Index { get; set; }

    public virtual T Copy<T>() where T : RszObject {
        var obj = (T) Activator.CreateInstance(typeof(T), null)!;
        obj.rsz         = rsz;
        obj.userDataRef = userDataRef;
        obj.structInfo  = structInfo;
        return obj;
    }

    /// <summary>
    /// To be used to init required fields when manually instancing a generated class.
    /// </summary>
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public static T Create<T>(RSZ rsz, uint hash) where T : RszObject {
        var structInfo = DataHelper.STRUCT_INFO[hash];
        var rszObject  = CreateRszObjectInstance(hash);
        rszObject.structInfo = structInfo;
        rszObject.rsz        = rsz;
        return (T) rszObject;
    }

    public static RszObject Read(BinaryReader reader, uint hash, RSZ rsz, int userDataRef) {
        if (userDataRef > -1) {
            return new UserDataShell {
                userDataRef = userDataRef,
                rsz         = rsz
            };
        }

        if (!DataHelper.STRUCT_INFO.ContainsKey(hash)) {
            Debug.WriteLine($"Unknown hash: {hash:X}");
            throw new FileNotSupported();
        }

        var structInfo = DataHelper.STRUCT_INFO[hash];
        var rszObject  = CreateRszObjectInstance(hash);
        rszObject.structInfo = structInfo;
        rszObject.rsz        = rsz;
        rszObject.pos        = reader.BaseStream.Position;

        switch (rszObject) {
            case ICustomReadWrite customReadWrite:
                customReadWrite.Read(reader);
                return rszObject;
            case IViaType viaTypeAsObject:
                viaTypeAsObject.Read(reader);
                return rszObject;
        }

        for (var i = 0; i < structInfo.fields!.Count; i++) {
            var field            = structInfo.fields[i];
            var fieldName        = field.name?.ToConvertedFieldName()!;
            var primitiveName    = field.GetCSharpType();
            var viaType          = field.type?.GetViaType().AsType();
            var isUserData       = field.type == "UserData";
            var isNonPrimitive   = primitiveName == null;
            var isObjectType     = field.type == "Object";
            var isStringType     = field.type == "String";
            var fieldInfo        = rszObject.GetType().GetProperty(fieldName)!;
            var fieldType        = fieldInfo.PropertyType;
            var fieldGenericType = fieldType.IsGenericType ? fieldType.GenericTypeArguments[0] : null; // GetInnermostGenericType(fieldType);
            var fieldSetMethod   = fieldInfo.SetMethod!;

            // Be careful with lists. The 'align' in them refers to their contents, not their count themselves, which is always a 4-aligned int.
            var align = field.GetAlign();
            reader.BaseStream.Align(align);

            if (field.array) {
                var arrayCount = reader.ReadInt32();

                if (isObjectType) { // Array of pointers.
                    var objects = new List<RszObject>();
                    for (var index = 0; index < arrayCount; index++) {
                        objects.Add(rsz.objectData[reader.ReadInt32() - 1]);
                    }
                    var items = objects.GetGenericItemsOfType(fieldGenericType!);
                    SetList(items, fieldSetMethod, rszObject);
                } else if (isStringType) { // Array of strings.
                    var strings = new List<GenericWrapper<string>>(arrayCount);
                    for (var s = 0; s < arrayCount; s++) {
                        reader.BaseStream.Align(field.align);
                        strings.Add(new(s, reader.ReadWString()));
                    }
                    SetList(strings, fieldSetMethod, rszObject);
                } else if (isNonPrimitive) { // Array of embedded objects. (Built-in types like via.vec2.)
                    reader.BaseStream.Align(field.align);
                    var objects = new List<IViaType>(arrayCount);
                    for (var s = 0; s < arrayCount; s++) {
                        var instance = (IViaType) Activator.CreateInstance(viaType!)!;
                        instance.Read(reader);
                        objects.Add(instance);
                    }
                    var items = objects.GetGenericItemsOfType(fieldGenericType!);
                    SetList(items, fieldSetMethod, rszObject);
                } else { // Primitive array.
                    var bytes         = reader.ReadBytes(field.size * arrayCount);
                    var genericMethod = typeof(RszObjectExtensions).GetMethod(nameof(RszObjectExtensions.GetDataAsList))!.MakeGenericMethod(fieldGenericType!);
                    var items         = genericMethod.Invoke(null, new object[] {bytes, field.size, arrayCount, field})!;
                    SetList(items, fieldSetMethod, rszObject);
                }
            } else {
                if (isObjectType || isUserData) { // Pointer to object.
                    var objectIndex = reader.ReadInt32() - 1; // Will be `0` for some `UserData` entries with no data in them.
                    if (objectIndex == -1) continue; // In which case just move onto the next field.
                    var objects = new List<RszObject> {rsz.objectData[objectIndex]};
                    var items   = objects.GetGenericItemsOfType(fieldGenericType!);
                    SetList(items, fieldSetMethod, rszObject);
                } else if (isStringType) { // A string.
                    var str = reader.ReadWString();
                    fieldSetMethod.Invoke(rszObject, new object?[] {str});
                } else if (isNonPrimitive) { // Embedded object. (A built-in type like via.vec2.)
                    var instance = (IViaType) Activator.CreateInstance(viaType!)!;
                    instance.Read(reader);
                    var items = new List<IViaType> {instance}.GetGenericItemsOfType(fieldGenericType!);
                    SetList(items, fieldSetMethod, rszObject); // Treated as a list so we have an 'open' button.
                } else { // A primitive.
                    var bytes = reader.ReadBytes(field.size);
                    var data  = bytes.GetDataAs(fieldType);
                    fieldSetMethod.Invoke(rszObject, new[] {data});
                }
            }
        }
        return rszObject;
    }

    private static void SetList(object items, MethodBase fieldSetMethod, RszObject rszObject) {
        var data = MakeGenericObservableCollection((dynamic) items);
        fieldSetMethod.Invoke(rszObject, new[] {data});
    }

    public static ObservableCollection<T> MakeGenericObservableCollection<T>(IEnumerable<T> itemSource) {
        return itemSource as ObservableCollection<T> ?? new(itemSource);
    }

    /**
     * If the hash isn't found it'll just return the base `RszObject`.
     */
    private static RszObject CreateRszObjectInstance(uint hash) {
        var rszType   = DataHelper.RE_STRUCTS.TryGet(hash, typeof(RszObject));
        var rszObject = (RszObject) Activator.CreateInstance(rszType) ?? new RszObject();
        return rszObject;
    }

    /**
     * Run before writing to setup all the instance info / indexes so we know exactly where an object is being written.
     * This is how we know what to point an 'object' field to.
     */
    public void SetupInstanceInfo(List<InstanceInfo> instanceInfo, bool forGp) {
        for (var i = 0; i < structInfo.fields!.Count; i++) {
            var field          = structInfo.fields[i];
            var fieldName      = field.name?.ToConvertedFieldName()!;
            var fieldInfo      = GetType().GetProperty(fieldName)!;
            var isObjectType   = field.type == "Object";
            var fieldGetMethod = fieldInfo.GetMethod!;

            if (isObjectType) {
                if (field.array) { // Array of pointers.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    foreach (RszObject obj in list) {
                        obj.SetupInstanceInfo(instanceInfo, forGp);
                    }
                } else { // Pointer to object.
                    // So it works in the UI, we always put the object in a list. Thus even if not an array, we need to extract from a list.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    ((RszObject) list[0]!).SetupInstanceInfo(instanceInfo, forGp);
                }
            }
        }

        var hash = (uint) GetType().GetField("HASH")!.GetValue(null)!;
        var crc  = uint.Parse(structInfo.crc!, NumberStyles.HexNumber);

        if (forGp && DataHelper.GP_CRC_OVERRIDE_INFO.TryGetValue(hash, out var value)) {
            crc = value;
        }

        instanceInfo.Add(new() {
            hash = hash,
            crc  = crc
        });

        objectInstanceIndex = instanceInfo.Count - 1;
    }

    public void Write(BinaryWriter writer, bool testWritePosition) {
        // Do once to write all child objects first.
        for (var i = 0; i < structInfo.fields!.Count; i++) {
            var field          = structInfo.fields[i];
            var fieldName      = field.name?.ToConvertedFieldName()!;
            var fieldInfo      = GetType().GetProperty(fieldName)!;
            var isObjectType   = field.type == "Object";
            var fieldGetMethod = fieldInfo.GetMethod!;

            if (isObjectType) {
                if (field.array) { // Array of pointers.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    foreach (var obj in list) {
                        ((RszObject) obj).Write(writer, testWritePosition);
                    }
                } else { // Pointer to object.
                    // So it works in the UI, we always put the object in a list. Thus even if not an array, we need to extract from a list.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    ((RszObject) list[0]!).Write(writer, testWritePosition);
                }
            }
        }

        if (testWritePosition) {
            Debug.Assert(pos == writer.BaseStream.Position, $"Expected {pos}, found {writer.BaseStream.Position}.");
        }

        switch (this) {
            case ICustomReadWrite customReadWrite:
                customReadWrite.Write(writer);
                return;
            case IViaType viaTypeAsObject:
                viaTypeAsObject.Write(writer);
                return;
        }

        for (var i = 0; i < structInfo.fields!.Count; i++) {
            var field          = structInfo.fields[i];
            var fieldName      = field.name?.ToConvertedFieldName()!;
            var primitiveName  = field.GetCSharpType();
            var isNonPrimitive = primitiveName == null;
            var isObjectType   = field.type == "Object";
            var isStringType   = field.type == "String";
            var fieldInfo      = GetType().GetProperty(fieldName)!;
            var fieldGetMethod = fieldInfo.GetMethod!;

            // Be careful with lists. The 'align' in them refers to their contents, not their count themselves, which is always a 4-aligned int.
            var align = field.GetAlign();
            writer.PadTill(() => writer.BaseStream.Position % align != 0);

            if (field.array) {
                if (isObjectType) { // Array of pointers.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    writer.Write(list.Count);
                    foreach (RszObject obj in list) {
                        writer.Write(obj.objectInstanceIndex);
                    }
                } else if (isStringType) { // Array of strings.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    writer.Write(list.Count);
                    foreach (GenericWrapper<string> obj in list) {
                        writer.BaseStream.Align(field.align);
                        writer.WriteWString(obj.Value);
                    }
                } else if (isNonPrimitive) { // Array of embedded objects. (Built-in types like via.vec2.)
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    writer.Write(list.Count);
                    writer.BaseStream.Align(field.align);
                    foreach (var obj in list) {
                        ((IViaType) obj).Write(writer);
                    }
                } else { // Primitive array.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    writer.Write(list.Count);
                    foreach (var obj in list) {
                        byte[] bytes;
                        if (obj.GetType().IsGeneric(typeof(IListWrapper<>))) {
                            var value = ((dynamic) obj).Value;
                            bytes = Extensions.GetBytes(value);
                        } else {
                            bytes = obj.GetBytes();
                        }
                        writer.Write(bytes);
                    }
                }
            } else {
                if (isObjectType) { // Pointer to object.
                    var obj = (RszObject) ((dynamic) fieldGetMethod.Invoke(this, null)!)[0];
                    writer.Write(obj.objectInstanceIndex);
                } else if (isStringType) { // Array of strings.
                    var str = (string) fieldGetMethod.Invoke(this, null)!;
                    writer.WriteWString(str);
                } else if (isNonPrimitive) { // Embedded object. (A built-in type like via.vec2.)
                    // So it works in the UI, we always put the object in a list. Thus even if not an array, we need to extract from a list.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    ((IViaType) list[0]!).Write(writer);
                } else { // A primitive.
                    var obj   = fieldGetMethod.Invoke(this, null)!;
                    var bytes = obj.GetBytes();
                    writer.Write(bytes);
                }
            }
        }
    }

    public void WriteObjectList(List<RszObject> objectList) {
        // Add all child objects first.
        for (var i = 0; i < structInfo.fields!.Count; i++) {
            var field          = structInfo.fields[i];
            var fieldName      = field.name?.ToConvertedFieldName()!;
            var fieldInfo      = GetType().GetProperty(fieldName)!;
            var isObjectType   = field.type == "Object";
            var fieldGetMethod = fieldInfo.GetMethod!;

            if (isObjectType) {
                if (field.array) { // Array of pointers.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    foreach (var obj in list) {
                        ((RszObject) obj).WriteObjectList(objectList);
                    }
                } else { // Pointer to object.
                    // So it works in the UI, we always put the object in a list. Thus even if not an array, we need to extract from a list.
                    var list = (IList) fieldGetMethod.Invoke(this, null)!;
                    ((RszObject) list[0]!).WriteObjectList(objectList);
                }
            }
        }

        objectList.Add(this);
    }

    public override string? ToString() {
        return structInfo.name;
    }
}

public static class RszObjectExtensions {
    public static int GetAlign(this StructJson.Field field) {
        return field.array ? 4 : field.align;
    }

    public static Type? AsType(this string? typeName) {
        return typeName == null ? null : Type.GetType("RE_Editor.Common.Structs." + typeName, true);
    }

    public static List<T> GetDataAsList<T>(this byte[] bytes, int size, int arrayCount, StructJson.Field field) {
        var genericArgs = typeof(T).GenericTypeArguments;
        var isGeneric   = genericArgs.Length != 0;
        var genericType = isGeneric ? genericArgs[0] : null;
        var list        = new List<T>(arrayCount);

        for (var i = 0; i < arrayCount; i++) {
            var startPos = i * size;
            var sub      = bytes.Subsequence(startPos, size);
            if (isGeneric) {
                //var getDataAs = typeof(Extensions).GetMethod(nameof(Extensions.GetDataAs), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new[] {typeof(IEnumerable<byte>)}, null)
                //                                  ?.MakeGenericMethod(genericType!);
                //var data = getDataAs?.Invoke(null, new object[] {sub}) ?? throw new("sub.GetDataAs failure.");
                var data = sub.GetDataAs(genericType!);
                T   wrapper;
                if (typeof(T).GetGenericTypeDefinition().Is(typeof(DataSourceWrapper<>))) {
                    wrapper = (T) Activator.CreateInstance(typeof(T), i, data, field)!;
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

    //public static void SetDataFromList<T>(this RszObject.FieldData fieldData, IList<T> list) where T : notnull {
    //    var size      = fieldData.fieldInfo.size;
    //    var byteCount = list.Count * size;
    //    var bytes     = new List<byte>(byteCount);

    //    foreach (var entry in list) {
    //        byte[] data;
    //        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
    //        if (entry.GetType().IsGeneric(typeof(IListWrapper<>))) {
    //            var  value     = ((dynamic) entry).Value;
    //            Type valueType = value.GetType();
    //            var  getBytes  = typeof(Extensions).GetMethod(nameof(Extensions.GetBytes), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.MakeGenericMethod(valueType);
    //            data = (byte[]) (getBytes?.Invoke(null, new object[] {value}) ?? throw new("sub.GetDataAs failure."));
    //        } else {
    //            data = entry.GetBytes();
    //        }
    //        bytes.AddRange(data);
    //    }

    //    if (bytes.Count != byteCount) throw new InvalidOperationException($"Resultant byte data array size is unexpected: found: `{bytes.Count}`, expected: `{byteCount}`.");

    //    fieldData.arrayCount = list.Count;
    //    fieldData.data       = bytes.ToArray();
    //}

    //public static RszObject.FieldData? getFieldByName(this Dictionary<int, RszObject.FieldData> fieldData, string name) {
    //    // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
    //    foreach (var value in fieldData.Values) {
    //        if (value.fieldInfo.name == name) return value;
    //    }
    //    return null;
    //}
}