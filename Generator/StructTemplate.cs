using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Models;
using RE_Editor.Common.Structs;
using RE_Editor.Generator.Models;

#if DD2
using RE_Editor.Common.Data;
#elif MHR
using RE_Editor.Common.Data;
#elif RE4
using RE_Editor.Common.Data;
#endif

namespace RE_Editor.Generator;

public class StructTemplate {
    private readonly GenerateFiles           generator;
    public readonly  string                  hash;
    public readonly  StructJson              structInfo;
    private readonly string                  className;
    private readonly Dictionary<string, int> usedNames = new();
    private          int                     sortOrder = 1000;
    private readonly string?                 parentClass;

    public StructTemplate(GenerateFiles generator, StructType structType) {
        this.generator = generator;
        hash           = structType.hash;
        structInfo     = structType.structInfo;
        className      = structType.name;
        parentClass    = GetParent();
    }

    public void Generate(bool dryRun) {
        var       filename = $@"{GenerateFiles.STRUCT_GEN_PATH}\{className}.cs";
        using var file     = new StreamWriter(dryRun ? new MemoryStream() : File.Open(filename, FileMode.Create, FileAccess.Write));
        WriteUsings(file);
        WriteClassHeader(file);
        if (structInfo.fields != null) {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structInfo.fields) {
                if (string.IsNullOrEmpty(field.originalType)) {
                    switch (field.type) {
                        case "Data":
                            switch (field.size) {
                                case 4:
                                    field.type         = "F32";
                                    field.originalType = "System.Single";
                                    break;
                                default:
                                    field.type         = nameof(UIntArray);
                                    field.originalType = nameof(UIntArray);
                                    break;
                            }
                            break;
                        case "String":
                            field.originalType = "System.String";
                            break;
                    }
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
        file.WriteLine($"public partial class {className} : {parentClass ?? nameof(RszObject)} {{");
        file.WriteLine($"    public {(parentClass == null ? "const" : "new const")} uint HASH = 0x{hash};");
    }

    private void WriteProperty(TextWriter file, StructJson.Field field) {
        if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) return;
        if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) return;

        var newName             = field.name?.ToConvertedFieldName()!;
        var primitiveName       = field.GetCSharpType();
        var typeName            = field.originalType!.ToConvertedTypeName();
        var isPrimitive         = primitiveName != null;
        var isEnumType          = typeName != null && generator.enumTypes.ContainsKey(typeName);
        var buttonType          = GetButtonType(field);
        var isNonPrimitive      = !isPrimitive && !isEnumType; // via.thing
        var isUserData          = field.type == "UserData";
        var isObjectType        = field.type == nameof(Object);
        var viaType             = GetViaType(field, isNonPrimitive, typeName, ref isObjectType, isUserData);
        var negativeOneForEmpty = GetNegativeForEmptyAllowed(field);

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

        if (field.type == nameof(UIntArray)) {
            file.WriteLine("    [IsList]");
            file.WriteLine($"    public ObservableCollection<{nameof(UIntArray)}> {newName} {{ get; set; }}");
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
                file.WriteLine($"    public ObservableCollection<DataSourceWrapper<{primitiveName}>> {newName} {{ get; set; }}");
            } else if (isUserData) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<{nameof(UserDataShell)}> {newName} {{ get; set; }}");
            } else if (isNonPrimitive && viaType != null) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<{viaType}> {newName} {{ get; set; }}");
            } else if (isObjectType) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<{typeName}> {newName} {{ get; set; }}");
            } else if (isEnumType) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<GenericWrapper<{typeName}>> {newName} {{ get; set; }}");
            } else if (isPrimitive) {
                file.WriteLine("    [IsList]");
                file.WriteLine($"    public ObservableCollection<GenericWrapper<{primitiveName}>> {newName} {{ get; set; }}");
            } else {
                throw new InvalidDataException("Not a primitive, enum, or object array type.");
            }
        } else {
            if (buttonType != null) { //  && field.name != "_Id" -- Not sure which needed this? Breaks for DD2 stuff like item drop params.
                var lookupName = GetLookupForDataSourceType(buttonType);
                file.WriteLine($"    [SortOrder({sortOrder + 10})]");
                file.WriteLine($"    [DataSource({nameof(DataSourceType)}.{buttonType})]");
                if (negativeOneForEmpty) file.WriteLine("    [NegativeOneForEmpty]");
                file.WriteLine($"    public {primitiveName} {newName} {{ get; set; }}");
                file.WriteLine("");
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    [DisplayName(\"{newName}\")]");
#if MHR
                file.WriteLine($"    public string {newName}_button => DataHelper.{lookupName}[Global.locale].TryGet((uint) {newName}).ToStringWithId({newName}{(buttonType == DataSourceType.ITEMS ? ", true" : "")});");
#elif RE4
                file.WriteLine($"    public string {newName}_button => {(negativeOneForEmpty ? $"{newName} == -1 ? \"<None>\".ToStringWithId({newName}) : " : "")}" +
                               $"DataHelper.{lookupName}[Global.variant][Global.locale].TryGet((uint) {newName}).ToStringWithId({newName}{(buttonType == DataSourceType.ITEMS ? ", true" : "")});");
#else
                file.WriteLine($"    public string {newName}_button => {(negativeOneForEmpty ? $"{newName} == -1 ? \"<None>\".ToStringWithId({newName}) : " : "")}" +
                               $"DataHelper.{lookupName}[Global.locale].TryGet((uint) {newName}).ToStringWithId({newName});");
#endif
            } else if (viaType?.Is(typeof(ISimpleViaType)) == true) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {viaType} {newName} {{ get; set; }}");
            } else if (isUserData) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public ObservableCollection<{nameof(UserDataShell)}> {newName} {{ get; set; }}");
            } else if (isNonPrimitive && viaType != null) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public ObservableCollection<{viaType}> {newName} {{ get; set; }}");
            } else if (isObjectType) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public ObservableCollection<{typeName}> {newName} {{ get; set; }}");
            } else if (isEnumType) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {typeName} {newName} {{ get; set; }}");
            } else if (isPrimitive) {
                file.WriteLine($"    [SortOrder({sortOrder})]");
                file.WriteLine($"    public {primitiveName} {newName} {{ get; set; }}");
            } else {
                throw new InvalidDataException("Not a primitive, enum, or object type.");
            }
        }

        sortOrder += 100;
    }

    private static string? GetViaType(StructJson.Field field, bool isNonPrimitive, string? typeName, ref bool isObjectType, bool isUserData) {
        // We do it here and later since we sometimes overwrite them.
        var viaType = field.originalType?.GetViaType();

        if (typeName == "Via_Prefab") {
            viaType      = nameof(Prefab);
            isObjectType = false;
        } else if (typeName == "System_Type") {
            viaType      = nameof(Type);
            isObjectType = false;
        } else if (isNonPrimitive && !isObjectType && !isUserData) {
            // This makes sure we've implemented the via type during generation.
            viaType = field.type!.GetViaType() ?? throw new NotImplementedException($"Hard-coded type '{field.type}' not implemented.");
        }
        return viaType;
    }

    private void WriteClassCreate(TextWriter file) {
        if (className.StartsWith("Via_")) return;

        var modifier = parentClass == null ? "" : "new ";

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
            var buttonType     = GetButtonType(field);
            var isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
            var isUserData     = field.type == "UserData";
            var isObjectType   = field.type == "Object";
            var viaType        = GetViaType(field, isNonPrimitive, typeName, ref isObjectType, isUserData);

            if (viaType?.Is(typeof(ISimpleViaType)) == true
                || isUserData
                || (isNonPrimitive && viaType != null)
                || isObjectType) {
                file.WriteLine($"        obj.{newName} = new();");
            } else if (isEnumType && !field.array && buttonType == null) {
                file.WriteLine($"        obj.{newName} = Enum.GetValues<{typeName}>()[0];");
            }
        }

        file.WriteLine($"        return obj;");
        file.WriteLine("    }");
    }

    private void WriteClassCopy(TextWriter file) {
        if (className.StartsWith("Via_")) return;

        var modifier = parentClass == null ? "virtual" : "override";

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
            var buttonType     = GetButtonType(field);
            var isNonPrimitive = !isPrimitive && !isEnumType; // via.thing
            var isUserData     = field.type == "UserData";
            var isObjectType   = field.type == "Object";
            var viaType        = GetViaType(field, isNonPrimitive, typeName, ref isObjectType, isUserData);

            // TODO: Fix generic/dataSource wrappers.

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (!field.array && viaType?.Is(typeof(ISimpleViaType)) == true) {
                file.WriteLine($"        obj.{newName} = new();");
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

    private DataSourceType? GetButtonType(StructJson.Field field) {
        // This part check the class + field name.
        var name = $"{className}.{field.name.ToConvertedFieldName()}";
#pragma warning disable IDE0066
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (name) {
#if DD2
            // Many of these don't seem to be the enum type, probably because the enum doesn't allow zero but the fields do.
            case "App_ArmorEnhanceParam.ItemId":
            case "App_ArmorEnhanceParam.NeedItemId0":
            case "App_ArmorEnhanceParam.NeedItemId1":
            case "App_ItemDataParam.ItemDropId":
            case "App_ItemDataParam.DecayedItemId":
            case "App_ItemDropParam_Table_Item.Id":
            case "App_ItemShopBuyParam.ItemId":
            case "App_ItemShopSellParam.ItemId":
            case "App_WeaponEnhanceParam.ItemId":
            case "App_WeaponEnhanceParam.NeedItemId0":
            case "App_WeaponEnhanceParam.NeedItemId1":
                return DataSourceType.ITEMS;
#endif
        }
#pragma warning restore IDE0066

        // And this check the original type.
        return field.originalType?.Replace("[]", "") switch {
#if DD2
            "app.ItemIDEnum" => DataSourceType.ITEMS,
#elif MHR
            "snow.data.ContentsIdSystem.ItemId" => DataSourceType.ITEMS,
            "snow.data.DataDef.PlEquipSkillId" => DataSourceType.SKILLS,
            "snow.data.DataDef.PlHyakuryuSkillId" => DataSourceType.RAMPAGE_SKILLS,
            "snow.data.DataDef.PlKitchenSkillId" => DataSourceType.DANGO_SKILLS,
            "snow.data.DataDef.PlWeaponActionId" => DataSourceType.SWITCH_SKILLS,
#elif RE4
            "chainsaw.ItemID" => DataSourceType.ITEMS,
            "chainsaw.WeaponID" => DataSourceType.WEAPONS,
#endif
            _ => null
        };
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private string? GetParent() {
        return className switch {
#if DD2
            "App_gc_pl_BurningLightBow" => "App_GenericConditionBase",
            "App_gc_pl_CarryAim" => "App_GenericConditionBase",
            "App_gc_pl_CarryObjectAim" => "App_GenericConditionBase",
            "App_gc_pl_DrawBow" => "App_GenericConditionBase",
            "App_gc_pl_DrawMagicBow" => "App_GenericConditionBase",
            "App_gc_pl_FireStorm" => "App_GenericConditionBase",
            "App_gc_pl_ItemCh255HeadAim" => "App_GenericConditionBase",
            "App_gc_pl_Job02BodyBinder" => "App_GenericConditionBase",
            "App_gc_pl_Job02FullBend" => "App_GenericConditionBase",
            "App_gc_pl_Job02FullBlast" => "App_GenericConditionBase",
            "App_gc_pl_Job02MeteorShot" => "App_GenericConditionBase",
            "App_gc_pl_Job02RandomShot" => "App_GenericConditionBase",
            "App_gc_pl_Job07Gungnir" => "App_GenericConditionBase",
            "App_gc_pl_Job07PsychoLauncher" => "App_GenericConditionBase",
            "App_gc_pl_Job09DetectFregrance" => "App_GenericConditionBase",
            "App_gc_pl_PreparingSpell" => "App_GenericConditionBase",
            "App_gc_pl_PreparingSpellOfAnodyne" => "App_GenericConditionBase",
            "App_gc_pl_SpiritArrowBow" => "App_GenericConditionBase",
            "App_gc_pl_ThunderChainBow" => "App_GenericConditionBase",
#elif RE4
            "Chainsaw_ItemUseResult_HealHitPoint" => "Chainsaw_ItemUseResultInfoBase",
            "Chainsaw_ItemUseResult_IncreaseHitPoint" => "Chainsaw_ItemUseResultInfoBase",
            "Chainsaw_WeaponItem" => "Chainsaw_Item",
            "Chainsaw_UniqueItem" => "Chainsaw_Item",
            "Chainsaw_RuleStratum_ParticleChapter" => "Chainsaw_RuleStratum_Particle",
            "Chainsaw_RuleStratum_ParticleFlag" => "Chainsaw_RuleStratum_Particle",
            "Chainsaw_RuleStratum_ParticleGmFlag" => "Chainsaw_RuleStratum_Particle",
            "Chainsaw_RuleStratum_ParticleItemInventory" => "Chainsaw_RuleStratum_Particle",
            "Chainsaw_RuleStratum_ParticleResourcePoint" => "Chainsaw_RuleStratum_Particle",
            "Chainsaw_RuleStratum_ParticleStatusEffect" => "Chainsaw_RuleStratum_Particle",
            "Chainsaw_RuleStratum_ParticleTrue" => "Chainsaw_RuleStratum_Particle",
#endif
            _ => null
        };
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
#elif MHR
            DataSourceType.DANGO_SKILLS => nameof(DataHelper.DANGO_SKILL_NAME_LOOKUP),
            DataSourceType.ITEMS => nameof(DataHelper.ITEM_NAME_LOOKUP),
            DataSourceType.RAMPAGE_SKILLS => nameof(DataHelper.RAMPAGE_SKILL_NAME_LOOKUP),
            DataSourceType.SKILLS => nameof(DataHelper.SKILL_NAME_LOOKUP),
            DataSourceType.SWITCH_SKILLS => nameof(DataHelper.SWITCH_SKILL_NAME_LOOKUP),
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