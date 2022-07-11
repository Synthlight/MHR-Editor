using System.Collections.ObjectModel;
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
    public    StructJson structInfo;
    protected RSZ        rsz;
    protected int        userDataRef = -1;

    [SortOrder(int.MaxValue - 1000)]
    public int Index { get; set; }

    public static RszObject Read(BinaryReader reader, uint hash, RSZ rsz, int userDataRef) {
        if (userDataRef > -1) {
            return new UserDataShell {
                userDataRef = userDataRef,
                rsz         = rsz
            };
        }

        if (!DataHelper.STRUCT_INFO.ContainsKey(hash)) throw new FileNotSupported();

        var structInfo = DataHelper.STRUCT_INFO[hash];
        var rszObject  = CreateRszObjectInstance(hash);
        rszObject.structInfo = structInfo;
        rszObject.rsz        = rsz;

        for (var i = 0; i < structInfo.fields!.Count; i++) {
            var field          = structInfo.fields[i];
            var fieldName      = field.name?.ToConvertedFieldName()!;
            var primitiveName  = field.GetCSharpType();
            var viaType        = field.type?.GetViaType().AsType();
            var isNonPrimitive = primitiveName == null;
            var isObjectType   = field.type == "Object";

            // Be careful with lists. The 'align' in them refers to their contents, not their count themselves, which is always a 4-aligned int.
            var align = field.array ? 4 : field.align;
            reader.Align(align);

            var fieldInfo        = rszObject.GetType().GetProperty(fieldName)!;
            var fieldType        = fieldInfo.PropertyType;
            var fieldGenericType = fieldType.IsGenericType ? fieldType.GenericTypeArguments[0] : null; // GetInnermostGenericType(fieldType);
            var fieldSetMethod   = fieldInfo.SetMethod!;

            // TODO: Strings & data objects.

            if (field.array) {
                if (isObjectType) { // Array of pointers.
                    var arrayCount = reader.ReadInt32();
                    var objects    = new List<RszObject>();
                    for (var index = 0; index < arrayCount; index++) {
                        objects.Add(rsz.objectData[reader.ReadInt32() - 1]);
                    }
                    var items = objects.GetGenericItemsOfType(fieldGenericType!);
                    SetList(items, fieldSetMethod, rszObject);
                } else if (isNonPrimitive) { // Array of embedded objects. (Built-in types like via.vec2.)
                    var arrayCount = reader.ReadInt32();
                    reader.Align(field.align);
                    var objects = new List<IViaType>(arrayCount);
                    for (var s = 0; s < arrayCount; s++) {
                        var instance = (IViaType) Activator.CreateInstance(viaType!)!;
                        instance.Read(reader);
                        objects.Add(instance);
                    }
                    var items = objects.GetGenericItemsOfType(fieldGenericType!);
                    SetList(items, fieldSetMethod, rszObject);
                } else { // Primitive array.
                    var arrayCount    = reader.ReadInt32();
                    var bytes         = reader.ReadBytes(field.size * arrayCount);
                    var genericMethod = typeof(RszObjectExtensions).GetMethod(nameof(RszObjectExtensions.GetDataAsList))!.MakeGenericMethod(fieldGenericType!);
                    var items         = genericMethod.Invoke(null, new object[] {bytes, field.size, arrayCount, field})!;
                    SetList(items, fieldSetMethod, rszObject);
                }
            } else {
                if (isObjectType) { // Pointer to object.
                    var objects = new List<RszObject> {rsz.objectData[reader.ReadInt32() - 1]};
                    var items   = objects.GetGenericItemsOfType(fieldGenericType!);
                    SetList(items, fieldSetMethod, rszObject);
                } else if (isNonPrimitive) { // Embedded object. (A built-in type like via.vec2.)
                    reader.Align(field.align);
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

    private static Type? GetInnermostGenericType(Type type) {
        if (!type.IsGenericType) return null;
        while (type.IsGenericType) {
            type = type.GenericTypeArguments[0];
        }
        return type;
    }

    public static ObservableCollection<T> MakeGenericObservableCollection<T>(IEnumerable<T> itemSource) {
        return itemSource is ObservableCollection<T> source ? source : new(itemSource);
    }

    /**
     * If the hash isn't found it'll just return the base `RszObject`.
     */
    private static RszObject CreateRszObjectInstance(uint hash) {
        var rszType   = DataHelper.MHR_STRUCTS.TryGet(hash, typeof(RszObject));
        var rszObject = (RszObject) Activator.CreateInstance(rszType) ?? new RszObject();
        return rszObject;
    }

    public void Write(BinaryWriter writer) {
        //foreach (var data in fieldData.Values) {
        //    writer.PadTill(() => {
        //        var align = data.fieldInfo.array ? 4 : data.fieldInfo.align;
        //        return writer.BaseStream.Position % align != 0;
        //    });
        //    if (data.isArray) writer.Write(data.arrayCount);
        //    writer.Write(data.data);
        //}
    }

    public override string? ToString() {
        return structInfo.name;
    }
}

public static class RszObjectExtensions {
    public static void Align(this BinaryReader reader, int align) {
        while (reader.BaseStream.Position % align != 0) {
            reader.BaseStream.Seek(1, SeekOrigin.Current);
        }
    }

    public static Type? AsType(this string? typeName) {
        return typeName == null ? null : Type.GetType("MHR_Editor.Common.Structs." + typeName, true);
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