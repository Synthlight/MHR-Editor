﻿using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Models;
using RE_Editor.Common.Structs;
using RE_Editor.Generator.Models;
using Guid = RE_Editor.Common.Structs.Guid;

#if DD2
using RE_Editor.Common.Data;
#elif DRDR
using RE_Editor.Common.Data;
#elif MHR
using RE_Editor.Common.Data;
#elif MHWS
using RE_Editor.Common.Data;
#elif RE2
using RE_Editor.Common.Data;
#elif RE3
using RE_Editor.Common.Data;
#elif RE4
using RE_Editor.Common.Data;
#endif

namespace RE_Editor.Generator;

public class StructTemplate(GenerateFiles generator, StructType structType) {
    public readonly  string                  hash        = structType.hash;
    public readonly  StructJson              structInfo  = structType.structInfo;
    private readonly string                  className   = structType.name;
    private readonly Dictionary<string, int> usedNames   = [];
    private          int                     sortOrder   = 1000;
    private readonly string?                 parentClass = structType.parent;

    public void Generate(bool dryRun) {
        var       filename = $@"{GenerateFiles.STRUCT_GEN_PATH}\{className}.cs";
        using var file     = new StreamWriter(dryRun ? new MemoryStream() : File.Open(filename, FileMode.Create, FileAccess.Write));
        WriteUsings(file);
        WriteClassHeader(file);
        if (structInfo.fields != null) {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structInfo.fields) {
                if (string.IsNullOrEmpty(field.originalType) && structInfo.name!.StartsWith("via")) {
                    switch (field.type) {
                        case "Data":
                            if (field.size == 4) {
                                field.type         = "F32";
                                field.originalType = "System.Single";
                            } else {
                                field.type         = nameof(UIntArray);
                                field.originalType = nameof(UIntArray);
                            }
                            break;
                        case "String":
                            field.originalType = "System.String";
                            break;
                    }
                }
                if (structInfo.name is "app.AttackUserData" or "app.HitBaseUserData" or "via.physics.RequestSetColliderUserData" && field.name == "v1") {
                    field.type         = "Object";
                    field.originalType = "via.physics.UserData";
                }
                if (string.IsNullOrEmpty(field.name)) continue;
                WriteProperty(file, field);
            }
        }
        WriteClassCreate(file);
        WriteClassCopy(file);
        WriteClassFooter(file);
        file.Flush();
        file.Close();
    }

    private static void WriteUsings(TextWriter file) {
        file.WriteLine("// ReSharper disable All");
        file.WriteLine("using System.Collections.ObjectModel;");
        file.WriteLine("using System.ComponentModel;");
        file.WriteLine("using System.Diagnostics.CodeAnalysis;");
        file.WriteLine("using System.Globalization;");
        file.WriteLine("using RE_Editor.Common;");
        file.WriteLine("using RE_Editor.Common.Attributes;");
        file.WriteLine("using RE_Editor.Common.Data;");
        file.WriteLine("using RE_Editor.Common.Models;");
        file.WriteLine("using RE_Editor.Common.Models.List_Wrappers;");
        file.WriteLine("using RE_Editor.Common.Structs;");
        file.WriteLine("using RE_Editor.Models.Enums;");
        file.WriteLine("using DateTime = RE_Editor.Common.Structs.DateTime;");
        file.WriteLine("using Guid = RE_Editor.Common.Structs.Guid;");
        file.WriteLine("using Range = RE_Editor.Common.Structs.Range;");
        file.WriteLine("using Size = RE_Editor.Common.Structs.Size;");
    }

    private void WriteClassHeader(TextWriter file) {
        file.WriteLine("");
        file.WriteLine("#pragma warning disable CS8600");
        file.WriteLine("#pragma warning disable CS8601");
        file.WriteLine("#pragma warning disable CS8602");
        file.WriteLine("#pragma warning disable CS8603");
        file.WriteLine("#pragma warning disable CS8618");
        file.WriteLine("");
        file.WriteLine("namespace RE_Editor.Models.Structs;");
        file.WriteLine("");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"InconsistentNaming\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"UnusedMember.Global\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"ClassNeverInstantiated.Global\")]");
        file.WriteLine("[SuppressMessage(\"ReSharper\", \"IdentifierTypo\")]");
        file.WriteLine("[SuppressMessage(\"CodeQuality\", \"IDE0079:Remove unnecessary suppression\")]");
        file.WriteLine("[MhrStruct]");
        file.WriteLine($"// {structInfo.name}");
        file.WriteLine($"public partial class {className} : {parentClass ?? nameof(RszObject)} {{");
        file.WriteLine($"    public {(parentClass == null ? "const" : "new const")} uint HASH = 0x{hash};");
    }

    private void WriteProperty(TextWriter file, StructJson.Field field) {
        if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) return;
        if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) return;

        // Happened for some RE4 types.
        if (field.originalType == "") {
            var convertedTypeName = field.GetCSharpType();
            if (convertedTypeName != null) {
                field.originalType = convertedTypeName;
            }

            if (field.originalType == "") {
                Console.WriteLine($"Warning: Unknown originalType, skipping: {structInfo.name}::{field.name}");
                return;
            }
        }

        var newName             = field.name?.ToConvertedFieldName()!;
        var primitiveName       = field.GetCSharpType();
        var typeName            = field.originalType!.ToConvertedTypeName();
        var isPrimitive         = primitiveName != null;
        var isEnumType          = typeName != null && generator.enumTypes.ContainsKey(typeName);
        var buttonType          = field.buttonType;
        var isNonPrimitive      = !isPrimitive && !isEnumType; // via.thing
        var isUserData          = field.type == "UserData";
        var isObjectType        = field.type == nameof(Object);
        var viaType             = GetViaType(field, isNonPrimitive, typeName, ref isObjectType, isUserData);
        var negativeOneForEmpty = GetNegativeForEmptyAllowed(field);
        var modifier            = ""; // `override` or `virtual`

        if (field.overrideCount > 0) modifier     = "override ";
        else if (field.virtualCount > 0) modifier = "virtual ";

        if (!usedNames.TryAdd(newName, 1)) {
            usedNames[newName]++;
            newName += usedNames[newName].ToString();
        }

        file.WriteLine("");

        /*
        if (field.name!.ToLower() == "_id") {
            file.WriteLine("    [ShowAsHex]");
            isEnumType = false;
        }
        */

        file.WriteLine($"    // {field.name}");
        file.WriteLine($"    // {field.originalType}");
        if (field.type == nameof(UIntArray)) {
            file.WriteLine("    [IsList]");
            file.WriteLine($"    public {modifier}ObservableCollection<{nameof(UIntArray)}> {newName} {{ get; set; }}");
        } else if (field.array) {
            file.WriteLine($"    [SortOrder({sortOrder})]");
            if (buttonType != null) {
                if (primitiveName == null) {
                    throw new InvalidDataException("Button type found but primitiveName is null.");
                }
                file.WriteLine($"    [DataSource({nameof(DataSourceType)}.{buttonType})]");
                foreach (var additionalAttributes in GetAdditionalAttributesForDataSourceType(buttonType)) {
                    file.WriteLine($"    {additionalAttributes}");
                }
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public {modifier}ObservableCollection<DataSourceWrapper<{primitiveName}>> {newName} {{ get; set; }}");
            } else if (isUserData) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public {modifier}ObservableCollection<{nameof(UserDataShell)}> {newName} {{ get; set; }}");
            } else if (isNonPrimitive && viaType != null) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public {modifier}ObservableCollection<{viaType}> {newName} {{ get; set; }}");
            } else if (isObjectType) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public {modifier}ObservableCollection<{typeName}> {newName} {{ get; set; }}");
            } else if (isEnumType) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public {modifier}ObservableCollection<GenericWrapper<{typeName}>> {newName} {{ get; set; }}");
            } else if (isPrimitive) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public {modifier}ObservableCollection<GenericWrapper<{primitiveName}>> {newName} {{ get; set; }}");
            } else {
                throw new InvalidDataException("Not a primitive, enum, or object array type.");
            }
        } else {
            if (buttonType != null) { //  && field.name != "_Id" -- Not sure which needed this? Breaks for DD2 stuff like item drop params.
                var lookupName = GetLookupForDataSourceType(buttonType);
                file.WriteLine($"    [SortOrder({sortOrder + 10})]");
                file.WriteLine($"    [DataSource({nameof(DataSourceType)}.{buttonType})]");
                if (negativeOneForEmpty) file.WriteLine("    [NegativeOneForEmpty]");
                file.WriteLine($"    public {modifier}{primitiveName} {newName} {{ get; set; }}");
                file.WriteLine("");
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    [DisplayName(\"{newName}\")]");
#if MHR
                file.WriteLine($"    public {modifier}string {newName}_button => DataHelper.{lookupName}[Global.locale].TryGet((uint) {newName}).ToStringWithId({newName}{(buttonType == DataSourceType.ITEMS ? ", true" : "")});");
#elif RE4
                file.WriteLine($"    public {modifier}string {newName}_button => {(negativeOneForEmpty ? $"{newName} == -1 ? \"<None>\".ToStringWithId({newName}) : " : "")}" +
                               $"DataHelper.{lookupName}[Global.variant][Global.locale].TryGet((uint) {newName}).ToStringWithId({newName}{(buttonType == DataSourceType.ITEMS ? ", true" : "")});");
#else
                file.WriteLine($"    public {modifier}string {newName}_button => {(negativeOneForEmpty ? $"{newName} == -1 ? \"<None>\".ToStringWithId({newName}) : " : "")}" +
                               $"DataHelper.{lookupName}[Global.locale].TryGet((uint) {newName}).ToStringWithId({newName});");
#endif
            } else if (viaType?.Is(typeof(ISimpleViaType)) == true) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {modifier}{viaType} {newName} {{ get; set; }}");
            } else if (isUserData) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {modifier}ObservableCollection<{nameof(UserDataShell)}> {newName} {{ get; set; }}");
            } else if (isNonPrimitive && viaType != null) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {modifier}ObservableCollection<{viaType}> {newName} {{ get; set; }}");
            } else if (isObjectType) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {modifier}ObservableCollection<{typeName}> {newName} {{ get; set; }}");
            } else if (isEnumType) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {modifier}{typeName} {newName} {{ get; set; }}");
            } else if (isPrimitive) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {modifier}{primitiveName} {newName} {{ get; set; }}");
            } else {
                throw new InvalidDataException("Not a primitive, enum, or object type.");
            }
        }

        sortOrder += 100;
    }

    private static string? GetViaType(StructJson.Field field, bool isNonPrimitive, string? typeName, ref bool isObjectType, bool isUserData) {
        // We do it here and later since we sometimes overwrite them.
        var viaType = field.originalType?.GetViaType();

        switch (typeName) {
            case "Via_Prefab":
                viaType      = nameof(Prefab);
                isObjectType = false;
                break;
            case "System_Type":
                viaType      = nameof(Type);
                isObjectType = false;
                break;
            case "Via_OBB":
                viaType      = nameof(UIntArray);
                isObjectType = false;
                break;
            default: {
                if (isNonPrimitive && !isObjectType && !isUserData) {
                    // This makes sure we've implemented the via type during generation.
                    viaType = field.type!.GetViaType() ?? throw new NotImplementedException($"Hard-coded type '{field.type}' not implemented.");
                }
                break;
            }
        }
        return viaType;
    }

    private void WriteClassCreate(TextWriter file) {
        if (className.StartsWith("Via_")) return;

        var isParentViaType = parentClass?.ToLower().StartsWith("via") ?? false;
        var modifier        = parentClass == null || isParentViaType ? "" : "new ";

        file.WriteLine("");
        file.WriteLine($"    public {modifier}static {className} Create(RSZ rsz) {{");
        file.WriteLine($"        var obj = Create<{className}>(rsz, HASH);");

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var field in structInfo.fields!) {
            if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
            if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
            if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;

            var newName        = field.name?.ToConvertedFieldName()!;
            var primitiveName  = field.GetCSharpType();
            var typeName       = field.originalType!.ToConvertedTypeName();
            var isPrimitive    = primitiveName != null;
            var isEnumType     = typeName != null && generator.enumTypes.ContainsKey(typeName);
            var buttonType     = field.buttonType;
            var isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
            var isUserData     = field.type == "UserData";
            var isObjectType   = field.type == "Object";
            var viaType        = GetViaType(field, isNonPrimitive, typeName, ref isObjectType, isUserData);

            if (!field.array && isObjectType && viaType == null && typeName != null && !isEnumType) {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (typeName.StartsWith("Via")) { // For things like `Via_AnimationCurve` which generate from the json but aren't our manually implemented `via` types.
                    file.WriteLine($"        obj.{newName} = [new()];");
                } else {
                    file.WriteLine($"        obj.{newName} = [{typeName}.Create(rsz)];");
                }
            } else if (viaType?.Is(typeof(ISimpleViaType)) == true
                       || isUserData
                       || (isNonPrimitive && viaType != null)
                       || isObjectType) {
                if (!field.array && viaType != null
                                 && !viaType.Is(typeof(ISimpleViaType))
                                 && viaType != nameof(Type)
                                 && viaType != nameof(Prefab)
                                 && viaType != nameof(UIntArray)) {
                    file.WriteLine($"        obj.{newName} = [new()];");
                } else {
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (!field.array && viaType == nameof(Guid)) {
                        file.WriteLine($"        obj.{newName} = Guid.New();");
                    } else {
                        file.WriteLine($"        obj.{newName} = new();");
                    }
                }
            } else if (isEnumType && !field.array && buttonType == null) {
                file.WriteLine($"        obj.{newName} = Enum.GetValues<{typeName}>()[0];");
            } else if ((isEnumType || isPrimitive) && field.array) {
                file.WriteLine($"        obj.{newName} = new(new());"); // GenericWrapper
            }
        }

        file.WriteLine("        return obj;");
        file.WriteLine("    }");
    }

    private void WriteClassCopy(TextWriter file) {
        if (className.StartsWith("Via_")) return;

        var isParentViaType = parentClass?.ToLower().StartsWith("via") ?? false;
        var modifier        = parentClass == null || isParentViaType ? "virtual" : "override";

        file.WriteLine("");
        file.WriteLine($"    public {modifier} {className} Copy() {{");

        file.WriteLine($"        var obj = base.Copy<{className}>();");

        foreach (var field in structInfo.fields!) {
            if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
            if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
            if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;

            var newName        = field.name?.ToConvertedFieldName()!;
            var primitiveName  = field.GetCSharpType();
            var typeName       = field.originalType!.ToConvertedTypeName();
            var isPrimitive    = primitiveName != null;
            var isEnumType     = typeName != null && generator.enumTypes.ContainsKey(typeName);
            var buttonType     = field.buttonType;
            var isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
            var isUserData     = field.type == "UserData";
            var isObjectType   = field.type == "Object";
            var viaType        = GetViaType(field, isNonPrimitive, typeName, ref isObjectType, isUserData);

            // TODO: Fix generic/dataSource wrappers.

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (!field.array && viaType?.Is(typeof(ISimpleViaType)) == true) {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (!field.array && viaType == nameof(Guid)) {
                    file.WriteLine($"        obj.{newName} = {newName}.Copy();");
                } else {
                    file.WriteLine($"        obj.{newName} = new();");
                }
            } else if ((field.array || isObjectType || isNonPrimitive) && buttonType == null) {
                file.WriteLine($"        obj.{newName} ??= new();");
                file.WriteLine($"        foreach (var x in {newName}) {{");
                if (typeName == "System_Type" || viaType == "Type") { // `Type` is a built-in type, no copy.
                    file.WriteLine($"            obj.{newName}.Add(x);");
                } else {
                    if (viaType?.Is(typeof(ISimpleViaType)) == true) {
                        file.WriteLine($"            obj.{newName}.Add(x.Copy());");
                    } else if (isObjectType && viaType == null && isNonPrimitive && typeName?.Contains("GenericWrapper") == false) {
                        file.WriteLine($"            obj.{newName}.Add({(isEnumType ? "x" : $"x.Copy<{typeName}>()")});");
                    } else {
                        file.WriteLine($"            obj.{newName}.Add({(isEnumType ? "x" : "x.Copy()")});");
                    }
                }
                file.WriteLine("        }");
            } else {
                file.WriteLine($"        obj.{newName} = {newName};");
            }
        }

        file.WriteLine("        return obj;");
        file.WriteLine("    }");
    }

    private static void WriteClassFooter(TextWriter file) {
        file.Write("}");
    }

    private static List<string> GetAdditionalAttributesForDataSourceType(DataSourceType? dataSourceType) {
        return dataSourceType switch {
#if MHR
            DataSourceType.ITEMS => ["[ButtonIdAsHex]"],
#elif RE4
            DataSourceType.ITEMS => ["[ButtonIdAsHex]"],
            DataSourceType.WEAPONS => ["[ButtonIdAsHex]"],
#endif
            _ => []
        };
    }

    public static string GetLookupForDataSourceType(DataSourceType? dataSourceType) {
        return dataSourceType switch {
#if DD2
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
#elif DRDR
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
#elif MHR
            DataSourceType.DANGO_SKILLS => nameof(DataHelper.DANGO_SKILL_NAME_LOOKUP),
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
            DataSourceType.RAMPAGE_SKILLS => nameof(DataHelper.RAMPAGE_SKILL_NAME_LOOKUP),
            DataSourceType.SKILLS => nameof(DataHelper.SKILL_NAME_LOOKUP),
            DataSourceType.SWITCH_SKILLS => nameof(DataHelper.SWITCH_SKILL_NAME_LOOKUP),
#elif MHWS
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
            DataSourceType.SKILLS => nameof(DataHelper.SKILL_NAME_BY_ENUM_VALUE),
#elif RE2
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
            DataSourceType.WEAPONS => nameof(DataHelper.WEAPON_NAME_LOOKUP),
#elif RE3
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
            DataSourceType.WEAPONS => nameof(DataHelper.WEAPON_NAME_LOOKUP),
#elif RE4
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
            DataSourceType.WEAPONS => nameof(DataHelper.WEAPON_NAME_LOOKUP),
#endif
            _ => throw new ArgumentOutOfRangeException(dataSourceType.ToString())
        };
    }

    private static bool GetNegativeForEmptyAllowed(StructJson.Field field) {
        return field.name?.ToConvertedFieldName() switch {
#if RE4
            "CurrentAmmo" => true,
#endif
            _ => false
        };
    }
}